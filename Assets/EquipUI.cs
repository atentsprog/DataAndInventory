using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipUI : MonoBehaviour
{
    public static EquipUI instance;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (var item in UserData.equipedItem)
        {
            var shopItem = ShopItemData.GetItem(item.Value);
            EquipItem.Items[item.Key].UpdateIcon(shopItem.icon);
        }
    }

    internal void SetEquipItem(HaveItemInfo haveItemInfo)
    {
        var item = ShopItemData.GetItem(haveItemInfo);
        EquipItem.Items[item.itemType].SetItem(item);
    }
}
