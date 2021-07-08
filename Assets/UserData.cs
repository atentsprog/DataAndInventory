using Firebase.Firestore;
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

    internal ShopItemInfo GetShopItemInfo()
    {
        return ShopItemData.instance.shopItems.Find(x => x.itemID == itemID);
    }
}

public class UserData : MonoBehaviour
{
    public static UserData instance;

    public List<InventoryItemInfo> inventoryItems;

    int gold;
    public int Gold
    {
        get { return gold; }
        set { gold = value;
            // 구글에 변경된 골드 저장.
            SaveGoldToCloud();
            MoneyUI.instance?.RefreshUI();
        }
    }

    private void SaveGoldToCloud()
    {
        FirestoreData.SaveToUserCloud("UserInfo", "Gold", gold);
    }

    public int dia;
    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        while (string.IsNullOrEmpty(FirestoreManager.instance.userID))
            yield return null;

        Load();
    }
    private void OnDestroy()
    {
        //Save();
    }    
    
    private void Load()
    {
        FirestoreData.LoadFromUserCloud("UserInfo", LoadCallBack);
        //Gold = 
        //if (PlayerPrefs.HasKey("gold"))
        //{
        //    //Gold = PlayerPrefs.GetInt("gold");
        //    dia = PlayerPrefs.GetInt("dia");

        //    int itemCount = PlayerPrefs.GetInt("inventoryItems.Count");
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        var loadItem = new InventoryItemInfo();
        //        loadItem.itemID = PlayerPrefs.GetInt("inventoryItems.itemID" + i);
        //        loadItem.count = PlayerPrefs.GetInt("inventoryItems.count" + i);
        //        loadItem.getDate = PlayerPrefs.GetString("inventoryItems.getDate" + i);
        //        inventoryItems.Add(loadItem);
        //    }
        //}
        //else
        //{
        //    Gold = 1100;
        //    dia = 120;
        //}
    }

    private void LoadCallBack(IDictionary<string, object> obj)
    {
        if (obj == null)
            return;

        if(obj.ContainsKey("Gold"))
            Gold = Convert.ToInt32(obj["Gold"]);
        else
            Gold = 1100;

        if (obj.ContainsKey("Dia"))
            dia = Convert.ToInt32(obj["Dia"]);
        else
            dia = 120;
    }

    public static void SetGold(int gold)
    {
        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.Save();
        if(instance)
        {
            instance.Gold = gold;
            //MoneyUI.instance.RefreshUI();
        }
    }

    [ContextMenu("SaveUserData")]
    private void Save()
    {
        CustomUser customUser = new CustomUser();
        customUser.Name = "a";
        customUser.Gold = 1;
        customUser.Dia = 2;
        customUser.InventoryItems = new List<InventoryItemCloud>();
        customUser.InventoryItems.Add(new InventoryItemCloud()
        {
            ItemUID = 1,
            Count = 2,
            GetDate = DateTime.Now
        });
        customUser.InventoryItems.Add(new InventoryItemCloud()
        {
            ItemUID = 2,
            Count = 3,
            GetDate = DateTime.Now
        });

        FirestoreData.SaveToUserCloud("UserData", "User1", customUser);

        //PlayerPrefs.SetInt("inventoryItems.Count", inventoryItems.Count);
        //for (int i = 0; i < inventoryItems.Count; i++)
        //{
        //    var saveItem = inventoryItems[i];
        //    PlayerPrefs.SetInt("inventoryItems.itemID" + i, saveItem.itemID);
        //    PlayerPrefs.SetInt("inventoryItems.count" + i, saveItem.count);
        //    PlayerPrefs.SetString("inventoryItems.getDate" + i, saveItem.getDate);
        //}
        

        //PlayerPrefs.SetInt("gold", Gold);
        //PlayerPrefs.SetInt("dia", dia);
        //PlayerPrefs.Save();
    }


}

[FirestoreData]
public sealed class InventoryItemCloud
{
    [FirestoreProperty]
    public int ItemUID { get; set; }
    [FirestoreProperty]
    public int Count { get; set; }
    [FirestoreProperty]
    public DateTime GetDate { get; set; }

}
[FirestoreData]
public sealed class CustomUser
{
    [FirestoreProperty]
    public int UserUID { get; set; }

    [FirestoreProperty]
    public int Gold { get; set; }
    [FirestoreProperty]
    public int Dia { get; set; }
    [FirestoreProperty] public string Name { get; set; }
    [FirestoreProperty] public List<InventoryItemCloud> InventoryItems { get; set; }

    public override bool Equals(object obj)
    {
        if (!(obj is CustomUser))
        {
            return false;
        }

        CustomUser other = (CustomUser)obj;
        return Gold == other.Gold && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}