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
        #region Fields

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        protected bool _isLoopable = false;

        [SerializeField, HideInInspector]
        protected WindroseCycles _cycles = new();

        [SerializeField]
        protected int _size;

        #endregion

        #region Getters

        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }
        public int Size
        {
            get => _size;
            set
            {
                _size = value;
                AdjustCyclesToSize(_size);
            }
        }
        public override Type PerformerType => typeof(PerformerWindrose);
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
                SpriteAnimationCycle newCycle = new();
                newCycle.AdjustToSize(_size);
                _cycles.Add(direction, newCycle);
            }
            return _cycles[direction];
        }

        public bool TryGetCycle(WindroseDirection direction, out SpriteAnimationCycle cycle)
        {
            return _cycles.TryGetValue(direction, out cycle);
        }

        private void AdjustCyclesToSize(int size)
        {
            foreach (var item in _cycles)
            {
                item.Value.AdjustToSize(size);
            }
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

        #endregion
    }
}