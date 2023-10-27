using System.Linq;

namespace SpriteAnimations
{
    /// <summary>
    /// The performer responsible for playing single animations.
    /// </summary>
    public class ComboAnimator : AnimationPerformer
    {
        #region Fields

        protected SpriteAnimationSingleCycle _singleAnimation;

        #endregion 

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            base.StartAnimation(animation);

            _currentAnimation = animation;
            _singleAnimation = _currentAnimation as SpriteAnimationSingleCycle;

            _currentCycle = _singleAnimation.Cycle;

            _currentCycleElapsedTime = 0;

            if (_currentCycle.Size > 0)
            {
                _animator.SpriteRenderer.sprite = _currentCycle.GetFirstFrame().Sprite;
            }
            else
            {
                Logger.LogWarning($"{animation.AnimationName} has no frames to be played.", _animator);
            }

            _isPlaying = true;
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
            if (!_isPlaying) return;

            _currentCycleElapsedTime += deltaTime;

            if (_currentCycleElapsedTime >= _currentCycle.CalculateDuration()) // means cycle passed last frame
            {
                EndCycle();
                return;
            }

            var (index, evaluatedFrame) = _currentCycle.EvaluateIndexAndFrame(_currentCycleElapsedTime);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;

            // From here it means the new frame will be displayed

            _currentFrame = evaluatedFrame;
            _animator.SpriteRenderer.sprite = _currentFrame.Sprite;

            InvokeFramePlayed(index, _currentFrame);

            // Calling frame actions
            if (_frameIndexActions.TryGetValue(index, out var byIndexAction))
            {
                byIndexAction.Invoke(_currentFrame);
            }

            if (string.IsNullOrEmpty(evaluatedFrame.Id)) return;

            if (_frameIdActions.TryGetValue(evaluatedFrame.Id, out var byNameAction))
            {
                byNameAction.Invoke(_currentFrame);
            }
        }

        /// <summary>
        /// Resets the current animation cycle and starts playing the single cycle animation from the start.
        /// </summary>
        /// <returns>The updated <see cref="SingleAnimator"/> instance.</returns>
        public ComboAnimator FromStart()
        {
            _currentCycle = _singleAnimation.Cycle;

            ResetCycle();

            if (_currentCycle.Size > 0)
            {
                _animator.SpriteRenderer.sprite = _currentCycle.Frames.First().Sprite;
            }

            _isPlaying = true;
            return this;
        }

        #endregion

        #region Ending

        /// <summary>
        /// Ends the current cycle. In case the animation is loopable, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        protected void EndCycle()
        {
            if (!HasCurrentAnimation)
            {
                Logger.LogWarning($"Single Cycle Animation cycle ended but it has no current animation. Please report this.", _animator);
                return;
            }

            if (_singleAnimation.IsLoopable)
            {
                ResetCycle();
            }
            else
            {
                EndAnimation();
            }

            _onEndAction?.Invoke();
        }

        /// <summary>
        /// Resets the cycle.
        /// </summary>
        protected void ResetCycle()
        {
            _currentCycleElapsedTime = 0f;
            _currentFrame = null;
        }

        /// <summary>
        /// Use this to stop playing the animation. Cannot use StopAnimation() because
        /// of events being cleared.
        /// </summary>
        protected void EndAnimation()
        {
            _currentCycle = null;
            _currentFrame = null;
            _isPlaying = false;
        }

        #endregion
    }

}