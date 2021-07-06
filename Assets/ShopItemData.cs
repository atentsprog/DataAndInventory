﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZG.Core.Type;

[UGS(typeof(MoneyType))]
public enum MoneyType
{
    Gold,
    Dia
}
[UGS(typeof(Grade))]
public enum Grade
{
    Normal,
    Rare,
    Epic,
    Legend
}
[UGS(typeof(ItemType))]
public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Etc,
}

[System.Serializable]
public class ShopItemInfo
{
    // 이름, 아이콘, 가격, 
    public string name;
    public int itemID;
    //public Sprite icon;
    public string iconName;
    public string       description;
    public ItemType     type;
    public int          buyPrice;
    public int          sellPrice;

    public ShopItemInfo(MyGame.ItemData item)
    {
        name = item.name;
        itemID = item.itemID;
        iconName = item.iconName;
        description = item.description;
        type = item.type;
        buyPrice = item.buyPrice;
        sellPrice = item.sellPrice;
    }

    public Sprite Icon
    {
        get { return Resources.Load<Sprite>(iconName); }
    }
}

public class ShopItemData : MonoBehaviour
{
    public static ShopItemData instance;
    private void Awake()
    {
        instance = this;
    }
    public List<ShopItemInfo> shopItems;

    [ContextMenu("Load Name2", false, -10000)]
    void Load()
    {
        ////Fist time must be need call load();
        //UnityGoogleSheet.Load<MyGame.ItemData>();  // or call DefaultTable.Data.Load(); it's same!

        MyGame.ItemData.Load();

        shopItems.Clear();
        foreach(var item in MyGame.ItemData.ItemDataList)
        {
            shopItems.Add(new ShopItemInfo(item));
        }
        //MyGame.ItemData.ItemDataMap
    }
}
