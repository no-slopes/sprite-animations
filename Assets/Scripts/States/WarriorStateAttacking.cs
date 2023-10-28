using HandyFSM;
using SpriteAnimations;
using UnityEngine;

public class WarriorStateAttacking : WarriorState
{
    public override void OnEnter()
    {
        WarriorMachine.Animator.Play("Attack").SetOnFrame("Slash", OnSlash).SetOnEnd(OnAnimationEnd);
    }

    private void OnSlash(Frame frame)
    {
        Debug.Log("Slash frame played");
        // Check colisions and deal damage.
    }

    private void OnAnimationEnd()
    {
        State targetState;

        if (WarriorMachine.MovementInput == Vector2.zero)
        {
            targetState = WarriorMachine.GetState<WarriorStateIdle>();
        }
        else
        {
            targetState = WarriorMachine.GetState<WarriorStateRunning>();
        }

        WarriorMachine.EndState(targetState);
    }

}

