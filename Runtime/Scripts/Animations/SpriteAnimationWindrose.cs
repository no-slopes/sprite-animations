using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Windrose Sprite Animation", menuName = "Sprite Animations/Windrose Sprite Animation")]
    public class SpriteAnimationWindrose : SpriteAnimation
    {
        #region Static

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

        [SerializeField]
        protected WindroseFlipStrategy _flipStrategy = WindroseFlipStrategy.NoFlip;

        [SerializeField, HideInInspector]
        protected WindroseCycles _cycles = new();

        #endregion

        #region Getters

        /// <summary>
        /// If the animation is loopable.
        /// </summary>
        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }

        /// <summary>
        /// If this animation should flip the east cycles sprites to compose the the west cycles.
        /// </summary>
        public WindroseFlipStrategy FlipStrategy { get => _flipStrategy; set => _flipStrategy = value; }

        /// <summary>
        /// The type of the performer.
        /// </summary>
        public override Type PerformerType => typeof(WindroseAnimator);

        /// <summary>
        /// The type of the animation.
        /// </summary>
        public override SpriteAnimationType AnimationType => SpriteAnimationType.Windrose;

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
        public SpriteAnimationCycle FindOrCreateCycle(WindroseDirection direction)
        {
            // Check if the cycle for the specified direction already exists
            if (!_cycles.ContainsKey(direction))
            {
                // Create a new SpriteAnimationCycle
                SpriteAnimationCycle newCycle = new()
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
        public bool TryGetCycle(WindroseDirection direction, out SpriteAnimationCycle cycle)
        {
            return _cycles.TryGetValue(direction, out cycle);
        }

        #endregion

        #region Subclasses

        [Serializable]
        protected class WindroseCycles : Dictionary<WindroseDirection, SpriteAnimationCycle>, ISerializationCallbackReceiver
        {
            [SerializeField, HideInInspector]
            private List<WindroseDirection> _keyData = new();

            [SerializeField, HideInInspector]
            private List<SpriteAnimationCycle> _valueData = new();

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