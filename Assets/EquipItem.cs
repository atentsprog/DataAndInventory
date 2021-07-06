using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipItem : MonoBehaviour
{
    internal void SetItem(InventoryItemInfo inventoryItemInfo)
    {
        // 아이콘 설정.
        ShopItemInfo shopItemInfo = inventoryItemInfo.GetShopItemInfo();

        Image iamge = transform.Find("Image").GetComponent<Image>();
        iamge.sprite = shopItemInfo.Icon;
    }
}
