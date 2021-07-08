using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
    const string UserDataCollection = "UserData"; // UserData콜렉션 이름.
    private void SaveGoldToCloud()
    {
        FirestoreManager.SaveToUserCloud(UserDataCollection, ("Gold", gold));
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
        FirestoreManager.LoadFromUserCloud(UserDataCollection, LoadCallBack);
    }

    private void LoadCallBack(DocumentSnapshot snapshop)
    {
        snapshop.TryGetValue("MyUserData", out CustomUser customUser);
        if (customUser == null)
        {
            customUser = new CustomUser();
            customUser.Gold = 1100;
            customUser.Dia = 10;
        }

        print(customUser);
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


    [ContextMenu("클래스 저장")]
    private void SaveClass()
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

        FirestoreManager.SaveToUserCloud(UserDataCollection, ("MyUserData", customUser));
    }
}
