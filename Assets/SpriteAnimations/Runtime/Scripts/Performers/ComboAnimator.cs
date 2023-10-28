using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SpriteAnimations
{
    /// <summary>
    /// The performer responsible for playing combo animations.
    /// </summary>
    public class ComboAnimator : AnimationPerformer
    {
        #region Fields

        /// <summary>
        /// The current <see cref="SpriteAnimationCombo"/> associated to the <see cref="ComboAnimator"/>
        /// </summary>
        protected SpriteAnimationCombo _comboAnimation;

        /// <summary>
        /// The currently being played cycle's index
        /// </summary>
        protected int _currentCycleIndex = 0;

        /// <summary>
        /// Is the combo animator currently waiting for the <see cref="Next"/> method to be called?
        /// </summary>
        protected bool _waiting = false;

        /// <summary>
        /// The current counter for undertanding whe the animation should be interrupted
        /// </summary>
        protected float _currentWaitCounter = 0;

        /// <summary>
        /// The amount of time to wait for the next cycle. If higher than 0, it will
        /// be used instead of the <see cref="SpriteAnimationCombo.WaitingTime"/>
        /// </summary>
        protected float _currentCycleWaitOverride = -1;

        /// <summary>
        /// The action to be invoked when the current cycle ends
        /// </summary>
        protected UnityAction<int> _onCycleEndedAction;

        /// <summary>
        /// The action to be invoked when the animation is interrupted
        /// </summary>
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
            _onInterruptedAction = null;
            _onCycleEndedAction = null;
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

        /// <summary>
        /// Evaluates the wait time for the animation.
        /// </summary>
        /// <param name="deltaTime">The time since the last frame.</param>
        protected void EvaluateWait(float deltaTime)
        {
            // Check if animation is playing and waiting
            if (!_isPlaying || !_waiting) return;

            // Get the wait duration
            float waitDuration = _currentCycleWaitOverride >= 0 ? _currentCycleWaitOverride : _comboAnimation.WaitingTime;

            // Increment the wait counter
            _currentWaitCounter += deltaTime;

            // Check if the wait counter exceeds the wait duration
            if (_currentWaitCounter > waitDuration) // Interrupting
            {
                // Invoke the interrupted action delegate
                _onInterruptedAction?.Invoke();

                // End the animation
                EndAnimation();
            }
        }

        /// <summary>
        /// Evaluates a new frame based on the given deltaTime.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        protected void EvaluateNewFrame(float deltaTime)
        {
            // Check if the animation is not playing or waiting
            if (!_isPlaying || _waiting) return;

            // Increment the elapsed time of the current cycle
            _currentCycleElapsedTime += deltaTime;

            // Check if the current cycle has passed the last frame
            if (_currentCycleElapsedTime >= _currentCycle.CalculateDuration())
            {
                // End the current cycle
                EndCycle();
                return;
            }

            // Evaluate the index and frame for the current elapsed time
            var (index, evaluatedFrame) = _currentCycle.EvaluateIndexAndFrame(_currentCycleElapsedTime);

            // Check if the evaluated frame is null or the same as the current frame
            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;

            // From here it means the new frame will be displayed

            // Set the current frame to the evaluated frame
            _currentFrame = evaluatedFrame;

            // Update the sprite renderer with the new frame's sprite
            _animator.SpriteRenderer.sprite = _currentFrame.Sprite;

            // Invoke the frame played event
            InvokeFramePlayed(index, _currentFrame);

            // Calling frame actions based on the frame index
            if (_frameIndexActions.TryGetValue(index, out var byIndexAction))
            {
                byIndexAction.Invoke(_currentFrame);
            }

            // Check if the evaluated frame has a non-empty ID
            if (!string.IsNullOrEmpty(evaluatedFrame.Id))
            {
                // Calling frame actions based on the frame ID
                if (_frameIdActions.TryGetValue(evaluatedFrame.Id, out var byIDAction))
                {
                    byIDAction.Invoke(_currentFrame);
                }
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

        /// <summary>
        /// Ends the current cycle of the animation.
        /// </summary>
        protected void EndCycle()
        {
            if (!HasCurrentAnimation)
            {
                // Logs a warning if the animation has no current animation
                Logger.LogWarning($"Combo Cycle Animation cycle ended but it has no current animation. Please report this.", _animator);
                return;
            }

            if (_currentCycleIndex < _comboAnimation.Cycles.Count - 1)
            {
                // Time to wait for the next cycle request

                // Starts waiting for the next cycle
                _currentWaitCounter = 0;
                _waiting = true;

                // Invokes the onCycleEndedAction with the current cycle index
                _onCycleEndedAction?.Invoke(_currentCycleIndex);
                return;
            }

            // The last cycle has been played
            EndAnimation();

            // Invokes the onCycleEndedAction with the current cycle index
            _onCycleEndedAction?.Invoke(_currentCycleIndex);

            // Invokes the onEndAction
            _onEndAction?.Invoke();
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

            _currentWaitCounter = 0;
            _currentCycleWaitOverride = -1;
        }

        #endregion

        #region Cycles Handling

        /// <summary>
        /// Plays the next cycle.
        /// </summary>
        /// <returns>The updated <see cref="ComboAnimator"/>.</returns>
        public ComboAnimator Next()
        {
            // Check if not waiting and return current object if true
            if (!_waiting) return this;

            // Try to get the next cycle of the combo animation
            if (!_comboAnimation.TryGetCycle(_currentCycleIndex + 1, out _currentCycle))
            {
                // Log an error if the next cycle is not found
                Logger.LogError($"Could not find a next cycle for the current animation. Current index: {_currentCycleIndex} - Requested index: {_currentCycleIndex + 1}", _animator);
                return this;
            }

            // Increment the current cycle index
            _currentCycleIndex++;

            // Set waiting flag to false
            _waiting = false;

            // Reset the elapsed time for the current cycle
            _currentCycleElapsedTime = 0;

            // Return the updated ComboAnimator object
            return this;
        }

        /// <summary>
        /// Overrides the input wait time for the combo animator.
        /// </summary>
        /// <param name="maxInputWait">The maximum input wait time.</param>
        /// <returns>The updated <see cref="ComboAnimator"/>.</returns>
        public ComboAnimator OverrideInputWait(float maxInputWait)
        {
            _currentCycleWaitOverride = maxInputWait;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when the animation is interrupted.
        /// </summary>
        /// <param name="onInterrupted">The action to be executed.</param>
        /// <returns>The updated <see cref="ComboAnimator"/>.</returns>
        public ComboAnimator SetOnInterrupted(UnityAction onInterrupted)
        {
            _onInterruptedAction = onInterrupted;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when a cycle ends.
        /// </summary>
        /// <param name="onCycleEnded">The action to be executed.</param>
        /// <returns>The updated <see cref="ComboAnimator"/>.</returns>
        public ComboAnimator SetOnCycleEnded(UnityAction<int> onCycleEnded)
        {
            _onCycleEndedAction = onCycleEnded;
            return this;
        }

        #endregion
    }

}