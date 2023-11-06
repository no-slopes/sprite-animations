using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace SpriteAnimations
{
    public class CompositeAnimator : AnimationPerformer
    {
        #region Fields

        /// <summary>
        /// The current <see cref="SpriteAnimationComposite"/> associated to the <see cref="CompositeAnimator"/>
        /// </summary>
        protected SpriteAnimationComposite _compositeAnimation;

        /// <summary>
        /// The current <see cref="CompositeCycle"/>
        /// </summary>
        protected CompositeCycle _currentCompositeCycle;

        /// <summary>
        /// If the current animation should loop the core cycle
        /// </summary>
        protected bool _shouldLoopCore;

        /// <summary>
        /// The action to be invoked when the antecipation cycle ends
        /// </summary>
        protected UnityAction _onAntecipationEndAction;

        /// <summary>
        /// The action to be invoked when the core cycle ends
        /// </summary>
        protected UnityAction _onCoreEndAction;

        /// <summary>
        /// The action to be invoked when the recovery cycle ends
        /// </summary>
        protected UnityAction _onRecoveryEndAction;

        #endregion


        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            base.StartAnimation(animation);

            _currentAnimation = animation;
            _compositeAnimation = _currentAnimation as SpriteAnimationComposite;

            _currentCompositeCycle = CompositeCycle.Antecipation;

            ResetCycle();
            ChangeCycle(_currentCompositeCycle);

            _isPlaying = true;
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void StopAnimation()
        {
            base.StopAnimation();
            _onAntecipationEndAction = null;
            _onCoreEndAction = null;
            _onRecoveryEndAction = null;
            EndAnimation();
        }

        #endregion

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

            if (_frameIdActions.TryGetValue(evaluatedFrame.Id, out var byIDAction))
            {
                byIDAction.Invoke(_currentFrame);
            }
        }

        /// <summary>
        /// Resets the current animation and plays it from start.
        /// </summary>
        /// <returns>The updated <see cref="CompositeAnimator"/> instance.</returns>
        public CompositeAnimator FromStart()
        {
            ResetCycle();
            ChangeCycle(CompositeCycle.Antecipation);
            _isPlaying = true;
            return this;
        }

        #region Cycles Handling

        /// <summary>
        /// Ends the current cycle of the composite animation changing the cycles according to the flow
        /// and triggering their respective actions.
        /// </summary>
        protected void EndCycle()
        {
            // Check if the composite animation has a current animation
            if (!HasCurrentAnimation)
            {
                // Log a warning if there is no current animation
                Logger.LogWarning($"Composite Animation cycle ended but it has no current animation. Please report this.", _animator);
                return;
            }

            // Reset the cycle state
            ResetCycle();

            // Check if the current cycle is Antecipation
            if (_currentCompositeCycle.Equals(CompositeCycle.Antecipation))
            {
                // Change the cycle to Core
                ChangeCycle(CompositeCycle.Core);

                // Invoke the action when the Antecipation cycle ends
                _onAntecipationEndAction?.Invoke();
                return;
            }

            // Check if the current cycle is Core
            if (_currentCompositeCycle.Equals(CompositeCycle.Core))
            {
                // Check if the Core cycle is not loopable
                if (!_shouldLoopCore)
                {
                    // Change the cycle to Recovery
                    ChangeCycle(CompositeCycle.Recovery);
                }

                // Invoke the action when the Core cycle ends
                _onCoreEndAction?.Invoke();
                return;
            }

            // Invoke the action when the Recovery cycle ends
            _onRecoveryEndAction?.Invoke();

            // End the animation
            EndAnimation();

            // Invoke the action when the overall cycle ends
            Debug.Log("End Cycle");
            _onEndAction?.Invoke();
        }

        /// <summary>
        /// Changes the current cycle of the composite animation.
        /// </summary>
        /// <param name="cycle">The new cycle to change to.</param>
        protected virtual void ChangeCycle(CompositeCycle cycle)
        {
            // Update the current cycle based on the input cycle
            _currentCycle = cycle switch
            {
                CompositeCycle.Antecipation => _compositeAnimation.AntecipationCycle,
                CompositeCycle.Core => _compositeAnimation.CoreCycle,
                CompositeCycle.Recovery => _compositeAnimation.RecoveryCycle,
                _ => _compositeAnimation.AntecipationCycle,
            };

            // Update the current composite cycle
            _currentCompositeCycle = cycle;

            // Get the first frame of the new cycle
            _currentFrame = _currentCycle.GetFirstFrame();

            // Update the sprite of the animator with the sprite of the current frame
            _animator.SpriteRenderer.sprite = _currentFrame.Sprite;
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
            _shouldLoopCore = false;
            _currentCycle = null;
            _currentFrame = null;
            _isPlaying = false;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Breaks the core cycle loop and starts the recovery cycle.
        /// </summary>
        /// <returns>The updated <see cref="CompositeAnimator"/> instance.</returns>
        public virtual CompositeAnimator ExitCoreLoop()
        {
            ResetCycle();
            ChangeCycle(CompositeCycle.Recovery);
            return this;
        }

        /// <summary>
        /// Sets whether the core cycle should loop or not.
        /// </summary>
        /// <param name="isLoopableCore">A boolean indicating whether the core cycle should loop.</param>
        /// <returns>The updated <see cref="CompositeAnimator"/> instance.</returns>
        public virtual CompositeAnimator SetLoopableCore(bool isLoopableCore)
        {
            _shouldLoopCore = isLoopableCore;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when the antecipation ends.
        /// </summary>
        /// <param name="onAntecipationEnd">The action to be executed.</param>
        /// <returns>The updated <see cref="CompositeAnimator"/> instance.</returns>
        public virtual CompositeAnimator SetOnAntecipationEnd(UnityAction onAntecipationEnd)
        {
            _onAntecipationEndAction = onAntecipationEnd;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when the core cycle ends.
        /// </summary>
        /// <param name="onCoreEnd">The action to be executed.</param>
        /// <returns>The updated <see cref="CompositeAnimator"/> instance.</returns>
        public virtual CompositeAnimator SetOnCoreEnd(UnityAction onCoreEnd)
        {
            _onCoreEndAction = onCoreEnd;
            return this;
        }

        /// <summary>
        /// Sets the action to be executed when the recovery cycle ends.
        /// </summary>
        /// <param name="onRecoveryEnd">The action to be executed.</param>
        /// <returns>The updated <see cref="CompositeAnimator"/> instance.</returns>
        public virtual CompositeAnimator SetOnRecoveryEnd(UnityAction onRecoveryEnd)
        {
            _onRecoveryEndAction = onRecoveryEnd;
            return this;
        }

        #endregion
    }

    public enum CompositeCycle
    {
        Antecipation,
        Core,
        Recovery,
    }
}