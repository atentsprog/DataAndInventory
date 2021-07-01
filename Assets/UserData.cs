using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItemInfo
{
    public int itemID;
    public int count;
    public string getDate; //획득한 시간.
}

public class UserData : MonoBehaviour
{
    public static UserData instance;

    public List<InventoryItemInfo> inventoryItems;


    public int gold;
    public int dia;
    private void Awake()
    {
        instance = this;

        Load();
    }
    private void OnDestroy()
    {
        Save();
    }    
    
    private void Load()
    {
        if (PlayerPrefs.HasKey("gold"))
        {
            gold = PlayerPrefs.GetInt("gold");
            dia = PlayerPrefs.GetInt("dia");

            int itemCount = PlayerPrefs.GetInt("inventoryItems.Count");
            for (int i = 0; i < itemCount; i++)
            {
                var loadItem = new InventoryItemInfo();
                loadItem.itemID = PlayerPrefs.GetInt("inventoryItems.itemID" + i);
                loadItem.count = PlayerPrefs.GetInt("inventoryItems.count" + i);
                loadItem.getDate = PlayerPrefs.GetString("inventoryItems.getDate" + i);
                inventoryItems.Add(loadItem);
            }
        }
        else
        {
            gold = 1100;
            dia = 120;
        }
    }
    private void Save()
    {
        PlayerPrefs.SetInt("inventoryItems.Count", inventoryItems.Count);
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            var saveItem = inventoryItems[i];
            PlayerPrefs.SetInt("inventoryItems.itemID" + i, saveItem.itemID);
            PlayerPrefs.SetInt("inventoryItems.count" + i, saveItem.count);
            PlayerPrefs.SetString("inventoryItems.getDate" + i, saveItem.getDate);
        }


        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.SetInt("dia", dia);
        PlayerPrefs.Save();
    }
}
