using System;
using HandyFSM;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorStateRunning : WarriorState
{
    private Func<bool> IsIdle => () => WarriorMachine.MovementInput.x == 0;

    protected void OnInit()
    {
        SetInterruptible(true);

        State idleState = WarriorMachine.GetState<WarriorStateIdle>();
        AddTransition(IsIdle, idleState);
    }

    public void OnEnter()
    {
        Debug.Log("Running animation played");
        WarriorMachine.Animator.Play("Running").SetOnFrame(2, frame =>
        {
            Debug.Log($"Third frame of running animation played");
        });

        WarriorMachine.AttackAction.performed += OnAttackPerformed;
    }

    public void OnExit()
    {
        WarriorMachine.AttackAction.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        State attackingState = WarriorMachine.GetState<WarriorStateAttacking>();
        WarriorMachine.EndState(attackingState);
    }
}

