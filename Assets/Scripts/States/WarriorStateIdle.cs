using System;
using HandyFSM;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarriorStateIdle : WarriorState
{
    private Func<bool> IsRunning => () => WarriorMachine.MovementInput.x != 0;

    protected void OnInit()
    {
        SetInterruptible(true);

        State runningState = WarriorMachine.GetState<WarriorStateRunning>();
        AddTransition(IsRunning, runningState);
    }

    public void OnEnter()
    {
        WarriorMachine.Animator.Play("Idle");

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
