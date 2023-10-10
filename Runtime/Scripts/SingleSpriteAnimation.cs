using UnityEngine;
using System.Collections.Generic;
using System;
using SpriteAnimations.Performers;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Simple Sprite Animation", menuName = "Sprite Animations/Simple Sprite Animation")]
    [Serializable]
    public class SimpleSpriteAnimation : SpriteAnimation
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

        public bool IsLoopable => _isLoopable;
        public SpriteAnimationCycle Cycle => _cycle;

        public override List<SpriteAnimationFrame> GetAllFrames() => _cycle.Frames;
        public override Type PerformerType => typeof(SingleSpriteAnimationPerformer);
        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;

        #endregion      
    }
}