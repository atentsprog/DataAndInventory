using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipItem : MonoBehaviour
{
    public static Dictionary<ItemType, EquipItem> Items = new Dictionary<ItemType, EquipItem>();
    public ItemType itemType;
    private void Awake()
    {
        Items[itemType] = this;
    }

    internal void SetItem(ItemInfo item)
    {
        UserData.equipedItem[item.itemType] = item.ID;
        UpdateIcon(item.icon);
    }

    public void UpdateIcon(Sprite sprte)
    {
        transform.Find("Icon").GetComponent<Image>().sprite = sprte;
    }
}
