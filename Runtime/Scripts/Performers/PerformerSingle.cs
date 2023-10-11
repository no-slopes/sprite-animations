using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Performers
{
    public class PerformerSingle : Performer
    {
        #region Fields

        protected Dictionary<string, UnityAction> _frameIdActions = new();

        #endregion

        #region Properties 

        protected SpriteAnimationSimple CurrentSimpleAnimation => _currentAnimation as SpriteAnimationSimple;

        #endregion    

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Simple;

        #endregion   

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            base.StartAnimation(animation);

            _currentCycleElapsedTime = 0;
            _currentAnimation = animation;

            _currentCycle = CurrentSimpleAnimation.Cycle;
            _currentCycleDuration = _currentCycle.CalculateDuration(_currentAnimation.FPS);
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void StopAnimation()
        {
            base.StopAnimation();
            _frameIdActions.Clear();
            EndAnimation();
        }

        /// <summary>
        /// Evaluates what frame should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public override void Tick(float deltaTime)
        {
            if (!HasCurrentAnimation) return;

            _currentCycleElapsedTime += deltaTime;
            EvaluateEnd();

            if (!HasCurrentAnimation) return;

            int frameIndex = CalculateFrameIndex(_currentCycleElapsedTime, _currentCycle.Size, _currentCycleDuration);
            SpriteAnimationFrame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(frameIndex);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;

            // From here it means the new frame will be displayed

            _currentFrame = evaluatedFrame;
            _animator.SpriteRenderer.sprite = _currentFrame.Sprite;

            // Calling frame actions
            if (_frameIndexActions.TryGetValue(frameIndex, out var byIndexAction))
            {
                byIndexAction.Invoke();
            }

            if (string.IsNullOrEmpty(evaluatedFrame.Id)) return;

            if (_frameIdActions.TryGetValue(evaluatedFrame.Id, out var byNameAction))
            {
                byNameAction.Invoke();
            }
        }

        #endregion

        #region Simple Sprite Animation Logic


        protected void EvaluateEnd()
        {
            if (_currentCycleElapsedTime >= _currentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
            }
        }

        #endregion

        #region Ending

        /// <summary>
        /// Ends the current cycle. In case the animation is loopable, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        public void EndCycle()
        {
            if (HasCurrentAnimation && CurrentSimpleAnimation.IsLoopable)
            {
                ResetCycle();
            }
            else if (HasCurrentAnimation)
            {
                EndAnimation();
            }
        }

        /// <summary>
        /// Resets the cycle.
        /// </summary>
        public void ResetCycle()
        {
            _currentCycleElapsedTime = 0f;
            _currentFrame = null;
            _onEndAction?.Invoke();
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            _currentAnimation = null;
            _currentCycle = null;
            _currentFrame = null;
            _onEndAction?.Invoke();
        }

        #endregion

        #region Actions

        /// <summary>
        /// Sets an action to be invoked when a specific frame ID is played.
        /// </summary>
        /// <param name="id">The frame ID.</param>
        /// <param name="action">The UnityAction to be invoked.</param>
        /// <returns>The updated PerformerSingle instance.</returns>
        public Performer OnFrameId(string id, UnityAction action)
        {
            _frameIdActions[id] = action;
            return this;
        }

        #endregion
    }

}