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
        if (eventData.button == PointerEventData.InputButton.Left)
            ItemInfoUI.instance.ShowInventoryItem(inventoryItemInfo);
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //inventoryItemInfo 아이템을 장착하자. -> 장착정보.
            EquipUI.instance.SetEquipItem(inventoryItemInfo);
        }
    }

    internal void Init(InventoryItemInfo item)
    {
        inventoryItemInfo = item;
        ShopItemInfo shopItemInfo = item.GetShopItemInfo();
        
        GetComponent<Image>().sprite = shopItemInfo.Icon;
        transform.Find("CountText").GetComponent<Text>().text = item.count.ToString();
    }
}
