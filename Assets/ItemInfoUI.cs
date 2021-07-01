using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI instance;
    Text price;
    Text nameText;
    Image icon;
    Text description;
    Button sellItemBtn;
    Button buyItemBtn;

    private void Awake()
    {
        instance = this;

        price = transform.Find("Scroll View/Viewport/Content/IconAndInfo/Price").GetComponent<Text>();
        nameText = transform.Find("Scroll View/Viewport/Content/IconAndInfo/Name").GetComponent<Text>();
        icon = transform.Find("Scroll View/Viewport/Content/IconAndInfo/Icon").GetComponent<Image>();
        description = transform.Find("Scroll View/Viewport/Content/IconAndInfo/Description").GetComponent<Text>();


        sellItemBtn = transform.Find("Scroll View/Viewport/Content/Buttons/Button2").GetComponent<Button>();
        buyItemBtn = transform.Find("Scroll View/Viewport/Content/Buttons/Button1").GetComponent<Button>();
        buyItemBtn.AddListener(this, BuyItem);
        sellItemBtn.AddListener(this, SellItem);
        buyItemBtn.gameObject.SetActive(false);
        sellItemBtn.gameObject.SetActive(false);
    }


    private void SellItem()
    {
        UserData.instance.gold += itemInfo.sellPrice;
        UserData.instance.DeleteItem(haveItemInfo);

        UserMoneyUI.instance.Refresh();
        InventoryUI.instance.Refresh();
    }

    private void BuyItem()
    {
        UserData.instance.gold -= itemInfo.buyPrice;
        UserData.instance.userItems.Add(TradeManager.instance.BuyItem(itemInfo));

        UserMoneyUI.instance.Refresh();
        InventoryUI.instance.Refresh();
    }

    ItemInfo itemInfo;


    HaveItemInfo haveItemInfo;
    internal void ShowInventoryItemInfo(HaveItemInfo haveItemInfo)
    {
        this.haveItemInfo = haveItemInfo;
        sellItemBtn.gameObject.SetActive(true);
        buyItemBtn.gameObject.SetActive(false);

        var itemInfo = ShopItemData.GetItem(haveItemInfo.id);
        ShowItemInfo(itemInfo);
        price.text = $"판매 가격 : {itemInfo.sellPrice}";
    }

    internal void ShowShopItemInfo(ItemInfo itemInfo)
    {
        sellItemBtn.gameObject.SetActive(false);
        buyItemBtn.gameObject.SetActive(true);

        ShowItemInfo(itemInfo);
        price.text = $"구입 가격 : {itemInfo.buyPrice}";
    }

    private void ShowItemInfo(ItemInfo itemInfo)
    {
        this.itemInfo = itemInfo;
        // 아이템 아이콘 표시.
        icon.sprite = itemInfo.icon;
        // 아이템 설명 표시.
        description.text = itemInfo.description;
        // 아이템 이름 표시
        nameText.text = itemInfo.name;
    }
}
