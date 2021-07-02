using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillSlotItem : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData data)
    {
        transform.Find("Image").GetComponent<Image>().sprite = data.pointerClick.GetComponent<Image>().sprite;
    }
}
