using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private bool isGrounded;

    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
        player.animator.SetLayerWeight(0, 0f);
        player.InputManager.UseAllInput();
    }

    public override void Exit()
    {
        base.Exit();
        player.InputManager.UseAllInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.SetMovement(playerData.inAirMoveSpeed);

        if (isGrounded && player.currentVelocity.y < 0.01f)
        {
            StateMachine.ChangeState(player.LocomotionState);
        }
        else if(isGrounded && player.currentVelocity.magnitude > 0f)
        {
            StateMachine.ChangeState(player.LocomotionState);
        }
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
