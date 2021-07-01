using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public ShopItemUI itemBase;
    void Start()
    {
        var items = ShopItemData.instance.shopItems;
        itemBase.gameObject.SetActive(true);
        foreach (ShopItemInfo item in items)
        {
            var newItem = Instantiate(itemBase, itemBase.transform.parent); // <- Á¤»ó
            newItem.Init(item);
        }
        itemBase.gameObject.SetActive(false);
    }
}
