using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    internal static InventoryUI instance;
    private void Awake()
    {
        instance = this;
    }
    public InventoryItem itemBase;
    void Start()
    {
        Refresh();
    }

    public List<InventoryItem> inventoryItems;

    public void Refresh()
    {
        inventoryItems.ForEach(x => Destroy(x.gameObject));
        inventoryItems.Clear();

        var items = UserData.instance.userItems;
        var itemParent = itemBase.transform.parent;
        itemBase.gameObject.SetActive(true);
        foreach (var item in items)
        {
            InventoryItem newShopItem = Instantiate(itemBase, itemParent);
            newShopItem.Init(item);
            inventoryItems.Add(newShopItem);
        }
        itemBase.gameObject.SetActive(false);
    }

}
