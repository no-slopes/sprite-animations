using System;
using System.Linq;
using UnityEngine;
using static SpriteAnimations.SpriteAnimationWindrose;

namespace SpriteAnimations
{
    /// <summary>
    /// The performer responsible for playing windrose animations.
    /// </summary>
    public class WindroseAnimator : AnimationPerformer
    {
        #region Fields

        protected SpriteAnimationWindrose _windroseAnimation;

        /// <summary>
        /// The current direction in wich the animation is playing.
        /// </summary>
        protected WindroseDirection _currentDirection = WindroseDirection.South;

        #endregion

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            base.StartAnimation(animation);

            _currentAnimation = animation;
            _windroseAnimation = _currentAnimation as SpriteAnimationWindrose;

            ResetCycle();

            // Try to get the cycle for the specified direction
            _windroseAnimation.TryGetCycle(_currentDirection, out _currentCycle);

            if (_currentCycle.Size > 0)
            {
                _animator.SpriteRenderer.sprite = _currentCycle.Frames.First().Sprite;
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
            if (_currentCycleElapsedTime >= _currentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
                return;
            }

            int frameIndex = CalculateFrameIndex(_currentCycleElapsedTime, _currentCycle.Size, _currentCycleDuration);
            Frame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(frameIndex);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return;

            // From here it means the new frame will be displayed

            _currentFrame = evaluatedFrame;
            _animator.SpriteRenderer.sprite = _currentFrame.Sprite;

            InvokeFramePlayed(frameIndex, _currentFrame);

            // Calling frame actions
            if (_frameIndexActions.TryGetValue(frameIndex, out var byIndexAction))
            {
                byIndexAction.Invoke(_currentFrame);
            }
        }

        /// <summary>
        /// Resets the current animation cycle and starts playing the windrose animation from the start.
        /// </summary>
        /// <returns>The updated <see cref="WindroseAnimator"/> instance.</returns>
        public WindroseAnimator FromStart()
        {
            // Check if there is a current animation
            if (!HasCurrentAnimation)
            {
                Logger.LogError($"Trying to play a {nameof(WindroseAnimator)} from start but it has no current animation. Have you called 'Play()' before?", _animator);
                return this;
            }

            // Reset the animation cycle
            ResetCycle();

            // Try to get the cycle for the specified direction
            _windroseAnimation.TryGetCycle(_currentDirection, out _currentCycle);

            if (_currentCycle.Size > 0)
            {
                _animator.SpriteRenderer.sprite = _currentCycle.Frames.First().Sprite;
            }

            _isPlaying = true;
            return this;
        }

        #endregion

        #region Directions

        /// <summary>
        /// Sets the direction of the animation.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <param name="flipStrategy">The strategy to use when flipping the sprite.</param>
        /// <returns>The updated <see cref="WindroseAnimator"/> instance.</returns>
        public WindroseAnimator SetDirection(WindroseDirection direction, WindroseFlipStrategy flipStrategy = WindroseFlipStrategy.NoFlip)
        {
            if (!_isPlaying)
            {
                // Log an error if the cycle does not exist
                Logger.LogError($"Trying to set direction for a non playing windrose animator. "
                + $"Did you call 'Play()' before setting the direction?", _animator);
                return this;
            }

            if (!HasCurrentAnimation)
            {
                Logger.LogError($"Trying to set direction for a {nameof(WindroseAnimator)} "
                + $"wich has no current animation. Please, report this issue.", _animator);
                return this;
            }

            if (flipStrategy.Equals(WindroseFlipStrategy.FlipEastToPlayWest))
            {
                bool shouldFlip = false;
                switch (direction)
                {
                    case WindroseDirection.West:
                        shouldFlip = true;
                        direction = WindroseDirection.East;
                        break;
                    case WindroseDirection.NorthWest:
                        shouldFlip = true;
                        direction = WindroseDirection.NorthEast;
                        break;
                    case WindroseDirection.SouthWest:
                        shouldFlip = true;
                        direction = WindroseDirection.SouthEast;
                        break;
                }

                _animator.SpriteRenderer.flipX = shouldFlip;
            }
            else
            {
                _animator.SpriteRenderer.flipX = false;
            }

            // Tries to get the cycle for the specified direction
            if (!_windroseAnimation.TryGetCycle(direction, out _currentCycle))
            {
                // Log an error if the cycle does not exist
                Logger.LogError($"Animation '{_currentAnimation.AnimationName}' does not have a cycle for direction {direction} "
                + $"using the '{flipStrategy}' strategy. The animation will now be stopped.", _animator);
                StopAnimation();
                return this;
            }

            // Sets the current direction
            _currentDirection = direction;

            _currentCycleDuration = _currentCycle.CalculateDuration(_currentAnimation.FPS);

            if (_currentCycle.Size > 0)
            {
                int frameIndex = CalculateFrameIndex(_currentCycleElapsedTime, _currentCycle.Size, _currentCycleDuration);
                Frame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(frameIndex);
                if (evaluatedFrame != null)
                {
                    _animator.SpriteRenderer.sprite = evaluatedFrame.Sprite;
                }
            }
            else
            {
                Logger.LogWarning($"{_windroseAnimation.AnimationName} has no frames to be played for the " +
                $" the '{direction}' direction using the '{flipStrategy}' strategy.", _animator);
            }

            return this;
        }

        /// <summary>
        /// Sets the direction of the animation.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <param name="flipStrategy">The strategy to use when flipping the sprite.</param>
        /// <returns>The updated <see cref="WindroseAnimator"/> instance.</returns>
        public WindroseAnimator SetDirection(Vector2 movementInput, WindroseFlipStrategy flipStrategy = WindroseFlipStrategy.NoFlip)
        {
            // Sets the current direction
            Vector2Int signedMovementInput = DirectionSign(movementInput);
            WindroseDirection direction = DirectionFromInput(signedMovementInput);
            return SetDirection(direction, flipStrategy);
        }

        /// <summary>
        /// Sets the direction of the animation.
        /// </summary>
        /// <param name="direction">The direction to set.</param>
        /// <returns>The updated <see cref="WindroseAnimator"/> instance.</returns>
        public WindroseAnimator SetDirection(Vector2Int signedMovementInput, WindroseFlipStrategy flipStrategy = WindroseFlipStrategy.NoFlip)
        {
            // Sets the current direction
            WindroseDirection direction = DirectionFromInput(signedMovementInput);
            return SetDirection(direction, flipStrategy);
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
                Logger.LogWarning($"Windrose Animation cycle ended but it has no current animation. Please report this.", _animator);
                return;
            }

            if (_windroseAnimation.IsLoopable)
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
        /// Resets the current cycle.
        /// </summary>
        protected void ResetCycle()
        {
            _currentCycleElapsedTime = 0f;
            _currentFrame = null;
        }

        protected void EndAnimation()
        {
            _currentCycle = null;
            _currentFrame = null;
            _isPlaying = false;
        }

        #endregion
    }

}