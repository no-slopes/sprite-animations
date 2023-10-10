using System.Linq;
using UnityEngine;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Performers
{
    public class SingleSpriteAnimationPerformer : SpriteAnimationPerformer
    {
        #region Properties 

        protected bool HasCurrentAnimation => _currentAnimation != null;
        protected SimpleSpriteAnimation CurrentSimpleAnimation => _currentAnimation as SimpleSpriteAnimation;

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
            HandleCycles();

            if (!HasCurrentAnimation) return;

            int frameIndex = CalculateFrameIndex(_currentCycleElapsedTime, _currentCycle.FrameCount, _currentCycleDuration);
            SpriteAnimationFrame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(frameIndex);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;
            // From here it means the new frame will be displayed

            _currentFrame = evaluatedFrame;
            _spriteRenderer.sprite = _currentFrame.Sprite;

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


        /// <summary>
        /// Handles the animation cycles. 
        /// It evaluates if the current cycle is over and if so, it changes the cycle.
        /// This also evaluate what is the current frame of the current cycle.
        /// </summary>
        protected void HandleCycles()
        {
            if (_currentCycleElapsedTime >= _currentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
            }
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is a loop, it restarts the cycle.
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
            _onEndAction?.Invoke();
        }

        /// <summary>
        /// Resets the cycle.
        /// </summary>
        public void ResetCycle()
        {
            _currentCycleElapsedTime = 0f;
            _currentFrame = null;
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            _currentAnimation = null;
            _currentCycle = null;
            _currentFrame = null;
        }

    }

}