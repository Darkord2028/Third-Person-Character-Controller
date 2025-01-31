using UnityEngine;

public class PlayerReloadState : PlayerAbilityState
{
    //Input Flags
    private bool dodgeInput;
    private bool reloadInput;

    private WeaponItem currentWeapon;

    public PlayerReloadState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationFinishTrigger()
    {
        base.AnimationFinishTrigger();

        currentWeapon.Reload();
        player.UIManager.SetCurrentAmmo(currentWeapon.CurrentAmmo, currentWeapon.CurrentClipAmmo);

        isAbilityDone = true;
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        
        //Setting is Ability Done to false
        isAbilityDone = false;

        //For Player to move while Reloading
        player.animator.SetBool("aimMove", true);

        //Referencing the current weapon
        currentWeapon = player.EquipmentManager.GetCurrentWeapon();
    }

    public override void Exit()
    {
        base.Exit();

        //Setting animation bool to false
        player.animator.SetBool("aimMove", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        //Reading Inputs
        dodgeInput = player.InputManager.DodgeInput;
        reloadInput = player.InputManager.ReloadInput;

        //Locomotion while reloading
        player.SetAimRotation(playerData.reloadRotationSpeed);
        player.SetMovement(playerData.reloadWalkingSpeed);

        //Setting up aim camera movement
        CameraLockedMovementInput();

        //Considering State Change
        if (dodgeInput && !isExitingState)
        {
            player.StateMachine.ChangeState(player.DodgeState);
        }
        if (reloadInput)
        {
            player.InputManager.UseReloadInput();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
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
