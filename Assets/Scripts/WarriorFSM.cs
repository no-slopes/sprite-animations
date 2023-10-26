using System;
using System.Collections;
using System.Collections.Generic;
using HandyFSM;
using Sirenix.OdinInspector;
using SpriteAnimations;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorFSM : StateMachine
{
    #region Inspector

    [BoxGroup("WarriorFSM")]
    [SerializeField]
    private SpriteAnimator _animator;

    [BoxGroup("WarriorFSM")]
    [SerializeField]
    private PlayerInput _playerInput;

    [BoxGroup("WarriorFSM")]
    [SerializeField]
    private InputActionReference _movementReference;

    [BoxGroup("WarriorFSM")]
    [SerializeField]
    private InputActionReference _attackReference;

    #endregion

    #region Fields

    private InputAction _movementAction;
    private InputAction _attackAction;

    private Vector2 _movementInput;

    #endregion

    #region Getters

    public SpriteAnimator Animator => _animator;

    public Vector2 MovementInput => _movementInput;

    public Type LoadableStateType => typeof(WarriorState);
    public Type DefaultStateType => typeof(WarriorStateIdle);

    public InputAction MovementAction => _movementAction;
    public InputAction AttackAction => _attackAction;

    #endregion

    #region Behaviour

    protected override void Awake()
    {
        base.Awake();
        _movementAction = _playerInput.actions.FindAction(_movementReference.action.id);
        _attackAction = _playerInput.actions.FindAction(_attackReference.action.id);
    }

    private void OnEnable()
    {
        _movementAction.performed += OnMovementInputPerformed;
        _movementAction.canceled += OnMovementInputStopped;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _movementAction.performed -= OnMovementInputPerformed;
        _movementAction.canceled -= OnMovementInputStopped;
    }

    #endregion

    #region Flip

    private void FlipCharacter(float sign)
    {
        int facingSign = (int)Mathf.Sign(_animator.transform.localScale.x);

        if (sign != facingSign)
        {
            Vector2 newScale = new Vector2(_animator.transform.localScale.x * -1, _animator.transform.localScale.y);
            _animator.transform.localScale = newScale;
        }
    }

    #endregion

    #region Callbacks

    private void OnMovementInputPerformed(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
        FlipCharacter(Mathf.Sign(_movementInput.x));
    }

    private void OnMovementInputStopped(InputAction.CallbackContext context)
    {
        _movementInput = Vector2.zero;
    }

    #endregion
}
