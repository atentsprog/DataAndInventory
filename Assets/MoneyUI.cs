using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    Text goldText;
    Text diaText;

    void Start()
    {
        goldText = transform.Find("Gold/Text").GetComponent<Text>();
        diaText = transform.Find("Dia/Text").GetComponent<Text>();
        goldText.text = UserData.instance.gold.ToString();
        diaText.text = UserData.instance.dia.ToString();
    }
}
