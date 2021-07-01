using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public ShopItem itemBase;
    void Start()
    {
        Init();
    }

    public List<ShopItem> shopItems;
    private void Init()
    {
        shopItems.ForEach(x => Destroy(x.gameObject));
        shopItems.Clear();

        var items = ShopItemData.instance.item;
        var itemParent = itemBase.transform.parent;
        itemBase.gameObject.SetActive(true);
        foreach (var item in items)
        {
            ShopItem newShopItem = Instantiate(itemBase, itemParent);
            newShopItem.Init(item);
            shopItems.Add(newShopItem);
        }
        itemBase.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
