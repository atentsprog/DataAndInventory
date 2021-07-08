﻿using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData instance;

    public int Gold { get; internal set; }

    public List<InventoryItemServer> inventoryItems;

    private void Awake()
    {
        instance = this;
    }
    public UserDataServer userDataServer;


    [ContextMenu("SaveUserData")]
    private void Save()
    {
        userDataServer = new UserDataServer();
        userDataServer.Gold = 1;
        userDataServer.Dia = 2;
        userDataServer.InventoryItems = new List<InventoryItemServer>();
        userDataServer.InventoryItems.Add(new InventoryItemServer()
        {
            ID = 1,
            UID = 1,
            Count = 1,
            GetDate = DateTime.Now.AddDays(-7)
        });
        userDataServer.InventoryItems.Add(new InventoryItemServer()
        {
            ID = 1,
            UID = 2,
            Count = 4,
            GetDate = DateTime.Now
        });
        //Dictionary<string, object> dic = new Dictionary<string, object>();
        //dic["MyUserInfo"] = userDataServer;
        //FirestoreData.SaveToUserCloud("UserInfo", dic);
        FirestoreManager.SaveToUserServer("UserInfo", ("MyUserInfo", userDataServer));
    }


    [ContextMenu("변수 2개 저장")]
    private void Save2Variables()
    {
        FirestoreManager.SaveToUserServer("UserInfo", ("Key1", "Value1"), ("Key2", 1));
    }

    internal void SellItem(int sellPrice, InventoryItemServer inventoryItemInfo)
    {
        userDataServer.Gold += sellPrice;
        userDataServer.InventoryItems.Remove(inventoryItemInfo);
        // 서버에 에서 삭제하자.
    }

    internal void ItemBuy(int buyPrice, InventoryItemServer newItem)
    {
        userDataServer.Gold -= buyPrice;
        userDataServer.InventoryItems.Add(newItem);
        //// 서버에에서 추가하자.
        //FirestoreManager.SaveToUserServer()
    }
}
[System.Serializable]
[FirestoreData]
public sealed class UserDataServer
{
    [SerializeField] private int gold;
    [SerializeField] private int dia;
    [SerializeField] private string name;
    [SerializeField] private int iD;
    [SerializeField] private List<InventoryItemServer> inventoryItems;

    [FirestoreProperty] public int Gold { get { return gold; } set { gold = value; } }

    [FirestoreProperty] public int Dia { get => dia; set => dia = value; }
    [FirestoreProperty] public string Name { get => name; set => name = value; }
    [FirestoreProperty] public int ID { get => iD; set => iD = value; }
    [FirestoreProperty]
    public List<InventoryItemServer> InventoryItems { get => inventoryItems; set => inventoryItems = value; }
}

//[System.Serializable]
//public class InventoryItemServer
//{
//    public int itemID;
//    public int count;
//    public string getDate; //획득한 시간.

//}

[System.Serializable]
[FirestoreData]
public sealed class InventoryItemServer
{
    [SerializeField] private int uID;
    [SerializeField] private int iD;
    [SerializeField] private int count;
    [SerializeField] private int enchant;
    [SerializeField] private string getDate;


    [FirestoreProperty] public int UID { get => uID; set => uID = value; }
    [FirestoreProperty] public int ID { get => iD; set => iD = value; }
    [FirestoreProperty] public int Count { get => count; set => count = value; }
    [FirestoreProperty] public int Enchant { get => enchant; set => enchant = value; }
    [FirestoreProperty] public DateTime GetDate { get => DateTime.Parse(getDate); set => getDate = value.ToString(); }

    public override bool Equals(object obj)
    {
        if (!(obj is InventoryItemServer))
        {
            return false;
        }

        InventoryItemServer other = (InventoryItemServer)obj;
        return UID == other.UID;
    }

    public override int GetHashCode()
    {
        return UID;
    }

    internal ShopItemInfo GetShopItemInfo()
    {
        return ShopItemData.instance.shopItems.Find(x => x.itemID == ID);
    }
}