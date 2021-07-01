using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData instance;

    public int gold;
    public int dia;
    private void Awake()
    {
        instance = this;

        Load();
    }
    private void OnDestroy()
    {
        Save();
    }    private void Load()
    {
        gold = PlayerPrefs.GetInt("gold");
        dia = PlayerPrefs.GetInt("dia");
    }
    private void Save()
    {
        PlayerPrefs.SetInt("gold", gold);
        PlayerPrefs.SetInt("dia", dia);
    }
}
