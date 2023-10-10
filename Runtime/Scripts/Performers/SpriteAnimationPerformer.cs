using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Performers
{
    public abstract class SpriteAnimationPerformer
    {

        #region Fields

        protected SpriteRenderer _spriteRenderer;

        protected SpriteAnimation _currentAnimation;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected SpriteAnimationCycle _currentCycle = new();

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float _currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected SpriteAnimationFrame _currentFrame;

        protected float _currentCycleDuration = 0.0f;

        /// <summary>
        /// If the animation has reached its end and should stop playing
        /// </summary>
        protected bool _animationEnded = false;

        protected Dictionary<int, UnityAction> _frameIndexActions = new();
        protected Dictionary<string, UnityAction> _frameIdActions = new();
        protected UnityAction _onEndAction;

        #endregion       

        #region Properties  

        public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }

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
            _frameIdActions.Clear();
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
                Debug.LogError($"Sprite Animations - Trying to set null as current animation.");
                return false;
            }

            var allFrames = animation.GetAllFrames();

            if (allFrames.Count > 0) return true;

            Debug.LogError($"Sprite animations - Could not evaluate the animation {nameof(animation)} to be played. Did you set animation frames?");

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
        public SpriteAnimationPerformer OnFrame(int frameIndex, UnityAction action)
        {
            _frameIndexActions[frameIndex] = action;
            return this;
        }

        /// <summary>
        /// Adds an action to be performed on a specific frame of the sprite animation.
        /// This overrides previous actions defined for that frame name.
        /// </summary>
        /// <param name="id">The id of the frame.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The updated SpriteAnimationPerformer object.</returns>
        public SpriteAnimationPerformer OnFrame(string id, UnityAction action)
        {
            _frameIdActions[id] = action;
            return this;
        }

        /// <summary>
        /// Sets the action to be invoked when the animation ends.
        /// This overrides previous actions defined for that animation end.
        /// </summary>
        /// <param name="action">The UnityAction to be invoked.</param>
        /// <returns>The updated SpriteAnimationPerformer instance.</returns>
        public SpriteAnimationPerformer OnEnd(UnityAction action)
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
