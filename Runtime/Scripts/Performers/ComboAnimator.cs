using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SpriteAnimations
{
    /// <summary>
    /// The performer responsible for playing single animations.
    /// </summary>
    public class ComboAnimator : AnimationPerformer
    {
        #region Fields

        protected SpriteAnimationCombo _comboAnimation;

        protected int _currentCycleIndex = 0;

        protected bool _waiting = false;
        protected float _currentWaitCounter = 0;
        protected float _currentCycleWaitOverride = -1;

        protected UnityAction _onCycleEndedAction;
        protected UnityAction _onInterruptedAction;

        #endregion 

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            base.StartAnimation(animation);

            _currentAnimation = animation;
            _comboAnimation = _currentAnimation as SpriteAnimationCombo;

            if (_comboAnimation.Cycles.Count <= 0)
            {
                Logger.LogError($"{_comboAnimation.AnimationName} has no cycles to be played. Please report this.", _animator);
                return;
            }

            _currentCycle = _comboAnimation.FirstCycle;

            _currentCycleIndex = 0;
            _currentCycleElapsedTime = 0;
            _waiting = false;

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
            EvaluateWait(deltaTime);
            EvaluateNewFrame(deltaTime);
        }

        protected void EvaluateWait(float deltaTime)
        {
            if (!_isPlaying || !_waiting) return;

            float waitDuration = _currentCycleWaitOverride >= 0 ? _currentCycleWaitOverride : _comboAnimation.WaitingTime;
            _currentWaitCounter += deltaTime;

            if (_currentWaitCounter > waitDuration) // Interrupting
            {
                _onInterruptedAction?.Invoke();
                EndAnimation();
            }
        }

        protected void EvaluateNewFrame(float deltaTime)
        {
            if (!_isPlaying || _waiting) return;

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

            if (_frameIdActions.TryGetValue(evaluatedFrame.Id, out var byIDAction))
            {
                byIDAction.Invoke(_currentFrame);
            }
        }

        /// <summary>
        /// Resets the current animation cycle and starts playing the single cycle animation from the start.
        /// </summary>
        /// <returns>The updated <see cref="SingleCycleAnimator"/> instance.</returns>
        public ComboAnimator FromStart()
        {
            _currentCycle = _comboAnimation.FirstCycle;
            _currentCycleIndex = 0;
            _waiting = false;

            if (_currentCycle.Size > 0)
            {
                _animator.SpriteRenderer.sprite = _currentCycle.GetFirstFrame().Sprite;
            }

            _isPlaying = true;
            return this;
        }

        #endregion

        #region Ending

        protected void EndCycle()
        {
            if (!HasCurrentAnimation)
            {
                Logger.LogWarning($"Combo Cycle Animation cycle ended but it has no current animation. Please report this.", _animator);
                return;
            }


            if (_currentCycleIndex < _comboAnimation.Cycles.Count - 1) // Time to wait for next cycle request
            {
                // Starts waiting for the next cycle
                _currentWaitCounter = 0;
                _waiting = true;
                _onCycleEndedAction?.Invoke();
                return;
            }

            // The last cycle has been played
            EndAnimation();
            _onCycleEndedAction?.Invoke();
            _onEndAction?.Invoke();
        }

        /// <summary>
        /// Use this to stop playing the animation. Cannot use StopAnimation() because
        /// of events being cleared.
        /// </summary>
        protected void EndAnimation()
        {
            _onInterruptedAction = null;
            _onCycleEndedAction = null;

            _currentCycle = null;
            _currentFrame = null;
            _isPlaying = false;

            _currentWaitCounter = 0;
            _currentCycleWaitOverride = -1;
        }

        #endregion

        #region Cycles Handling

        public ComboAnimator Next()
        {
            if (!_waiting) return this;

            if (!_comboAnimation.TryGetCycle(_currentCycleIndex + 1, out _currentCycle))
            {
                Logger.LogError($"Could not find a next cycle for the current animation. Current index: {_currentCycleIndex} -  "
                + $"Requested index: {_currentCycleIndex + 1}", _animator);
                return this;
            }

            _currentCycleIndex++;
            _waiting = false;
            _currentCycleElapsedTime = 0;

            return this;
        }

        public ComboAnimator OverrideInputWait(float maxInputWait)
        {
            _currentCycleWaitOverride = maxInputWait;
            return this;
        }

        public ComboAnimator SetOnInterrupted(UnityAction onInterrupted)
        {
            _onInterruptedAction = onInterrupted;
            return this;
        }

        public ComboAnimator SetOnCycleEnded(UnityAction onCycleEnded)
        {
            _onCycleEndedAction = onCycleEnded;
            return this;
        }

        #endregion
    }

}