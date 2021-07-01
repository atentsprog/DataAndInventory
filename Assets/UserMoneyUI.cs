using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserMoneyUI : MonoBehaviour
{
    public static UserMoneyUI instance;

    Text diaText;
    Text goldText;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        diaText = transform.Find("Dia/Text").GetComponent<Text>();
        goldText = transform.Find("Gold/Text").GetComponent<Text>();
    }

    public void Refresh()
    {
        goldText.text = "gold:" + UserData.instance.gold.ToNumber();
        diaText.text = "dia:" + UserData.instance.dia.ToNumber();
    }
}
