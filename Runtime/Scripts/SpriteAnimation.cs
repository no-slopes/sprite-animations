using UnityEngine;
using System.Collections.Generic;
using System;

namespace SpriteAnimations
{
    public delegate void NameChangedEvent(string newName);
    public abstract class SpriteAnimation : ScriptableObject
    {
        #region Inspector

        /// <summary>
        /// The animation name to be used by the SpriteAnimator
        /// </summary>
        [SerializeField]
        protected string _animationName;

        /// <summary>
        /// Amount of frames per second.
        /// </summary>
        [SerializeField]
        protected int _fps = 6;

        #endregion

        #region Properties

        /// <summary>
        /// The amount of frames per second.
        /// </summary>
        public int FPS { get => _fps; set => _fps = value; }

        /// <summary>
        /// The name of the animation. Can be used to identify the animation
        /// in the SpriteAnimator.
        /// </summary>
        public string AnimationName
        {
            get => _animationName;
            set
            {
                _animationName = value;
                NameChanged?.Invoke(_animationName);
            }
        }

        #endregion

        #region Abstratctions

        public abstract List<SpriteAnimationFrame> GetAllFrames();
        public abstract Type PerformerType { get; }
        public abstract SpriteAnimationType AnimationType { get; }

        #endregion

        #region Info

        public float CalculateDuration()
        {
            List<SpriteAnimationFrame> frames = GetAllFrames();

            if (frames.Count <= 0) return 0;

            return frames.Count / (float)_fps;
        }

        #endregion

        #region Events

        public event NameChangedEvent NameChanged;

        #endregion

    }
}