﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    InventoryItemInfo inventoryItemInfo;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            ItemInfoUI.instance.ShowInventoryItem(inventoryItemInfo);
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //inventoryItemInfo 아이템을 장착하자. -> 장착정보.
            //ShopItemInfo shopItemInfo = ShopItemData.instance.shopItems.Find(x => x.itemID == inventoryItemInfo.itemID);
            ShopItemInfo shopItemInfo = inventoryItemInfo.GetShopItemInfo();
        }
    }

    internal void Init(InventoryItemInfo item)
    {
        inventoryItemInfo = item;
        ShopItemInfo shopItemInfo = item.GetShopItemInfo();
        
        GetComponent<Image>().sprite = shopItemInfo.icon;
        transform.Find("CountText").GetComponent<Text>().text = item.count.ToString();
    }
}
