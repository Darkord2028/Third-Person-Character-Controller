using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionState : PlayerGroundedState
{
    private WeaponItem pickUpWeapon;

    public PlayerLocomotionState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        pickUpWeapon = player.CheckGunDrops();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.SetRotation(playerData.movementRotation);

        if (!sprintInput)
        {
            player.animator.SetFloat("moveX", 0);
            player.animator.SetFloat("moveY", moveAmount);
        }
        if (!isGrounded && !isExitingState)
        {
            StateMachine.ChangeState(player.InAirState);
        }
        else if (moveAmount == 0)
        {
            player.InputManager.UseSprintInput();
        }
        else if (moveAmount == 0.5f && !sprintInput)
        {
            player.SetMovement(playerData.walkingSpeed);
        }
        else if (moveAmount == 1 && !sprintInput)
        {
            player.SetMovement(playerData.movementSpeed);
        }
        else if(sprintInput)
        {
            player.SetMovement(playerData.sprintSpeed);
            player.animator.SetFloat("moveX", 0);
            player.animator.SetFloat("moveY", 2);
        }
        
        if(pickUpWeapon != null)
        {
            player.InputManager.UsePickUpWeaponInput();
            player.UIManager.SetPickWeaponSprite(pickUpWeapon);

            if (PickUpWeaponInput)
            {
                PickUpAndEquip();
            }
        }

    }

    private void CameraLockedMovementInput()
    {
        float rawHorizontalInput = player.InputManager.MovementInput.x;
        float rawVerticalInput = player.InputManager.MovementInput.y;

        float horizontalInput;
        float verticalInput;

        //Horizontal Raw Input
        if (rawHorizontalInput > 0f && rawHorizontalInput <= 0.5f)
        {
            horizontalInput = 0.5f;
        }
        else if (rawHorizontalInput > 0.5f && rawHorizontalInput <= 1)
        {
            horizontalInput = 1;
        }
        else if (rawHorizontalInput < 0 && rawHorizontalInput >= -0.5f)
        {
            horizontalInput = -0.5f;
        }
        else if (rawHorizontalInput < -0.5f && rawHorizontalInput >= -1)
        {
            horizontalInput = -1;
        }
        else
        {
            horizontalInput = 0;
        }

        //Vertical Raw Input
        if (rawVerticalInput > 0f && rawVerticalInput <= 0.5f)
        {
            verticalInput = 0.5f;
        }
        else if (rawVerticalInput > 0.5f && rawVerticalInput <= 1)
        {
            verticalInput = 1;
        }
        else if (rawVerticalInput < 0 && rawVerticalInput >= -0.5f)
        {
            verticalInput = -0.5f;
        }
        else if (rawVerticalInput < -0.5f && rawVerticalInput >= -1)
        {
            verticalInput = -1;
        }
        else
        {
            verticalInput = 0;
        }

        player.animator.SetFloat("moveX", horizontalInput, 0.1f, Time.deltaTime);
        player.animator.SetFloat("moveY", verticalInput, 0.1f, Time.deltaTime);

    }

    private void PickUpAndEquip()
    {
        if (pickUpWeapon.isPrimary)
        {
            player.EquipmentManager.UnequipWeapon(pickUpWeapon, player.EquipmentManager.currentPrimaryWeaponModel, pickUpWeapon.isPrimary);
        }
        else
        {
            player.EquipmentManager.UnequipWeapon(pickUpWeapon, player.EquipmentManager.currentSecondaryWeaponModel, pickUpWeapon.isPrimary);
        }

        player.EquipmentManager.LoadAndHolsterWeapon(pickUpWeapon, pickUpWeapon.isPrimary);
        player.UIManager.SetDefaultWeaponUI();
        player.StateMachine.ChangeState(player.LocomotionState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
