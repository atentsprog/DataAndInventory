using System;
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
        ItemInfoUI.instance.ShowInventoryItem(inventoryItemInfo);
    }

    internal void Init(InventoryItemInfo item)
    {
        inventoryItemInfo = item;
        ShopItemInfo shopItemInfo = ShopItemData.instance.shopItems.Find(x => x.itemID == item.itemID);
        
        GetComponent<Image>().sprite = shopItemInfo.icon;
        transform.Find("CountText").GetComponent<Text>().text = item.count.ToString();
    }
}
