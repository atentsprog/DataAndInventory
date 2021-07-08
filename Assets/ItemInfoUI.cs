using System;
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

    private void Awake()
    {
        instance = this;
    }
    public ShopItemInfo shopItemInfo;
    public InventoryItemServer inventoryItemInfo;
    internal void ShowInventoryItem(InventoryItemServer inventoryItemInfo)
    {
        this.inventoryItemInfo = inventoryItemInfo;
        shopBtn.SetActive(false);
        inventoryBtn.SetActive(true);

        var shopItemInfo = inventoryItemInfo.GetShopItemInfo();
        SetItemInfo(shopItemInfo);
    }
    private void ItemSell()
    {
        print("ItemSell");

        UserData.instance.SellItem(shopItemInfo.sellPrice, inventoryItemInfo);
        InventoryUI.instance.RefreshUI();
    }
    internal void ShowShopItem(ShopItemInfo shopItemInfo)
    {
        shopBtn.SetActive(true);
        inventoryBtn.SetActive(false);

        SetItemInfo(shopItemInfo);
    }

    private void SetItemInfo(ShopItemInfo shopItemInfo)
    {
        this.shopItemInfo = shopItemInfo;
        itemName.text = shopItemInfo.name;
        icon.sprite = shopItemInfo.Icon;
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
    }

    private void ItemBuy()
    {
        print("ItemBuy");

        var newItem = new InventoryItemServer();
        newItem.ID = shopItemInfo.itemID;
        newItem.Count = 1;
        newItem.GetDate = DateTime.Now;

        UserData.instance.ItemBuy(shopItemInfo.buyPrice, newItem);
        InventoryUI.instance.RefreshUI();


        //MoneyUI.instance.RefreshUI();
        //shopItemInfo
    }
}
