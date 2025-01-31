using UnityEngine;

[CreateAssetMenu(fileName = "newMeleeWeaponItem", menuName = "Item/Melee Weapon")]
public class MeleeWeaponItem : Item
{
    [Header("Weapon Information")]
    public MeleeWeaponType meleeWeaponType;
    public HolsterPosition holsterPosition;

    [Header("Weapon Animation")]
    public AnimatorOverrideController meleeWeaponAnimatorOverrideController;
    public float holdWeaponWeight;
    public float drawWeaponAnimationInSeconds;
}
