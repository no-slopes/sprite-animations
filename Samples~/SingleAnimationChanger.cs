using System.Collections;
using System.Collections.Generic;
using SpriteAnimations;
using UnityEngine;

public class SingleAnimationChanger : MonoBehaviour
{
    [SerializeField]
    private Transform _characterTransform;

    [SerializeField]
    private SpriteAnimator _animator;

    [SerializeField]
    private MovementInputDetector _detector;

    private bool _attacking;

    private void OnEnable()
    {
        _animator.Play("Idle");

        // Listening for when the input sign changes.
        _detector.MovementSignChanged.AddListener(OnMovementSignChanged);
    }

    private void OnDisable()
    {
        _detector.MovementSignChanged.RemoveListener(OnMovementSignChanged);
    }

    private void Update()
    {
        EvaluateAttack();
    }

    private void EvaluateAttack()
    {
        if (_attacking || !Input.GetKeyDown(KeyCode.E)) return;

        // From here means the "E" key is pressed and the Attack animation is not currently running

        _attacking = true;

        _animator.Play("Attack").SetOnFrame("Slash", frame =>
        {
            Debug.Log("Slash frame played");
        }).OnEnd(() =>
        {
            _attacking = false;
            EvaluateMovementAnimation(_detector.Movement);
        });
    }

    private void FlipCharacter(Vector2Int sign)
    {
        int facingSign = (int)Mathf.Sign(_characterTransform.localScale.x);

        if (sign.x != 0 && sign.x != facingSign)
        {
            Vector2 newScale = new Vector2(_characterTransform.localScale.x * -1, _characterTransform.localScale.y);
            _characterTransform.localScale = newScale;
        }
    }

    // Evaluates wich animation (Idle or Run) should be played based on the given input.
    // This kind of operation is much more interesting done in a FSM driven solution.
    // This code is meant only to display the animations and show how to tell the animator
    // to play the desired animation.
    private void EvaluateMovementAnimation(Vector2 movementInput)
    {
        if (_attacking) return;

        if (movementInput == Vector2.zero)
        {
            _animator.Play("Idle");
        }
        else
        {
            _animator.Play("Run");
        }
    }

    // This method is being called every time the input sign changes
    // meaning we only care about input that express the direction is changing.
    private void OnMovementSignChanged(Vector2 input, Vector2Int sign)
    {
        FlipCharacter(sign);
        EvaluateMovementAnimation(input);
    }
}
