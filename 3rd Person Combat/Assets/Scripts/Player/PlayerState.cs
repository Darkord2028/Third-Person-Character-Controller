using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine StateMachine;
    protected PlayerData playerData;

    protected bool isExitingState;
    protected bool isAnimationFinished;

    protected float startTime;

    private string AnimBoolName;

    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        StateMachine = stateMachine;
        this.playerData = playerData;
        AnimBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();
        startTime = Time.time;
        player.animator.SetBool(AnimBoolName, true);
        isExitingState = false;
        if (player.DebugBool)
        {
            Debug.Log(AnimBoolName);
        }
        isAnimationFinished = false;
    }

    public virtual void Exit()
    {
        isExitingState = true;
        player.animator.SetBool(AnimBoolName, false);
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() => DoChecks();

    public virtual void DoChecks() { }

    public virtual void AnimationTrigger() { }

    public virtual void AnimationFinishTrigger() => isAnimationFinished = true;

}
