using System.Collections.Generic;
using UnityEngine;

public class WeaponHolster : MonoBehaviour
{
    public HolsterPosition holsterPosition;
    public List<GameObject> weaponsList = new List<GameObject>();
}

public enum HolsterPosition
{
    HipsRight,
    HipsLeft,
    Back,
    Hand,
    Aim
}

public enum WeaponType
{
    None,
    M1911,
    AK47,
    Bennelli_M4
}

public enum MeleeWeaponType
{
    None,
    Bat
}

public enum SubWeaponType
{
    None,
    Grenade,
    SmokeBomb,
    FlashBomb
}
