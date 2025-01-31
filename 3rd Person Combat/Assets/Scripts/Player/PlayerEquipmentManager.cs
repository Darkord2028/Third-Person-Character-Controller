using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerEquipmentManager : MonoBehaviour
{
    #region Variables

    Player player;
    Animator animator;
    RuntimeAnimatorController basePlayerAnimatorController;

    [Header("Initial Equipment")]
    public WeaponItem primaryWeapon;
    public WeaponItem secondaryWeapon;
    public MeleeWeaponItem meleeWeapon;
    public SubWeaponItem subWeapon;

    [Header("Holster Transforms")]
    public List<GameObject> holsterTransforms = new List<GameObject>();

    [Header("Weapon Firepoint")]
    public Transform currentWeaponFirepoint;

    #endregion

    #region Runtime Variables

    [Header("Runtime Equipment")]
    public GameObject currentPrimaryWeaponModel { get; private set; }
    public GameObject currentSecondaryWeaponModel { get; private set; }
    public GameObject currentMeleeWeaponModel { get; private set; }
    public GameObject currentSubWeaponModel { get; private set; }

    public HolsterPosition currentPrimaryWeaponHolster;
    public HolsterPosition currentSecondaryWeaponHolster;
    public HolsterPosition currentMeleeWeaponHolster;
    public HolsterPosition currentSubWeaponHoster;

    //Draw Weapon Flags
    private bool EquipPrimaryWeaponBool;
    private bool EquipSecondaryWeaponBool;
    private bool EquipMeleeWeaponBool;
    private bool EquipSubWeaponBool;

    #endregion

    #region Unity Callback Functions

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();
        basePlayerAnimatorController = animator.runtimeAnimatorController;
    }

    private void Start()
    {
        LoadAndHolsterWeapon(primaryWeapon, true);
        LoadAndHolsterWeapon(secondaryWeapon, false);
        LoadAndHolsterWeapon(subWeapon);
        LoadAndHolsterWeapon(meleeWeapon);
    }

    private void Update()
    {
        HandleWeaponEquippingOnTrigger();
    }

    #endregion

    #region Set Weapons

    public void SetPrimaryWeaponEquipTrigger()
    {
        EquipPrimaryWeaponBool = true;

        player.UIManager.SetWeaponSprite(primaryWeapon);
        player.UIManager.SetCurrentAmmo(primaryWeapon.CurrentAmmo, primaryWeapon.CurrentClipAmmo);
    }

    public void SetSecondaryWeaponEquipTrigger()
    {
        EquipSecondaryWeaponBool = true;

        player.UIManager.SetWeaponSprite(secondaryWeapon);
        player.UIManager.SetCurrentAmmo(secondaryWeapon.CurrentAmmo, secondaryWeapon.CurrentClipAmmo);
    }

    public void SetMeleeWeaponEquipTrigger()
    {
        EquipMeleeWeaponBool = true;

        player.UIManager.SetWeaponSprite(meleeWeapon);
    }

    public void SetSubWeaponEquipTrigger()
    {
        EquipSubWeaponBool = true;

        player.UIManager.SetWeaponSprite(subWeapon);
    }

    private void HandleWeaponEquippingOnTrigger()
    {
        if (EquipPrimaryWeaponBool && currentPrimaryWeaponHolster != HolsterPosition.Hand)
        {
            //Equipping Primary Weapon
            EquipPrimaryWeapon(HolsterPosition.Hand);
            EquipPrimaryWeaponBool = false;
        }
        else if (EquipSecondaryWeaponBool && currentSecondaryWeaponHolster != HolsterPosition.Hand)
        {
            //Equipping Secondary Weapon
            EquipSecondaryWeapon(HolsterPosition.Hand);
            EquipSecondaryWeaponBool = false;
        }
        else if (EquipMeleeWeaponBool && currentMeleeWeaponHolster != HolsterPosition.Hand)
        {
            //Equipping Melee Weapon
            EquipMeleeWeapon();
            EquipMeleeWeaponBool = false;
        }
        else if(EquipSubWeaponBool && currentSubWeaponHoster != HolsterPosition.Hand)
        {
            EquipSubWeapon();
            EquipSubWeaponBool = false;
        }
    }

    public void HandleUnEquippingOnTrigger(bool isPrimary = true, bool isSecondary = true, bool isMelee = true, bool isSubWeapon = true)
    {
        if (isPrimary)
        {
            UnequipWeapon(primaryWeapon, currentPrimaryWeaponModel, true);
        }
        if (isSecondary)
        {
            UnequipWeapon(secondaryWeapon, currentSecondaryWeaponModel, false);
        }
        if (isMelee)
        {
            UnequipMeleeWeapon();
        }
        if (isSubWeapon)
        {
            UnequipSubWeapon();
        }
    }

    #endregion

    #region Weapon Equip

    public void EquipPrimaryWeapon(HolsterPosition holsterPosition)
    {
        //Unequipping Previous Weapons
        //UnequipMeleeWeapon();
        //UnequipWeapon(secondaryWeapon, currentSecondaryWeaponModel, false);

        //Setting Weapon Holster
        SetHolsterPosition(primaryWeapon, currentPrimaryWeaponModel, true, primaryWeapon.weaponAnimatorOverrideController, holsterPosition);

        //Weapon Animations Settings
        player.SetAimIK(primaryWeapon.SpineOffset, primaryWeapon.ChestOffset, secondaryWeapon.HeadOffset);
        player.InputManager.CanEquipWeapon(true);
    }

    public void EquipSecondaryWeapon(HolsterPosition holsterPosition)
    {
        //Unequipping Previous Weapon
        //UnequipMeleeWeapon();
        //UnequipWeapon(primaryWeapon, currentPrimaryWeaponModel, true);

        //Setting Weapon Holster
        SetHolsterPosition(secondaryWeapon, currentSecondaryWeaponModel, false, secondaryWeapon.weaponAnimatorOverrideController, holsterPosition);

        //Setting Up IK and Animation Bool
        player.SetAimIK(secondaryWeapon.SpineOffset, secondaryWeapon.ChestOffset, secondaryWeapon.HeadOffset);
        player.InputManager.CanEquipWeapon(true);
    }

    public void EquipMeleeWeapon()
    {
        // Unequipping Previous Weapon
        //UnequipMeleeWeapon();
        //UnequipWeapon(secondaryWeapon, currentSecondaryWeaponModel, false);
        //UnequipWeapon(primaryWeapon, currentPrimaryWeaponModel, true);
        SetMeleeHolster();
        player.InputManager.CanEquipWeapon(true);
    }

    public void EquipSubWeapon(HolsterPosition weaponHolster = HolsterPosition.Hand)
    {
        // Unequipping Previous Weapon
        //UnequipMeleeWeapon();
        //UnequipWeapon(secondaryWeapon, currentSecondaryWeaponModel, false);
        //UnequipWeapon(primaryWeapon, currentPrimaryWeaponModel, true);
        SetSubWeaponHolster(weaponHolster);
        player.InputManager.CanEquipWeapon(true);
    }

    #endregion

    #region Weapon Unequip

    public void UnequipWeapon(WeaponItem currentWeapon, GameObject currentWeaponModel, bool isPrimary)
    {
        if(currentMeleeWeaponHolster == HolsterPosition.Hand) return;

        player.animator.runtimeAnimatorController = basePlayerAnimatorController;
        player.StateMachine.ChangeState(player.LocomotionState);

        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == currentWeapon.holsterPosition)
            {
                foreach (Transform t in holster.transform)
                {
                    WeaponLoaderSlot slot = t.GetComponent<WeaponLoaderSlot>();

                    if (slot.weapon == currentWeapon.weapon)
                    {
                        currentWeaponModel.transform.SetParent(t.transform, false);

                        if (isPrimary)
                        {
                            currentPrimaryWeaponHolster = currentWeapon.holsterPosition;
                        }
                        else
                        {
                            currentSecondaryWeaponHolster = currentWeapon.holsterPosition;
                        }
                    }
                }
            }
        }
    }

    public void UnequipMeleeWeapon()
    {
        if (currentMeleeWeaponHolster != HolsterPosition.Hand) return;

        player.animator.runtimeAnimatorController = basePlayerAnimatorController;
        player.StateMachine.ChangeState(player.LocomotionState);

        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if(holster.holsterPosition == meleeWeapon.holsterPosition)
            {
                foreach (Transform t in holster.transform)
                {
                    WeaponLoaderSlot slot = t.GetComponent<WeaponLoaderSlot>();

                    if (slot.meleeWeapon == meleeWeapon.meleeWeaponType)
                    {
                        currentMeleeWeaponModel.transform.SetParent(t.transform, false);
                        currentMeleeWeaponHolster = meleeWeapon.holsterPosition;
                    }
                }
            }
        }

    }

    public void UnequipSubWeapon()
    {
        if (currentSubWeaponHoster != HolsterPosition.Hand) return;

        player.animator.runtimeAnimatorController = basePlayerAnimatorController;
        player.StateMachine.ChangeState(player.LocomotionState);

        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == subWeapon.holsterPosition)
            {
                foreach (Transform t in holster.transform)
                {
                    WeaponLoaderSlot slot = t.GetComponent<WeaponLoaderSlot>();

                    if (slot.subWeapon == subWeapon.subWeaponType)
                    {
                        currentSubWeaponModel.transform.SetParent(t.transform, false);
                        currentSubWeaponHoster = meleeWeapon.holsterPosition;
                    }
                }
            }
        }
    }

    #endregion

    #region Load and Holster Weapon

    public void UnloadAndDestroyWeaponModel(GameObject currentWeaponModel)
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadAndHolsterWeapon(WeaponItem weapon, bool isPrimary)
    {
        if (weapon == null)
        {
            return;
        }

        if (isPrimary)
        {
            UnloadAndDestroyWeaponModel(currentPrimaryWeaponModel);
            primaryWeapon = weapon;
        }
        else
        {
            UnloadAndDestroyWeaponModel(currentSecondaryWeaponModel);
            secondaryWeapon = weapon;
        }

        GameObject weaponModel = Instantiate(weapon.itemModel, transform);

        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == weapon.holsterPosition)
            {
                foreach (Transform weaponLoader in holster.transform)
                {
                    WeaponLoaderSlot weaponLoaderSlot = weaponLoader.GetComponent<WeaponLoaderSlot>();

                    if (weaponLoaderSlot.weapon == weapon.weapon)
                    {
                        if (isPrimary)
                        {
                            currentPrimaryWeaponHolster = holster.holsterPosition;
                        }
                        else
                        {
                            currentSecondaryWeaponHolster = holster.holsterPosition;
                        }
                        weaponModel.transform.SetParent(weaponLoaderSlot.transform, false);
                    }
                }
            }
        }
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
        if (isPrimary)
        {
            currentPrimaryWeaponModel = weaponModel;
            primaryWeapon.Initialize(player, currentPrimaryWeaponModel.GetComponentInChildren<ParticleSystem>(), currentPrimaryWeaponModel.GetComponentInChildren<AudioSource>(), currentPrimaryWeaponModel.GetComponentInChildren<Animator>(), player.bulletPoolHolder);
        }
        else
        {
            currentSecondaryWeaponModel = weaponModel;
            secondaryWeapon.Initialize(player, currentSecondaryWeaponModel.GetComponentInChildren<ParticleSystem>(), currentSecondaryWeaponModel.GetComponentInChildren<AudioSource>(), currentSecondaryWeaponModel.GetComponentInChildren<Animator>(), player.bulletPoolHolder);
        }
    }

    public void LoadAndHolsterWeapon(MeleeWeaponItem weapon)
    {
        UnloadAndDestroyWeaponModel(currentMeleeWeaponModel);

        if (weapon == null)
        {
            return;
        }

        GameObject weaponModel = Instantiate(weapon.itemModel, transform);

        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == weapon.holsterPosition)
            {
                foreach (Transform weaponLoader in holster.transform)
                {
                    WeaponLoaderSlot weaponLoaderSlot = weaponLoader.GetComponent<WeaponLoaderSlot>();

                    if (weaponLoaderSlot.meleeWeapon == weapon.meleeWeaponType)
                    {
                        currentMeleeWeaponHolster = holster.holsterPosition;
                        weaponModel.transform.SetParent(weaponLoaderSlot.transform, false);
                    }
                }
            }
        }
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
        currentMeleeWeaponModel = weaponModel;
    }

    public void LoadAndHolsterWeapon(SubWeaponItem weapon)
    {
        UnloadAndDestroyWeaponModel(currentSubWeaponModel);

        if (weapon == null)
        {
            return;
        }

        GameObject weaponModel = Instantiate(weapon.itemModel, transform);

        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == weapon.holsterPosition)
            {
                foreach (Transform weaponLoader in holster.transform)
                {
                    WeaponLoaderSlot weaponLoaderSlot = weaponLoader.GetComponent<WeaponLoaderSlot>();

                    if (weaponLoaderSlot.subWeapon == weapon.subWeaponType)
                    {
                        currentSubWeaponHoster = holster.holsterPosition;
                        weaponModel.transform.SetParent(weaponLoaderSlot.transform, false);
                    }
                }
            }
        }
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
        currentSubWeaponModel = weaponModel;
        subWeapon.Initialize(player, currentSubWeaponModel.GetComponentInChildren<ParticleSystem>(), currentSubWeaponModel.GetComponent<SphereCollider>());
    }

    #endregion

    #region Set Holster Position

    private void SetMeleeHolster()
    {
        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == HolsterPosition.Hand)
            {
                foreach (GameObject weaponTransform in holster.weaponsList)
                {
                    WeaponLoaderSlot weaponLoaderSlot = weaponTransform.GetComponent<WeaponLoaderSlot>();

                    if (weaponLoaderSlot.meleeWeapon == meleeWeapon.meleeWeaponType)
                    {
                        currentMeleeWeaponModel.transform.SetParent(weaponTransform.transform, false);
                        currentMeleeWeaponHolster = HolsterPosition.Hand;
                        //animator.runtimeAnimatorController = meleeWeapon.meleeWeaponAnimatorController;
                    }
                }
            }
        }
    }

    private void SetSubWeaponHolster(HolsterPosition weaponHolster)
    {
        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == weaponHolster)
            {
                foreach (GameObject weaponTransform in holster.weaponsList)
                {
                    WeaponLoaderSlot weaponLoaderSlot = weaponTransform.GetComponent<WeaponLoaderSlot>();

                    if (weaponLoaderSlot.subWeapon == subWeapon.subWeaponType)
                    {
                        currentSubWeaponModel.transform.SetParent(weaponTransform.transform, false);
                        currentSubWeaponHoster = weaponHolster;
                        //animator.runtimeAnimatorController = meleeWeapon.meleeWeaponAnimatorController;
                    }
                }
            }
        }
    }

    private void SetHolsterPosition(WeaponItem weapon, GameObject weaponModel, bool isPrimary, AnimatorOverrideController weaponAnimatorController, HolsterPosition weaponHolster)
    {
        foreach (GameObject holsterPosition in holsterTransforms)
        {
            WeaponHolster holster = holsterPosition.GetComponent<WeaponHolster>();

            if (holster.holsterPosition == weaponHolster)
            {
                foreach (GameObject weaponTransform in holster.weaponsList)
                {
                    WeaponLoaderSlot weaponLoaderSlot = weaponTransform.GetComponent<WeaponLoaderSlot>();

                    if (weaponLoaderSlot.weapon == weapon.weapon)
                    {
                        weaponModel.transform.SetParent(weaponLoaderSlot.transform, false);
                        if (isPrimary)
                        {
                            currentPrimaryWeaponHolster = weaponHolster;
                        }
                        else
                        {
                            currentSecondaryWeaponHolster = weaponHolster;
                        }
                        //animator.runtimeAnimatorController = weaponAnimatorController;
                    }
                }
            }
        }
    }

    #endregion

    #region Other Functions

    public WeaponItem GetCurrentWeapon()
    {
        if(currentPrimaryWeaponHolster == HolsterPosition.Hand)
        {
            return primaryWeapon;
        }
        else if(currentSecondaryWeaponHolster == HolsterPosition.Hand)
        {
            return secondaryWeapon;
        }
        else { return null; }
    }

    public SubWeaponItem GetCurrentSubWeapon()
    {
        if (currentSubWeaponHoster == HolsterPosition.Hand)
        {
            return subWeapon;
        }
        else
        {
            return null;
        }
    }

    #endregion

}