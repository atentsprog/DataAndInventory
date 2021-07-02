using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotItem : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var shopItem = ShopItemData.GetItem(eventData.pointerClick.GetComponent<InventoryItem>().haveItemInfo.id);
        var sprite = shopItem.icon;
        transform.Find("Icon").GetComponent<Image>().sprite = sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
