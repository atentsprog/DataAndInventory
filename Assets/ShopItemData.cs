using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Etc,
}

[System.Serializable]
public class ShopItemInfo
{
    // �̸�, ������, ����, 
    public string name;
    public Sprite icon;
    public string description;
    public ItemType type;
    public int buyPrice;
    public int sellPrice;
}

public class ShopItemData : MonoBehaviour
{
    public static ShopItemData instance;
    private void Awake()
    {
        instance = this;
    }
    public List<ShopItemInfo> shopItems;
}
