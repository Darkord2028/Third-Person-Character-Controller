using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMeleeAttackState : PlayerAbilityState
{
    private bool isAttackInput;
    private float forwardSpeedMultiplier;

    public PlayerMeleeAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        isAbilityDone = false;

        forwardSpeedMultiplier = 1f;
    }

    public override void Exit()
    {
        base.Exit();
        player.InputManager.UseMeleeAttackInput();

        forwardSpeedMultiplier = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        player.SetSlide(playerData.forwardAttackMoveSpeed * forwardSpeedMultiplier);

        if(Time.time > startTime + playerData.timeToReadAttackInput)
        {
            isAttackInput = player.InputManager.MeleeAttackInput;
        }
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        isAbilityDone = true;
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        if (!isAttackInput)
        {
            forwardSpeedMultiplier = 0;
            isAbilityDone = true;
        }
        else
        {
            forwardSpeedMultiplier = 1f;
        }

    }

}
