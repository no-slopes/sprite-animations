using System.Linq;
using UnityEngine;
using static SpriteAnimations.SpriteAnimation;
using static SpriteAnimations.SpriteAnimationWindrose;

namespace SpriteAnimations.Performers
{
    public class SpriteAnimationPerformerWindrose : SpriteAnimationPerformer
    {
        #region Fields

        private int _directionLessTicks = 0;
        private bool _warnedAboutDirectionLess = false;

        #endregion

        #region Properties 

        protected SpriteAnimationWindrose CurrentWindroseAnimation => _currentAnimation as SpriteAnimationWindrose;

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
            _directionLessTicks = 0;
            _warnedAboutDirectionLess = false;
            _currentCycleElapsedTime = 0;
            _currentAnimation = animation;
        }

        /// <summary>
        /// Sets the direction of the animation.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <returns>The updated SpriteAnimationPerformerWindrose instance.</returns>
        public SpriteAnimationPerformerWindrose SetDirection(WindroseDirection direction)
        {
            // Try to get the cycle for the specified direction
            if (!CurrentWindroseAnimation.TryGetCycle(WindroseDirection.East, out _currentCycle))
            {
                // Log an error if the cycle does not exist
                Logger.LogError($"Animation '{_currentAnimation.AnimationName}' does not have a cycle for direction {direction}.", _animator);
                EndAnimation();
                return this;
            }

            // Calculate the duration for the current cycle
            _currentCycleDuration = _currentCycle.CalculateDuration(_currentAnimation.FPS);
            return this;
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

            EvaluateDirectionLessCycle();

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

        private void EvaluateDirectionLessCycle()
        {
            if (HasCurrentCycle) return;
            _directionLessTicks++;

            if (!_warnedAboutDirectionLess && _directionLessTicks > 10)
            {
                Logger.LogWarning($"Seems like the animation {_currentAnimation.AnimationName} does not "
                + "have a direction set. Have you forgotten to call SetDirection?", _animator);

                _warnedAboutDirectionLess = true;
            }
        }

        #endregion

        #region End

        protected void EvaluateEnd()
        {
            if (_currentCycleElapsedTime >= _currentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
            }
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is loopable, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        public void EndCycle()
        {
            if (HasCurrentAnimation && CurrentWindroseAnimation.IsLoopable)
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
    }

}