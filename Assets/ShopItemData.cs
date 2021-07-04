using MyGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    Unique,
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
    public ShopItemInfo(ItemData data)
    {
        sellPrice = data.sellPrice;
        name = data.name;
        iconName = data.iconName;
        description = data.description;
        buyPrice = data.buyPrice;
        itemID = data.itemID;
        type = data.type;
    }

    public Sprite icon
    {
        get { return Resources.Load<Sprite>(iconName); }
    }

    public int      sellPrice;
    public string   name;
    public string   iconName;
    public string   description;
    public int      buyPrice;
    public int      itemID;
    public ItemType type;
    
}

//    // 이름, 아이콘, 가격, 
//    public string name;
//    public int itemID;
//    public Sprite icon;
//    public string description;
//    public ItemType type;
//    public int buyPrice;
//    public int sellPrice;
//}

public class ShopItemData : MonoBehaviour
{
    public static ShopItemData instance;
    public bool useCloudData;
    private void Awake()
    {
        instance = this;

        if (useCloudData)
        {
            LoadCloudData();
        }
    }

    public List<ShopItemInfo> shopItems;

    [ContextMenu("Load")]
    void LoadCloudData()
    {
        ItemData.Load();
        shopItems = ItemData.ItemDataList.Select(x => new ShopItemInfo(x)).ToList();
    }
}
