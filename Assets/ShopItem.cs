using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerClickHandler
{
    ItemInfo itemInfo;

    public void OnPointerClick(PointerEventData eventData)
    {
        //print("click: " + eventData);
        ItemInfoUI.instance.ShowShopItemInfo(itemInfo);
    }

    internal void Init(ItemInfo item)
    {
        itemInfo = item;
        Image image = transform.Find("Icon").GetComponent<Image>();
        image.sprite = itemInfo.icon;
    }
}
