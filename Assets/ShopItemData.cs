using System.Collections;
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

    [ContextMenu("리소스폴더 JSON로드", false, -10000)]
    void Load()
    {
        MyGame.ItemData.Load();

        InitFromGoogleData();
    }

    private void InitFromGoogleData()
    {
        shopItems.Clear();
        foreach (var item in MyGame.ItemData.ItemDataList)
        {
            shopItems.Add(new ShopItemInfo(item));
        }
    }

    [ContextMenu("첫번째 항목 구글 시트에 저장(테스트)", false, -10000)]
    void SaveToGogleSheet()
    {
        UnityGoogleSheet.Load<MyGame.ItemData>();
        var firstItem = shopItems[0];
        var mapItem = MyGame.ItemData.ItemDataMap[firstItem.itemID];
        mapItem.description = firstItem.description;
        //Wirte 
        UnityGoogleSheet.Write(mapItem);
    }

    [ContextMenu("인스펙터에 있는 내용 구글에 적용(너무 느림, 개당 약2초 소요)")]
    void SaveToGoogleSheetAll()
    {
        UnityGoogleSheet.Load<MyGame.ItemData>();
        for (int i = 0; i < shopItems.Count; i++)
        {
            //0 / shopItems.Count
#if UNITY_EDITOR
            float percent = ((float)i / shopItems.Count);
            UnityEditor.EditorUtility.DisplayProgressBar("구글 에서 데이터 가져오는 중", (percent * 100).ToString() + "%", percent);
#endif
            var item = shopItems[i];

            MyGame.ItemData itemData = MyGame.ItemData.ItemDataMap[item.itemID];
            itemData.name = item.name;
            itemData.itemID = item.itemID;
            itemData.iconName = item.iconName;
            itemData.description = item.description;
            itemData.type = item.type;
            itemData.buyPrice = item.buyPrice;
            itemData.sellPrice = item.sellPrice;
            UnityGoogleSheet.Write(itemData);
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }

    [ContextMenu("구글시트에서 로드")]
    void LoadFromGoogleSheet()
    {
        MyGame.ItemData.LoadFromGoogle((list, map) => {
            foreach (var data in MyGame.ItemData.ItemDataList)
            {
                InitFromGoogleData();
            }
        }, true);
    }
}
