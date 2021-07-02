using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    public HaveItemInfo haveItemInfo;


    public void OnPointerClick(PointerEventData eventData)
    {
        //print("click: " + eventData);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 우클릭이면 장착 하자.
            EquipUI.instance.SetEquipItem(haveItemInfo);
        }
        else
        {
            ItemInfoUI.instance.ShowInventoryItemInfo(haveItemInfo);
        }
    }

    internal void Init(HaveItemInfo item)
    {
        haveItemInfo = item;
        Image image = transform.Find("Icon").GetComponent<Image>();
        ItemInfo itemInfo = ShopItemData.GetItem(haveItemInfo.id);
        if(itemInfo == null)
        {
            Debug.LogWarning($"{haveItemInfo.id} ID가 없습니다");
            return;
        }
        image.sprite = itemInfo.icon;
    }
}
