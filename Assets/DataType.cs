﻿using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
[FirestoreData]
public sealed class InventoryItemCloud
{
    [SerializeField] int itemUID;
    [SerializeField] private int count;
    [SerializeField] private DateTime getDate;

    [FirestoreProperty]
    public int ItemUID { get { return itemUID; } set { itemUID = value; } }
    [FirestoreProperty]
    public int Count { get => count; set => count = value; }
    [FirestoreProperty]
    public DateTime GetDate { get => getDate; set => getDate = value; }

    public override bool Equals(object obj)
    {
        if (!(obj is InventoryItemCloud))
        {
            return false;
        }

        InventoryItemCloud other = (InventoryItemCloud)obj;
        return ItemUID == other.ItemUID;
    }
    public override int GetHashCode()
    {
        return ItemUID;
    }


    public override string ToString()
    {
        return $"ItemUID:{ItemUID}, Count:{Count}, GetDate:{GetDate}";
    }
}

[System.Serializable]
[FirestoreData]
public class CustomUser
{
    [SerializeField] private int gold;
    [SerializeField] private int userUID;
    [SerializeField] private int dia;
    [SerializeField] private string name;
    [SerializeField] private List<InventoryItemCloud> inventoryItems;

    public CustomUser()
    {
        InventoryItems = new List<InventoryItemCloud>();
    }
    [FirestoreProperty]
    public int UserUID { get => userUID; set => userUID = value; }


    [FirestoreProperty]
    public int Gold { get => gold; set => gold = value; }
    [FirestoreProperty]
    public int Dia { get => dia; set => dia = value; }
    [FirestoreProperty] public string Name { get => name; set => name = value; }

    [FirestoreProperty]
    public List<InventoryItemCloud> InventoryItems { get => inventoryItems; set => inventoryItems = value; }

    public override bool Equals(object obj)
    {
        if (!(obj is CustomUser))
        {
            return false;
        }

        CustomUser other = (CustomUser)obj;
        return UserUID == other.UserUID;
    }

    public override int GetHashCode()
    {
        return UserUID;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"{UserUID}, {Name} Gold:{Gold}, Dia{Dia}, InventoryItem Count:{InventoryItems.Count}");
        foreach (var item in InventoryItems)
            sb.AppendLine(item.ToString());

        return sb.ToString();
    }
}