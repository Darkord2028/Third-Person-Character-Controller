using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputManager : MonoBehaviour
{
    #region Input Flags
    public Vector2 MovementInput { get; private set; }
    public float moveAmount { get; private set; }
    public bool SprintInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DodgeInput { get; private set; }
    public bool AimInput { get; private set; }
    public bool MeleeAttackInput { get; private set; }
    public bool FireInput { get; private set; }
    public bool ReloadInput { get; private set; }
    public bool EquipPrimaryWeaponInput { get; private set; }
    public bool EquipSecondaryWeaponInput { get; private set; }
    public bool EquipMeleeWeaponInput { get; private set; }
    public bool EquipSubWeaponInput { get; private set; }
    public bool PickUpWeaponInput { get; private set; }

    #endregion

    #region Private Variables

    private bool canEquipWeapon = true;

    #endregion

    #region Class References

    PlayerEquipmentManager EquipmentManager;

    #endregion

    #region Unity Callback Functions

    private void Start()
    {
        EquipmentManager = GetComponent<PlayerEquipmentManager>();
    }

    private void Update()
    {
        HandleRawMovementInput();
    }

    #endregion

    #region Input Actions

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SprintInput = !SprintInput;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
        }
    }

    public void OnDodgeInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            DodgeInput = true;
        }
    }

    public void OnAimInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            AimInput = true;
        }
        else if (context.canceled)
        {
            AimInput = false;
        }
    }

    public void OnReloadInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ReloadInput = true;
        }
    }

    public void OnMeleeAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MeleeAttackInput = true;
        }
    }

    public void OnPrimaryWeaponEquip(InputAction.CallbackContext context)
    {
        if (context.started && canEquipWeapon)
        {
            EquipPrimaryWeaponInput = true;
        }
    }

    public void OnSecondaryWeaponEquip(InputAction.CallbackContext context)
    {
        if (context.started && canEquipWeapon)
        {
            EquipSecondaryWeaponInput = true;
        }
    }

    public void OnMeleeWeaponEquip(InputAction.CallbackContext context)
    {
        if (context.started && canEquipWeapon)
        {
            EquipMeleeWeaponInput = true;
        }
    }

    public void OnSubWeaponEquip(InputAction.CallbackContext context)
    {
        if(context.started && canEquipWeapon)
        {
            EquipSubWeaponInput = true;
        }
    }

    public void OnShootInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            FireInput = true;
        }
        else if (context.canceled)
        {
            FireInput = false;
        }
    }

    public void OnPickUpItem(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            PickUpWeaponInput = true;
        }
    }

    #endregion

    #region Handle Input Functions

    private void HandleRawMovementInput()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(MovementInput.x) + Mathf.Abs(MovementInput.y));

        if (moveAmount <= 0.5f && moveAmount > 0f)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1f)
        {
            moveAmount = 1f;
        }
    }

    #endregion

    #region Use Inputs Functions

    public void UseSprintInput () => SprintInput = false;
    public void UseJumpInput () => JumpInput = false;
    public void UseDodgeInput () => DodgeInput = false;
    public void UseMeleeAttackInput() => MeleeAttackInput = false;
    public void UseEquipPrimaryWeaponInput() => EquipPrimaryWeaponInput = false;
    public void UseEquipSecondaryWeaponInput() => EquipSecondaryWeaponInput = false;
    public void UseEquipMeleeWeaponInput() => EquipMeleeWeaponInput = false;
    public void UseEquipSubWeaponInput() => EquipSubWeaponInput = false;
    public void UseReloadInput() => ReloadInput = false;
    public void UsePickUpWeaponInput() => PickUpWeaponInput = false;

    public void UseAllInput()
    {
        UseJumpInput();
        UseSprintInput();
        UseDodgeInput();
        UseMeleeAttackInput();
        UseEquipPrimaryWeaponInput();
        UseEquipSecondaryWeaponInput();
        UseEquipMeleeWeaponInput();
        UseEquipSubWeaponInput();
        UseReloadInput();
        UsePickUpWeaponInput();
    }

    public void UseEquipInput()
    {
        UseEquipMeleeWeaponInput();
        UseEquipPrimaryWeaponInput();
        UseEquipSecondaryWeaponInput();
        UseEquipSubWeaponInput();
    }

    #endregion

    #region Other Weapons

    public void CanEquipWeapon(bool boolCanEquip)
    {
        canEquipWeapon = boolCanEquip;
    }

    #endregion

}
