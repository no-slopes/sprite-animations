using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpriteAnimations
{
    public delegate void FramePlayedEvent(int index, Frame frame);
    /// <summary>
    /// The base class for all animation performers.
    /// </summary>
    public abstract class AnimationPerformer
    {
        #region Fields

        protected SpriteAnimator _animator;
        protected SpriteAnimation _currentAnimation;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected Cycle _currentCycle;

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float _currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected Frame _currentFrame;

        protected float _currentCycleDuration = 0.0f;

        protected Dictionary<int, UnityAction<Frame>> _frameIndexActions = new();
        protected Dictionary<string, UnityAction<Frame>> _frameIdActions = new();
        protected UnityAction _onEndAction;

        #endregion       

        #region Properties  

        public SpriteAnimator Animator { get => _animator; set => _animator = value; }
        protected bool HasCurrentAnimation => _currentAnimation != null;
        protected bool HasCurrentCycle => _currentCycle != null;

        #endregion

        #region Getters 

        /// <summary>
        /// Current cycle being played
        /// </summary>
        public Cycle CurrentCycle => _currentCycle;

        /// <summary>
        /// The current frame displayed in the animator's SpriteRenderer
        /// </summary>
        public Frame CurrentFrame => _currentFrame;

        #endregion

        #region Events

        /// <summary>
        /// Event invoked when this performer changed the current frame displayed at the animator's SpriteRenderer
        /// </summary>
        public event FramePlayedEvent FramePlayed;

        protected void InvokeFramePlayed(int index, Frame frame)
        {
            FramePlayed?.Invoke(index, frame);
        }

        protected void ClearFramePlayedListeners()
        {
            FramePlayed = null;
        }

        #endregion

        #region Logic       

        /// <summary>
        /// Starts the given sprite animation.
        /// </summary>
        /// <param name="animation">The sprite animation to start.</param>
        public virtual void StartAnimation(SpriteAnimation animation)
        {
            // Set the flipX property of the sprite renderer to false
            _animator.SpriteRenderer.flipX = false;
        }

        /// <summary>
        /// Stops the animation and clears all the frame index actions, frame ID actions, and the on end action.
        /// </summary>
        public virtual void StopAnimation()
        {
            // Clear the frame index actions
            _frameIndexActions.Clear();

            // Clear the frame ID actions
            _frameIdActions.Clear();

            // Set the on end action to null
            _onEndAction = null;

            ClearFramePlayedListeners();
        }

        /// <summary>
        /// The method called every time the animator should evaluate if the frame must be changed.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last tick.</param>
        public abstract void Tick(float deltaTime);

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
        /// Sets an action to be performed when a specific frame index is played.
        /// </summary>
        /// <param name="frameIndex">The index of the frame.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The SpriteAnimationPerformer instance.</returns>
        public AnimationPerformer SetOnFrame(int frameIndex, UnityAction<Frame> action)
        {
            _frameIndexActions[frameIndex] = action;
            return this;
        }

        /// <summary>
        /// Sets an action to be invoked when a specific frame ID is played.
        /// </summary>
        /// <param name="id">The frame ID.</param>
        /// <param name="action">The UnityAction to be invoked.</param>
        /// <returns>The updated PerformerSingle instance.</returns>
        public AnimationPerformer SetOnFrame(string id, UnityAction<Frame> action)
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
