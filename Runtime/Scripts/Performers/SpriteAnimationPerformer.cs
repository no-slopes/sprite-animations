using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Performers
{
    public abstract class SpriteAnimationPerformer
    {

        #region Fields

        protected SpriteAnimation _currentAnimation;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected SpriteAnimationCycle _currentCycle = new();

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected SpriteAnimationCycleType _currentCycleType = SpriteAnimationCycleType.Core;

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float _currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected SpriteAnimationFrame _currentFrame;

        /// <summary>
        /// If the animation has reached its end and should stop playing
        /// </summary>
        protected bool _animationEnded = false;

        protected Dictionary<int, UnityAction> _frameIndexActions = new();
        protected Dictionary<string, UnityAction> _frameNameActions = new();
        protected Dictionary<SpriteAnimationCycleType, UnityAction> _cycleTypeEndActions = new();
        protected UnityAction _onEndAction;

        #endregion       

        #region Properties  

        /// <summary>
        /// The duration in seconds a frame should display while in that animation
        /// </summary>
        protected float FrameDuration => _currentAnimation != null ? 1f / _currentAnimation.FPS : 0f;

        /// <summary>
        /// The duration of the current animation's cycle in seconds
        /// </summary>
        protected float CurrentCycleDuration => FrameDuration * _currentCycle.FrameCount;

        /// <summary>
        /// The index of the current frame in the current cycle.
        /// This is calculated based on the total amount of frames the current cycle has and the current elapsed time for 
        /// the that cycle.
        /// </summary>
        protected int CurrentFrameIndex => Mathf.FloorToInt(_currentCycleElapsedTime * _currentCycle.FrameCount / CurrentCycleDuration);

        /// <summary>
        /// If the animation has frames to be played
        /// </summary>
        protected bool HasCurrentFrames => _currentCycle != null && _currentCycle.FrameCount > 0;

        #endregion

        #region Getters 

        public abstract SpriteAnimationType AnimationType { get; }
        public SpriteAnimationCycle CurrentCycle => _currentCycle;

        #endregion

        #region Logic       

        public virtual void StartAnimation(SpriteAnimation animation)
        {

        }

        public virtual void StopAnimation()
        {
            _frameIndexActions.Clear();
            _frameNameActions.Clear();
            _cycleTypeEndActions.Clear();
            _onEndAction = null;
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

        #region Abstract Methods

        public abstract SpriteAnimationFrame EvaluateFrame(float deltaTime);
        protected bool HasCycle { get; }

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
        /// <param name="frameName">The name of the frame.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The updated SpriteAnimationPerformer object.</returns>
        public SpriteAnimationPerformer OnFrame(string frameName, UnityAction action)
        {
            _frameNameActions[frameName] = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be performed when a sprite animation cycle ends.
        /// This overrides previous actions defined for that frame cycle ending.
        /// </summary>
        /// <param name="cycleType">The type of the animation cycle.</param>
        /// <param name="action">The action to perform.</param>
        /// <returns>The SpriteAnimationPerformer instance.</returns>
        public SpriteAnimationPerformer OnCycleEnd(SpriteAnimationCycleType cycleType, UnityAction action)
        {
            _cycleTypeEndActions[cycleType] = action;
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

    }

}
