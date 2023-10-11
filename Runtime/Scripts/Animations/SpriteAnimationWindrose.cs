using UnityEngine;
using System.Collections.Generic;
using SpriteAnimations.Performers;
using System;
using System.Linq;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Windrose Sprite Animation", menuName = "Sprite Animations/Windrose Sprite Animation")]
    public class SpriteAnimationWindrose : SpriteAnimation
    {
        #region Static

        public static WindroseDirection DirectionFromInput(Vector2Int signedInput)
        {
            if (signedInput.x == 0 && signedInput.y > 0) return WindroseDirection.North;
            if (signedInput.x > 0 && signedInput.y > 0) return WindroseDirection.NorthEast;
            if (signedInput.x > 0 && signedInput.y == 0) return WindroseDirection.East;
            if (signedInput.x > 0 && signedInput.y < 0) return WindroseDirection.SouthEast;
            if (signedInput.x == 0 && signedInput.y < 0) return WindroseDirection.South;
            if (signedInput.x < 0 && signedInput.y < 0) return WindroseDirection.SouthWest;
            if (signedInput.x < 0 && signedInput.y == 0) return WindroseDirection.West;
            if (signedInput.x < 0 && signedInput.y > 0) return WindroseDirection.NorthWest;

            return WindroseDirection.South;
        }

        public static Vector2Int InputFromDirection(WindroseDirection direction)
        {
            return direction switch
            {
                WindroseDirection.North => new Vector2Int(0, 1),
                WindroseDirection.NorthEast => new Vector2Int(1, 1),
                WindroseDirection.East => new Vector2Int(1, 0),
                WindroseDirection.SouthEast => new Vector2Int(1, -1),
                WindroseDirection.South => new Vector2Int(0, -1),
                WindroseDirection.SouthWest => new Vector2Int(-1, -1),
                WindroseDirection.West => new Vector2Int(-1, 0),
                WindroseDirection.NorthWest => new Vector2Int(-1, 1),
                _ => new Vector2Int(0, -1),
            };
        }

        public static Vector2Int DirectionSign(Vector2 movementInput)
        {
            return new Vector2Int()
            {
                x = movementInput.x > 0 ? 1 : movementInput.x < 0 ? -1 : 0,
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

        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }
        public override Type PerformerType => typeof(WindroseAnimator);
        public override SpriteAnimationType AnimationType => SpriteAnimationType.Windrose;

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _cycles.Values.Max(cycle => cycle.Size);
        }

        #endregion

        #region Cycles

        public SpriteAnimationCycle FindOrCreateCycle(WindroseDirection direction)
        {
            if (!_cycles.ContainsKey(direction))
            {
                SpriteAnimationCycle newCycle = new()
                {
                    Identifiable = false
                };

                _cycles.Add(direction, newCycle);
            }
            return _cycles[direction];
        }

        public bool TryGetCycle(WindroseDirection direction, out SpriteAnimationCycle cycle)
        {
            return _cycles.TryGetValue(direction, out cycle);
        }

        #endregion

        #region Enums

        public enum WindroseDirection
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
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

        [Serializable]
        protected class FrameIds : Dictionary<WindroseDirection, SpriteAnimationCycle>, ISerializationCallbackReceiver
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