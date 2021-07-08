using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    private void Awake()
    {
        instance = this;
    }
    public InventoryItem itemBase;


    List<GameObject> childItem = new List<GameObject>();
    internal void RefreshUI()
    {
        childItem.ForEach(x => Destroy(x));
        childItem.Clear();

        var items = UserData.instance.userDataServer.InventoryItems;
        itemBase.gameObject.SetActive(true);
        foreach (InventoryItemServer item in items)
        {
            var newItem = Instantiate(itemBase, itemBase.transform.parent); // <- 정상
            newItem.Init(item);
            childItem.Add(newItem.gameObject);
        }
        itemBase.gameObject.SetActive(false);
    }
}
