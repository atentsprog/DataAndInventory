using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    public static MoneyUI instance;
    private void Awake()
    {
        instance = this;
        goldText = transform.Find("Gold/Text").GetComponent<Text>();
        diaText = transform.Find("Dia/Text").GetComponent<Text>();
    }
    Text goldText;
    Text diaText;

    void Start()
    {
        RefreshUI();

    }

    public void RefreshUI()
    {
        //goldText.text = UserData.instance.Gold.ToString();
        //diaText.text = UserData.instance.Dia.ToString();
    }
}
