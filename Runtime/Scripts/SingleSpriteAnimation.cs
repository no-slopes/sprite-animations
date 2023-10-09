using UnityEngine;
using System.Collections.Generic;
using System;
using SpriteAnimations.Performers;

namespace SpriteAnimations
{
    [CreateAssetMenu(fileName = "Single Sprite Animation", menuName = "Handy Tools/Handy Sprite Animator/Single Sprite Animation")]
    [Serializable]
    public class SingleSpriteAnimation : SpriteAnimation
    {
        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        [Space]
        protected bool _loopable = false;

        /// <summary>
        /// The animation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected SpriteAnimationCycle _cycle = new();

        #endregion  

        #region Getters

        public bool IsLoopable => _loopable;
        public SpriteAnimationCycle Cycle => _cycle;

        public override List<SpriteAnimationFrame> GetAllFrames() => _cycle.Frames;
        public override Type PerformerType => typeof(SingleSpriteAnimationPerformer);
        public override SpriteAnimationType AnimationType => SpriteAnimationType.Single;

        #endregion      
    }
}