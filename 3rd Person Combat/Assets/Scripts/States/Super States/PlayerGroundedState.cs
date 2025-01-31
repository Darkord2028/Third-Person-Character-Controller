using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected bool isGrounded;
    protected bool sprintInput;
    protected bool AimInput;
    protected bool ReloadInput;
    protected bool MeleeAttackInput;
    protected bool EquipPrimaryWeaponInput;
    protected bool EquipSecondaryWeaponInput;
    protected bool EquipMeleeWeaponInput;
    protected bool EquipSubWeaponInput;
    protected bool PickUpWeaponInput;

    private bool JumpInput;
    private bool DodgeInput;

    protected float moveAmount;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
        player.animator.SetLayerWeight(0, 1f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        moveAmount = player.InputManager.moveAmount;
        sprintInput = player.InputManager.SprintInput;
        JumpInput = player.InputManager.JumpInput;
        DodgeInput = player.InputManager.DodgeInput;
        AimInput = player.InputManager.AimInput;
        ReloadInput = player.InputManager.ReloadInput;
        MeleeAttackInput = player.InputManager.MeleeAttackInput;

        EquipPrimaryWeaponInput = player.InputManager.EquipPrimaryWeaponInput;
        EquipSecondaryWeaponInput = player.InputManager.EquipSecondaryWeaponInput;
        EquipMeleeWeaponInput = player.InputManager.EquipMeleeWeaponInput;
        EquipSubWeaponInput = player.InputManager.EquipSubWeaponInput;

        PickUpWeaponInput = player.InputManager.PickUpWeaponInput;

        if (!isGrounded && !isExitingState)
        {
            StateMachine.ChangeState(player.InAirState);
        }
        else if (DodgeInput && player.DodgeState.CanSlide() && !isExitingState)
        {
            player.InputManager.UseDodgeInput();
            StateMachine.ChangeState(player.DodgeState);
        }
        else if (JumpInput && !isExitingState)
        {
            player.InputManager.UseJumpInput();
            StateMachine.ChangeState(player.JumpState);
        }
        else if (AimInput)
        {
            StateMachine.ChangeState(player.AimState);
        }
        else if (MeleeAttackInput)
        {
            player.InputManager.UseMeleeAttackInput();
            StateMachine.ChangeState(player.MeleeAttackState);
        }
        else if (ReloadInput)
        {
            player.InputManager.UseReloadInput();
            StateMachine.ChangeState(player.ReloadState);
        }
        else if (EquipPrimaryWeaponInput && player.EquipmentManager.currentPrimaryWeaponHolster != HolsterPosition.Hand && StateMachine.CurrentState == player.LocomotionState)
        {
            player.InputManager.UseEquipInput();
            player.InputManager.CanEquipWeapon(false);

            player.EquipmentManager.HandleUnEquippingOnTrigger(false, true, true, true);
            player.animator.runtimeAnimatorController = player.EquipmentManager.primaryWeapon.weaponAnimatorOverrideController;
            player.animator.SetTrigger("DrawWeapon");
            player.EquipmentManager.primaryWeapon.PlayEquipAudio();
        }
        else if (EquipSecondaryWeaponInput && player.EquipmentManager.currentSecondaryWeaponHolster != HolsterPosition.Hand && StateMachine.CurrentState == player.LocomotionState)
        {
            player.InputManager.UseEquipInput();
            player.InputManager.CanEquipWeapon(false);

            player.EquipmentManager.HandleUnEquippingOnTrigger(true, false, true, true);
            player.animator.runtimeAnimatorController = player.EquipmentManager.secondaryWeapon.weaponAnimatorOverrideController;
            player.animator.SetTrigger("DrawWeapon");
            player.EquipmentManager.primaryWeapon.PlayEquipAudio();
        }
        else if (EquipMeleeWeaponInput && player.EquipmentManager.currentMeleeWeaponHolster != HolsterPosition.Hand && StateMachine.CurrentState == player.LocomotionState)
        {
            player.InputManager.UseEquipInput();
            player.InputManager.CanEquipWeapon(false);

            player.EquipmentManager.HandleUnEquippingOnTrigger(true, true, false, true);
            player.animator.runtimeAnimatorController = player.EquipmentManager.meleeWeapon.meleeWeaponAnimatorOverrideController;
            player.animator.SetTrigger("DrawWeapon");
        }
        else if(EquipSubWeaponInput && player.EquipmentManager.currentSubWeaponHoster != HolsterPosition.Hand && StateMachine.CurrentState == player.LocomotionState)
        {
            player.InputManager.UseEquipInput();
            player.InputManager.CanEquipWeapon(false);

            player.EquipmentManager.HandleUnEquippingOnTrigger(true, true, true, false);
            player.animator.runtimeAnimatorController = player.EquipmentManager.subWeapon.subWeaponOverrideController;
            player.animator.SetTrigger("DrawWeapon");
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
