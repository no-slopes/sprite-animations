using UnityEngine;
using System;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Single Cycle Animation", menuName = "Sprite Animations/Single Cycle Animation")]
    public class SpriteAnimationSingleCycle : SpriteAnimation
    {
        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        protected bool _isLoopable = false;

        /// <summary>
        /// The animation Cycle
        /// </summary>
        [SerializeField]
        protected Cycle _cycle;

        #endregion  

        #region Getters

        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }
        public Cycle Cycle => _cycle;

        public override Type PerformerType => typeof(SingleCycleAnimator);
        public override AnimationType AnimationType => AnimationType.SingleCycle;

        #endregion

        #region Cycle

        /// <summary>
        /// This must be executed upon the asset creation
        /// </summary>
        public void GenerateCycle()
        {
            _cycle = new Cycle(this);
        }

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _cycle.Size;
        }

        #endregion
    }
}