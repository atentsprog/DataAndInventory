using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public ShopItem itemBase;
    void Start()
    {
        var items = ShopItemData.instance.shopItems;
        itemBase.gameObject.SetActive(true);
        foreach (ShopItemInfo item in items)
        {
            var newItem = Instantiate(itemBase, itemBase.transform.parent); // <- 정상
            newItem.Init(item);
        }
        itemBase.gameObject.SetActive(false);
    }
}
