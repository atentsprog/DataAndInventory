﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI instance;
    Text itemName;
    Image icon;
    Text description;
    Button button;

    private void Awake()
    {
        instance = this;
    }
    public ShopItemInfo shopItemInfo;
    internal void ShowShopItem(ShopItemInfo shopItemInfo)
    {
        shopBtn.SetActive(true);
        this.shopItemInfo = shopItemInfo;
        itemName.text = shopItemInfo.name;
        icon.sprite = shopItemInfo.icon;
        description.text = shopItemInfo.description;
    }

    public GameObject shopBtn;
    public GameObject inventoryBtn;
    void Start()
    {
        shopBtn = transform.Find("ShopBtn").gameObject;
        inventoryBtn = transform.Find("InventoryBtn").gameObject;
        shopBtn.SetActive(false);
        inventoryBtn.SetActive(false);

        shopBtn.transform.Find("Button").GetComponent<Button>()
            .AddListener(this, ItemBuy);

        inventoryBtn.transform.Find("Button").GetComponent<Button>()
            .AddListener(this, ItemSell);
        

        itemName = transform.Find("Name").GetComponent<Text>();
        icon = transform.Find("Icon").GetComponent<Image>();
        description = transform.Find("Description").GetComponent<Text>();
        button = transform.Find("Button").GetComponent<Button>();
    }

    private void ItemSell()
    {
        print("ItemSell");
    }
    private void ItemBuy()
    {
        print("ItemBuy");
    }
}
