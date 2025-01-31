using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerAbilityState
{
    private bool AimInput;
    private bool FireInput;
    private bool canThrowBomb;

    private WeaponItem currentEquipWeapon;
    private SubWeaponItem currentSubWeapon;

    public PlayerAimState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        //Setting up State Variable
        isAbilityDone = false;

        //HUD Elements
        player.UIManager.crosshair.SetActive(true);

        //Animation Bool for movement while shooting
        player.animator.SetBool("aimMove", true);

        //Unequip Melee Weapon
        player.EquipmentManager.UnequipMeleeWeapon();

        //Checking if weapon is Equipped
        if (player.EquipmentManager.GetCurrentWeapon() == null)
        {
            if(player.EquipmentManager.GetCurrentSubWeapon() != null)
            {
                player.EquipmentManager.EquipSubWeapon(HolsterPosition.Aim);
                currentEquipWeapon = null;
                currentSubWeapon = player.EquipmentManager.subWeapon;
                canThrowBomb = true;
            }
            else
            {
                player.EquipmentManager.EquipPrimaryWeapon(HolsterPosition.Aim);
                player.UIManager.SetWeaponSprite(player.EquipmentManager.primaryWeapon);
                player.UIManager.SetCurrentAmmo(player.EquipmentManager.primaryWeapon.CurrentAmmo, player.EquipmentManager.primaryWeapon.CurrentClipAmmo);
                currentEquipWeapon = player.EquipmentManager.primaryWeapon;
                canThrowBomb = false;
            }
        }
        else
        {
            //Setting Holster Position to Aim Hand
            EquipWeaponOnAim();
        }

        //Changing Player Camera to Aim Camera
        player.playerAimCamera.Priority = 11;

        //Setting Body IK for aiming
        player.SetIKWeights(1);
    }

    public override void Exit()
    {
        base.Exit();

        //HUD Elements
        player.UIManager.crosshair.SetActive(false);

        //Animation Bool for movement while aiming
        player.animator.SetBool("aimMove", false);

        //Changing Holster Poisition from Aim to Hand
        EquipWeaponOnHand();

        //Changing Aim Camera to Player Camera
        player.playerAimCamera.Priority = 9;

        //Setting Body IK weight
        player.SetIKWeights(0);

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //Input Check
        AimInput = player.InputManager.AimInput;
        FireInput = player.InputManager.FireInput;

        //Locomotion while aiming
        player.SetAimRotation(playerData.aimRotationSpeed);
        player.SetMovement(playerData.aimWalkingSpeed);

        //Setting up aim camera movement
        CameraLockedMovementInput();

        //State Change Check
        if (!AimInput && !isExitingState)
        {
            isAbilityDone = true;
        }
        else if (FireInput && currentEquipWeapon != null)
        {
            Shoot();
        }
        else if(canThrowBomb && FireInput && currentSubWeapon != null)
        {
            canThrowBomb = false;
            ThrowWeapon();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void Shoot()
    {
        if(currentEquipWeapon.CurrentClipAmmo <= 0 && currentEquipWeapon.autoReload)
        {
            currentEquipWeapon.PlayEmptyClipAudio();
            player.StateMachine.ChangeState(player.ReloadState);
        }
        else if(currentEquipWeapon.CurrentClipAmmo > 0)
        {
            currentEquipWeapon.Shoot();
            player.UIManager.SetCurrentAmmo(currentEquipWeapon.CurrentAmmo, currentEquipWeapon.CurrentClipAmmo);
        }
    }

    private void ThrowWeapon()
    {
        currentSubWeapon.Explode();
    }

    private void EquipWeaponOnAim()
    {
        if (player.EquipmentManager.currentPrimaryWeaponHolster == HolsterPosition.Hand)
        {
            player.EquipmentManager.EquipPrimaryWeapon(HolsterPosition.Aim);
            currentEquipWeapon = player.EquipmentManager.primaryWeapon;
        }
        else
        {
            player.EquipmentManager.EquipSecondaryWeapon(HolsterPosition.Aim);
            currentEquipWeapon = player.EquipmentManager.secondaryWeapon;
        }
    }

    private void EquipWeaponOnHand()
    {
        if (player.EquipmentManager.currentPrimaryWeaponHolster == HolsterPosition.Aim)
        {
            player.EquipmentManager.EquipPrimaryWeapon(HolsterPosition.Hand);
        }
        else
        {
            player.EquipmentManager.EquipSecondaryWeapon(HolsterPosition.Hand);
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


}
