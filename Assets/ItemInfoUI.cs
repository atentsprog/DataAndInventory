using MyGame;
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
    public InventoryItemInfo inventoryItemInfo; // 지금 보여주고 있는 아이템을 삭제할 경우 사용
    internal void ShowInventoryItem(InventoryItemInfo inventoryItemInfo)
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

        UserData.instance.Gold += shopItemInfo.sellPrice;

        // UserData 싱글턴 클래스 접근해서 가지고 있는 아이템중에 보관하고 있던 inventoryItemInfo이 있으면 삭제
        UserData.instance.inventoryItems.Remove(inventoryItemInfo);

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
    }

    private void ItemBuy()
    {
        print("ItemBuy");

        UserData.instance.Gold -= shopItemInfo.buyPrice;
        var newItem = new InventoryItemInfo();
        newItem.itemID = shopItemInfo.itemID;
        newItem.count = 1;
        newItem.getDate = DateTime.Now.ToString();
        UserData.instance.inventoryItems.Add(newItem);
        InventoryUI.instance.RefreshUI();
        //MoneyUI.instance.RefreshUI();
        //shopItemInfo
    }
}
