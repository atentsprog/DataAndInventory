using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HaveItemInfo
{
    public int uid;
    public int id;
    public int count;
    public string getDate;
}

public class UserData : MonoBehaviour
{
    public static UserData instance;
    private void Awake()
    {
        instance = this;
        DataLoad();
    }

    public int userID;
    public int gold;
    public int dia;

    public List<HaveItemInfo> userItems;

    private void OnDestroy()
    {
        DataSave();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            DataSave();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            DataLoad();
    }

    [ContextMenu("DataSave")]
    public void DataSave()
    {
        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.SetInt("dia", dia);

        PlayerPrefs.SetInt("haveItemCount", userItems.Count);

        for (int i = 0; i < userItems.Count; i++)
        {
            var item = userItems[i];
            PlayerPrefs.SetInt("uid" + i, item.uid);
            PlayerPrefs.SetInt("id" + i, item.id);
            PlayerPrefs.SetInt("count" + i, item.count);
            PlayerPrefs.SetString("getDate" + i, item.getDate);
        }

        //장착 정보 저장.
        foreach(var item in equipedItem)
        {
            PlayerPrefs.SetInt("Equiped" + item.Key, item.Value);
        }
        PlayerPrefs.Save();
    }

    public static Dictionary<ItemType, int> equipedItem = new Dictionary<ItemType, int>(); // < , ItemID>

    [ContextMenu("DataLoad")]
    public void DataLoad()
    {
        userItems.Clear();

        gold = PlayerPrefs.GetInt("gold");
        dia = PlayerPrefs.GetInt("dia");

        int haveItemCount = PlayerPrefs.GetInt("haveItemCount");
        for (int i = 0; i < haveItemCount; i++)
        {
            HaveItemInfo item = new HaveItemInfo();
            item.uid = PlayerPrefs.GetInt("uid" + i);
            item.id = PlayerPrefs.GetInt("id" + i);
            item.count = PlayerPrefs.GetInt("count" + i);
            item.getDate = PlayerPrefs.GetString("getDate" + i);
            userItems.Add(item);
        }

        equipedItem.Clear();
        var itemTypes = Enum.GetNames(typeof(ItemType));

        for (ItemType itemType = ItemType.StartIndex + 1; itemType < ItemType.EndIndex; itemType++)
        {
            string key = "Equiped" + itemType;
            if (PlayerPrefs.HasKey(key))
                equipedItem[itemType] = PlayerPrefs.GetInt(key);
        }
    }
    public static void SetGold(int gold)
    {
        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.Save();
        if (instance)
        {
            instance.gold = gold;
            UserMoneyUI.instance.Refresh();
        }
    }

    internal void DeleteItem(HaveItemInfo itemInfo)
    {
        userItems.Remove(itemInfo);
        DataSave();
    }
}
