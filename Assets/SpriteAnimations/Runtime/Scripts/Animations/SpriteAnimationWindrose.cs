using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SpriteAnimations
{
    /// <summary>
    /// The Windrose Animation. This animation has multiple cycles, each referring to a cardinal
    /// position.
    /// </summary>
    [CreateAssetMenu(fileName = "Windrose Animation", menuName = "Sprite Animations/Windrose Animation")]
    public class SpriteAnimationWindrose : SpriteAnimation
    {
        #region Static

        /// <summary>
        /// Creates a new <see cref="SpriteAnimationWindrose"/> on demand.
        /// 
        /// Remember that all the sprites lists in the dictionary should have the same length 
        /// so the animation stay consistent.
        /// </summary>
        /// <param name="cycles"></param>
        /// <param name="isLoopable"></param>
        /// <returns></returns>
        public static SpriteAnimationWindrose OnDemand(int fps, Dictionary<WindroseDirection, List<Sprite>> cycles, bool isLoopable = false)
        {
            var animation = CreateInstance<SpriteAnimationWindrose>();
            animation.FPS = fps;
            animation.IsLoopable = isLoopable;

            bool lengthAlreadyWarned = false;
            int previousLength = -1;

            foreach (var pair in cycles)
            {
                LengthCheck(pair.Value.Count, previousLength);

                Cycle newCycle = new()
                {
                    Animation = animation
                };

                foreach (Sprite sprite in pair.Value)
                {
                    newCycle.AddFrame(sprite);
                }
                animation.Cycles[pair.Key] = newCycle;
            }

            void LengthCheck(int length, int previousLength)
            {
                if (previousLength < 0 || lengthAlreadyWarned) return;

                if (length != previousLength)
                {
                    Logger.LogWarning($"It was detected that the amount of sprites is not consinstent among all cycles. "
                        + $"Remember that this might cause the animation to not work as expected");
                    lengthAlreadyWarned = true;
                }
            }

            return animation;
        }

        /// <summary>
        /// Returns the windrose direction based on the signed input.
        /// </summary>
        /// <param name="signedInput">The signed input representing the x and y coordinates.</param>
        /// <returns>The windrose direction based on the signed input.</returns>
        public static WindroseDirection DirectionFromInput(Vector2Int signedInput)
        {
            int x = signedInput.x;
            int y = signedInput.y;

            if (x == 0 && y > 0)
                return WindroseDirection.North;
            if (x > 0 && y > 0)
                return WindroseDirection.NorthEast;
            if (x > 0 && y == 0)
                return WindroseDirection.East;
            if (x > 0 && y < 0)
                return WindroseDirection.SouthEast;
            if (x == 0 && y < 0)
                return WindroseDirection.South;
            if (x < 0 && y < 0)
                return WindroseDirection.SouthWest;
            if (x < 0 && y == 0)
                return WindroseDirection.West;
            if (x < 0 && y > 0)
                return WindroseDirection.NorthWest;

            return WindroseDirection.South;
        }

        /// <summary>
        /// Converts a WindroseDirection enum value into a Vector2Int representing the corresponding direction.
        /// </summary>
        /// <param name="direction">The WindroseDirection enum value.</param>
        /// <returns>A Vector2Int representing the direction.</returns>
        public static Vector2Int InputFromDirection(WindroseDirection direction)
        {
            return direction switch
            {
                WindroseDirection.North => new Vector2Int(0, 1), // North direction, returns (0, 1)
                WindroseDirection.NorthEast => new Vector2Int(1, 1), // North-East direction, returns (1, 1)
                WindroseDirection.East => new Vector2Int(1, 0), // East direction, returns (1, 0)
                WindroseDirection.SouthEast => new Vector2Int(1, -1), // South-East direction, returns (1, -1)
                WindroseDirection.South => new Vector2Int(0, -1), // South direction, returns (0, -1)
                WindroseDirection.SouthWest => new Vector2Int(-1, -1), // South-West direction, returns (-1, -1)
                WindroseDirection.West => new Vector2Int(-1, 0), // West direction, returns (-1, 0)
                WindroseDirection.NorthWest => new Vector2Int(-1, 1), // North-West direction, returns (-1, 1)
                _ => new Vector2Int(0, -1), // Default case, returns (0, -1)
            };
        }

        /// <summary>
        /// Returns the sign of each axis of the movement input.
        /// </summary>
        /// <param name="movementInput">The movement input vector.</param>
        /// <returns>A Vector2Int representing the sign of each component of the movement input.</returns>
        public static Vector2Int DirectionSign(Vector2 movementInput)
        {
            return new Vector2Int()
            {
                // Calculate the sign of the x component of the movement input
                x = movementInput.x > 0 ? 1 : movementInput.x < 0 ? -1 : 0,

                // Calculate the sign of the y component of the movement input
                y = movementInput.y > 0 ? 1 : movementInput.y < 0 ? -1 : 0
            };
        }

        #endregion

        #region Fields

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        protected bool _isLoopable = false;

        [SerializeField, HideInInspector]
        protected WindroseCycles _cycles = new();

        #endregion

        #region Getters

        /// <summary>
        /// If the animation is loopable.
        /// </summary>
        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }

        public WindroseCycles Cycles => _cycles;

        /// <summary>
        /// The type of the performer.
        /// </summary>
        public override Type PerformerType => typeof(WindroseAnimator);

        /// <summary>
        /// The type of the animation.
        /// </summary>
        public override AnimationType AnimationType => AnimationType.Windrose;

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the maximum size of all cycles.
        /// </summary>
        /// <returns>The maximum size of all cycles.</returns>
        public override int CalculateFramesCount()
        {
            return _cycles.Values.Max(cycle => cycle.Size);
        }

        #endregion

        #region Cycles

        /// <summary>
        /// Find or create a SpriteAnimationCycle for the specified direction.
        /// </summary>
        /// <param name="direction">The direction to find or create the cycle for.</param>
        /// <returns>The found or created SpriteAnimationCycle.</returns>
        public Cycle FindOrCreateCycle(WindroseDirection direction)
        {
            // Check if the cycle for the specified direction already exists
            if (!_cycles.ContainsKey(direction))
            {
                // Create a new SpriteAnimationCycle
                Cycle newCycle = new(this)
                {
                    Identifiable = false
                };

                // Add the new cycle to the dictionary
                _cycles.Add(direction, newCycle);
            }

            // Return the cycle for the specified direction
            return _cycles[direction];
        }

        /// <summary>
        /// Tries to get the sprite animation cycle associated with the given windrose direction.
        /// </summary>
        /// <param name="direction">The windrose direction.</param>
        /// <param name="cycle">The sprite animation cycle associated with the direction, if found.</param>
        /// <returns>True if the sprite animation cycle was found, false otherwise.</returns>
        public bool TryGetCycle(WindroseDirection direction, out Cycle cycle)
        {
            return _cycles.TryGetValue(direction, out cycle);
        }

        #endregion

        #region Templating

        /// <summary>
        /// Creates a new <see cref="SpriteAnimationWindrose"/> using the provided cycles as a template.
        /// </summary>
        /// <param name="cycles">A dictionary of WindroseDirection and lists of Sprites representing the cycles.</param>
        /// <returns>A new <see cref="SpriteAnimationWindrose"/> instance.</returns>
        public SpriteAnimationWindrose UseAsTemplate(Dictionary<WindroseDirection, List<Sprite>> cycles)
        {
            // Create a clone of this SpriteAnimationWindrose instance
            SpriteAnimationWindrose clone = Instantiate(this);

            // Iterate through each pair in the provided cycles dictionary
            foreach (var pair in cycles)
            {
                // Check if the cycle for the windrose direction exists in the original animation
                if (!_cycles.TryGetValue(pair.Key, out Cycle originalCycle))
                {
                    // Log a warning if the cycle for the windrose direction was not found
                    Logger.LogWarning($"The cycle for the windrose direction {pair.Key} was not found. The animation might not work as expected.", this);
                    continue;
                }

                // Transfer the sprites from the provided cycle to the clone cycle
                TransferCycle(pair.Key, pair.Value, originalCycle);
            }

            // Transfers sprites from the provided cycle to the clone cycle
            void TransferCycle(WindroseDirection direction, List<Sprite> sprites, Cycle originalCycle)
            {
                // Check if the number of sprites matches the template's cycle size
                if (originalCycle.Size != sprites.Count)
                {
                    // Log a warning if the number of sprites does not match the template's cycle size
                    Logger.LogWarning($"The amount of sprites ({sprites.Count}) for the windrose direction {direction} " +
                        $"does not match the template's cycle size ({originalCycle.Size}). The animation might not work as expected.", this);
                }

                // Check if the clone has the cycle for the windrose direction
                if (!clone.TryGetCycle(direction, out Cycle cloneCycle))
                {
                    // Log a warning if the cycle for the windrose direction was not found in the clone
                    Logger.LogWarning($"The cycle for the windrose direction {direction} was not found. The animation might not work as expected.", this);
                    return;
                }

                // Transfer each sprite to the corresponding frame in the clone cycle
                for (int i = 0; i < sprites.Count; i++)
                {
                    // Check if the frame at the current index exists in the clone cycle
                    if (!cloneCycle.TryGetFrame(i, out Frame frame))
                    {
                        // Log a warning if the frame at the current index was not found in the clone cycle
                        Logger.LogWarning($"The frame at index {i} for the windrose direction {direction} was not found. The animation might not work as expected.", this);
                        continue;
                    }

                    // Assign the sprite to the frame in the clone cycle
                    frame.Sprite = sprites[i];
                }
            }

            // Return the clone of the SpriteAnimationWindrose instance
            return clone;
        }

        #endregion

        #region Subclasses

        [Serializable]
        public class WindroseCycles : Dictionary<WindroseDirection, Cycle>, ISerializationCallbackReceiver
        {
            [SerializeField, HideInInspector]
            private List<WindroseDirection> _keyData = new();

            [SerializeField, HideInInspector]
            private List<Cycle> _valueData = new();

            void ISerializationCallbackReceiver.OnAfterDeserialize()
            {
                Clear();
                for (int i = 0; i < _keyData.Count && i < _valueData.Count; i++)
                {
                    this[_keyData[i]] = _valueData[i];
                }
            }

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                _keyData.Clear();
                _valueData.Clear();

                foreach (var item in this)
                {
                    _keyData.Add(item.Key);
                    _valueData.Add(item.Value);
                }
            }
        }

        #endregion
    }
}