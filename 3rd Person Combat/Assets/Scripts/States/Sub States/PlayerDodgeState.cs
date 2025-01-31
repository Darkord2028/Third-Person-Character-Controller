using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : PlayerAbilityState
{
    private bool isGrounded;
    private float slideSpeed;
    private int slideSpeedMultiplier;

    public PlayerDodgeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();
        if (!isExitingState)
        {
            isAbilityDone = true;
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        slideSpeedMultiplier = 0;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = player.isGrounded();
    }

    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;
        slideSpeed = playerData.slideSpeed;
        slideSpeedMultiplier = 1;
    }

    public override void Exit()
    {
        base.Exit();
        player.InputManager.UseDodgeInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        slideSpeed *= slideSpeedMultiplier;
        player.SetSlide(slideSpeed);
    }

    public bool CanSlide()
    {
        if(Time.time > startTime + playerData.dodgeTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
