using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    private Player player;

    [Header("Crosshair")]
    public GameObject crosshair;

    [Header("Weapon")]
    public Sprite DefaultSprite;
    public Image weaponSprite;
    public TextMeshProUGUI weaponAmmoText;

    [Header("Pick Up Weapon")]
    public Image pickUpWeaponIcon;
    public TextMeshProUGUI pickUpWeaponText;

    private void Start()
    {
        player = GetComponent<Player>();

        weaponSprite.sprite = DefaultSprite;
        crosshair.SetActive(false);

        pickUpWeaponText.text = "";
    }

    #region Set Functions

    public void SetWeaponSprite(Item weapon)
    {
        weaponSprite.sprite = weapon.itemSprite;
    }

    public void SetCurrentAmmo(int currentAmmo, int currentClipAmmo)
    {
        weaponAmmoText.text = $"{currentClipAmmo} / {currentAmmo}";
    }

    public void SetPickWeaponSprite(Item weapon)
    {
        pickUpWeaponIcon.sprite = weapon.itemSprite;
        pickUpWeaponText.text = $"{"Pick Up"} {weapon.itemName}";
    }

    public void SetDefaultWeaponUI()
    {
        weaponSprite.sprite = DefaultSprite;
        weaponAmmoText.text = "";
    }

    #endregion

}
