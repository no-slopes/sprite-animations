using UnityEngine;
using System.Collections.Generic;
using System;
using SpriteAnimations.Performers;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Simple Sprite Animation", menuName = "Sprite Animations/Simple Sprite Animation")]
    public class SpriteAnimationSimple : SpriteAnimation
    {
        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        protected bool _isLoopable = false;

        /// <summary>
        /// The animation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected SpriteAnimationCycle _cycle = new();

        #endregion  

        #region Getters

        public bool IsLoopable { get => _isLoopable; set => _isLoopable = value; }
        public SpriteAnimationCycle Cycle => _cycle;

        public override Type PerformerType => typeof(PerformerSingle);
        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;

        #endregion

        #region Calculations

        public override int CalculateFramesCount()
        {
            return _cycle.Size;
        }

        #endregion
    }
}