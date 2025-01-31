using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item Information")]
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemModel;
}
