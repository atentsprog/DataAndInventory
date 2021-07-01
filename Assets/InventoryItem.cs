using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    internal void Init(InventoryItemInfo item)
    {
        ShopItemInfo shopItemInfo = ShopItemData.instance.shopItems.Find(x => x.itemID == item.itemID);
        
        GetComponent<Image>().sprite = shopItemInfo.icon;
        transform.Find("CountText").GetComponent<Text>().text = item.count.ToString();
    }
}
