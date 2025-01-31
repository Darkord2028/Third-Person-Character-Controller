using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    protected bool isAbilityDone;
    private bool isGrounded;

    protected Vector2 moveInput;
    protected float moveAmount;

    public PlayerAbilityState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = player.isGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        player.InputManager.UseAllInput();

        isAbilityDone = false;
    }

    public override void Exit()
    {
        base.Exit();
        player.InputManager.UseAllInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        moveInput = player.InputManager.MovementInput;
        moveAmount = player.InputManager.moveAmount;

        if (isAbilityDone && !isExitingState)
        {
            if (isGrounded)
            {
                StateMachine.ChangeState(player.LocomotionState);
            }
            else
            {
                StateMachine.ChangeState(player.InAirState);
            }
            
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
