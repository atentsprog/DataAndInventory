using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    Acc,
    Potion,
    Etc
}

[System.Serializable]
public class ItemInfo
{
    public int ID;
    public string name;
    public string iconName;
    public Sprite icon;
    public ItemType itemType;
    public int buyPrice;
    public int sellPrice;
    public string description;
    public int maxStackCount;
    // ������ ���(��޺��� �ٸ� ����̳� �׵θ�)
}

public class ShopItemData : MonoBehaviour
{
    public static ShopItemData instance;
    private void Awake()
    {
        instance = this;
    }
    public List<ItemInfo> item;
}
