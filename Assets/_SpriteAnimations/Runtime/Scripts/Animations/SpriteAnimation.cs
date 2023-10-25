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

        public abstract Type PerformerType { get; }
        public abstract AnimationType AnimationType { get; }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the duration of the animation in seconds.
        /// </summary>
        /// <returns>The duration of the animation in seconds.</returns>
        public float CalculateDuration()
        {
            // Calculate the number of frames in the animation
            int frameCount = CalculateFramesCount();

            // If there are no frames, return 0 duration
            if (frameCount <= 0) return 0;

            // Calculate and return the duration in seconds
            return frameCount / (float)_fps;
        }

        /// <summary>
        /// Calculates the total amount of frames in the animation.
        /// </summary>
        /// <returns>The count of frames.</returns>
        public abstract int CalculateFramesCount();

        #endregion

        #region Events

        public event NameChangedEvent NameChanged;

        #endregion

    }
}