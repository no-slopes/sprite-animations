using System.Collections;
using System.Collections.Generic;
using SpriteAnimations;
using UnityEngine;

namespace SpriteAnimations.Sampling
{
    public class WindroseAnimationChanger : MonoBehaviour
    {
        [SerializeField]
        private SpriteAnimator _animator;

        [SerializeField]
        private MovementInputDetector _detector;

        private bool _walking;
        private bool _rolling;
        private Vector2Int _facingDirection;
        private WindroseAnimator _windroseAnimator;

        private void OnEnable()
        {
            _windroseAnimator = _animator.Play<WindroseAnimator>("Idle");
            _windroseAnimator.SetDirection(Vector2.down);

            // Listening for when the input sign changes.
            _detector.MovementSignChanged.AddListener(OnMovementSignChanged);
        }

        private void OnDisable()
        {
            _detector.MovementSignChanged.RemoveListener(OnMovementSignChanged);
        }

        private void Update()
        {
            EvaluateRoll();
        }

        private void EvaluateRoll()
        {
            if (_rolling || !Input.GetKeyDown(KeyCode.E)) return;

            // From here means the "E" key is pressed and the rolling animation is not currently running

            _rolling = true;
            _walking = false;

            // Now we play the roll animation setting the direction to the current facing direction
            // and register an action to evaluate the movement animation (if idle or walking) at the end
            // of the roll animation.
            _animator.Play<WindroseAnimator>("Roll").SetDirection(_facingDirection).SetOnEnd(() =>
            {
                _rolling = false;
                Debug.Log("Roll animation ended. Evaluating movement animation.");
                EvaluateMovementAnimation(_detector.Movement, _facingDirection);
            });
        }

        // Evaluates wich animation (Idle or Walk) should be played based on the given input.
        private void EvaluateMovementAnimation(Vector2 movementInput, Vector2Int sign)
        {
            if (_rolling) return;

            if (movementInput == Vector2.zero)
            {
                // When we talk about 8-Directional top-down movement there is an issue regarding
                // leaving a diagonal direction an playing the idle animation right away. The Input
                // System detects an input change before detecting the "zero" input. This causes the idle
                // animation to always face a non diagonal direction. As this is an input reading related problem
                // and depending on the game needs would be a differente solution, treating this here would be
                // extremely out of scope for a sample. The SpriteAnimations solution focuses only on playing the animations.
                _walking = false;
                _animator.Play<WindroseAnimator>("Idle");
            }
            else
            {
                _facingDirection = sign;
                // This kind of redundant check is exactly what a FSM would prevent. When dealing with character states
                // it is much better to code in a FSM driven solution. 
                // Here we do not simply tell the animator to play again because if the character is already walking
                // we want the animator to change the direction only for the next frame. Calling Play again would
                // make the animator play the walk cycle for the other direction from the first frame.
                if (!_walking)
                {
                    _walking = true;
                    _windroseAnimator = _animator.Play<WindroseAnimator>("Walk").SetDirection(movementInput);
                }
                else
                {
                    _windroseAnimator.SetDirection(movementInput);
                }
                _windroseAnimator.SetOnFrame(2, OnThirdWalkFrame);
            }
        }

        // This method is being called every time the input sign changes
        // meaning we only care about input that express the direction is changing.
        private void OnMovementSignChanged(Vector2 input, Vector2Int sign)
        {
            EvaluateMovementAnimation(input, sign);
        }

        // This method is fired every time the third frame (index 2) of the walk animation
        // is played. Just to demonstrate how the Frame Actions can be registered.
        private void OnThirdWalkFrame(Frame frame)
        {
            Debug.Log($"The third frame of the Walk animation has been played as {Time.time}");
        }
    }
}
