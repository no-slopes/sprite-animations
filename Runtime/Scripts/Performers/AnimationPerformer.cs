using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpriteAnimations.Performers
{
    public abstract class AnimationPerformer
    {

        #region Fields

        protected SpriteAnimator _animator;
        protected SpriteAnimation _currentAnimation;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected SpriteAnimationCycle _currentCycle;

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float _currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected SpriteAnimationFrame _currentFrame;

        protected float _currentCycleDuration = 0.0f;

        protected Dictionary<int, UnityAction> _frameIndexActions = new();
        protected UnityAction _onEndAction;

        #endregion       

        #region Properties  

        public SpriteAnimator Animator { get => _animator; set => _animator = value; }
        protected bool HasCurrentAnimation => _currentAnimation != null;
        protected bool HasCurrentCycle => _currentCycle != null;

        #endregion

        #region Getters 

        public abstract SpriteAnimationType AnimationType { get; }
        public SpriteAnimationCycle CurrentCycle => _currentCycle;
        public SpriteAnimationFrame CurrentFrame => _currentFrame;

        #endregion

        #region Logic       

        public virtual void StartAnimation(SpriteAnimation animation)
        {

        }

        public virtual void StopAnimation()
        {
            _frameIndexActions.Clear();
            _onEndAction = null;
        }

        public virtual void Tick(float deltaTime)
        {

        }

        /// <summary>
        /// Validates if the animation should be played.
        /// </summary>
        /// <returns> true if animation can be played </returns>
        protected bool ValidateAnimation(SpriteAnimation animation)
        {
            if (animation == null)
            {
                Logger.LogError($"Trying to set null as current animation.", _animator);
                return false;
            }

            int framesCount = animation.CalculateFramesCount();

            if (framesCount > 0) return true;

            Logger.LogError($"Unable play the animation {animation.AnimationName} due to the lack of frames. "
            + $"Did you set animation frames?", _animator);

            return false;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Sets an action to be performed on a specific frame index.
        /// This overrides previous actions defined for that frame index.
        /// </summary>
        /// <param name="frameIndex">The index of the frame.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The SpriteAnimationPerformer instance.</returns>
        public AnimationPerformer OnFrame(int frameIndex, UnityAction action)
        {
            _frameIndexActions[frameIndex] = action;
            return this;
        }

        /// <summary>
        /// Sets the action to be invoked when the animation ends.
        /// This overrides previous actions defined for that animation end.
        /// </summary>
        /// <param name="action">The UnityAction to be invoked.</param>
        /// <returns>The updated SpriteAnimationPerformer instance.</returns>
        public AnimationPerformer OnEnd(UnityAction action)
        {
            _onEndAction = action;
            return this;
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the frame index based on the elapsed time, amount of frames, and duration.
        /// </summary>
        /// <param name="elapsedTime">The time that has elapsed since the animation started.</param>
        /// <param name="amountOfFrames">The total number of frames in the animation.</param>
        /// <param name="duration">The duration of the animation in seconds.</param>
        /// <returns>The index of the current frame to display.</returns>
        protected int CalculateFrameIndex(float elapsedTime, int amountOfFrames, float duration)
        {
            return Mathf.FloorToInt(elapsedTime * amountOfFrames / duration);
        }

        #endregion

    }

}
