using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI instance;
    private void Awake()
    {
        instance = this;
    }

    internal void ShowShopItemInfo(ItemInfo itemInfo)
    {
        // 아이템 아이콘 표시.
        // 아이템 설명 표시.
        // 아이템 이름 표시

    }
}
