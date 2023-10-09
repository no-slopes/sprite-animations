using System.Linq;
using UnityEngine;
using static SpriteAnimations.SpriteAnimation;

namespace SpriteAnimations.Performers
{
    public class SingleSpriteAnimationPerformer : SpriteAnimationPerformer
    {
        #region Properties 

        protected bool HasCurrentAnimation => _currentAnimation != null;
        protected SingleSpriteAnimation CurrentSimpleAnimation => _currentAnimation as SingleSpriteAnimation;

        #endregion    

        #region Getters

        public override SpriteAnimationType AnimationType => SpriteAnimationType.Single;

        #endregion   

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            base.StartAnimation(animation);

            ValidateAnimation(animation);

            _currentAnimation = animation;

            ResetCycle();

            _animationEnded = false;
            _currentCycle = CurrentSimpleAnimation.Cycle;
            _currentCycleType = SpriteAnimationCycleType.Core;
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void StopAnimation()
        {
            EndAnimation();
            _currentAnimation = null;
        }

        /// <summary>
        /// Evaluates what frame should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public override SpriteAnimationFrame EvaluateFrame(float deltaTime)
        {
            if (_animationEnded) return _currentFrame;

            _currentCycleElapsedTime += deltaTime;

            HandleCycles();

            _currentFrame = EvaluateCycleFrame();


            return _currentFrame;
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
            if (_currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
            }
        }

        protected SpriteAnimationFrame EvaluateCycleFrame()
        {
            int frameIndex = CurrentFrameIndex;
            SpriteAnimationFrame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(frameIndex);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return _currentFrame;

            // From here it means the new frame will be displayed

            if (_frameIndexActions.TryGetValue(frameIndex, out var byIndexAction))
            {
                byIndexAction.Invoke();
            }

            if (_frameNameActions.TryGetValue(evaluatedFrame.Id, out var byNameAction))
            {
                byNameAction.Invoke();
            }

            return evaluatedFrame;
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is a loop, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        public void EndCycle()
        {
            if (_cycleTypeEndActions.TryGetValue(_currentCycleType, out var byTypeAction))
            {
                byTypeAction.Invoke();
            }

            if (HasCurrentAnimation && CurrentSimpleAnimation.IsLoopable)
            {
                ResetCycle();
            }
            else
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
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            _animationEnded = true;
            _onEndAction?.Invoke();
        }

    }

}