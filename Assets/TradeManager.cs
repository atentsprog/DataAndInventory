using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        uid = PlayerPrefs.GetInt("TradeUID");
    }

    int uid;
    public HaveItemInfo BuyItem(ItemInfo itemInfo)
    {
        var newItem = new HaveItemInfo();
        newItem.uid = uid;
        newItem.id = itemInfo.ID;
        newItem.count = 1;
        newItem.getDate = System.DateTime.Now.ToString();

        uid++;
        PlayerPrefs.SetInt("TradeUID", uid);
        PlayerPrefs.Save();

        return newItem;
    }
}
