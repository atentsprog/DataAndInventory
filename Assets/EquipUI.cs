using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipUI : MonoBehaviour
{
    public EquipItem weaponEquipItem;
    public EquipItem armorEquipItem;
    public EquipItem potionEquipItem;

    public static EquipUI instance;
    private void Awake()
    {
        instance = this;
    }

    internal void SetEquipItem(InventoryItemInfo inventoryItemInfo)
    {
        ShopItemInfo shopItemInfo = inventoryItemInfo.GetShopItemInfo();

        switch (shopItemInfo.type)
        {
            case ItemType.Weapon:
                weaponEquipItem.SetItem(inventoryItemInfo); 
                break;
            case ItemType.Armor: armorEquipItem.SetItem(inventoryItemInfo); break;
            case ItemType.Potion: potionEquipItem.SetItem(inventoryItemInfo); break;
        }
    }
}
