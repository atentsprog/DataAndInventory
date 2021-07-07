using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FirestoreManager))]
public class FirestoreData : MonoBehaviour
{
    public static FirestoreData instance;
    FirestoreManager firestoreManager;
    private void Awake()
    {
        instance = this;
        firestoreManager = GetComponent<FirestoreManager>();
    }
    internal static void SaveToUserCloud(string _collectionPath, Dictionary<string, object> data = null)
    {
        instance.firestoreManager.SaveToUserCloud(_collectionPath, null, data);
    }

    internal static void LoadFromUserCloud(string _collectionPath, Action<IDictionary<string, object>> ac = null)
    {
        instance.firestoreManager.LoadFromUserCloud(_collectionPath, null, ac);
    }

    internal static void SaveToUserCloud(string _collectionPath, string subDocPath = null, Dictionary<string, object> data = null)
    {
        instance.firestoreManager.SaveToUserCloud(_collectionPath, subDocPath, data);
    }

    internal static void SaveToUserCloud(string _collectionPath, string key, object value)
    {
        var saveData = new Dictionary<string, object>();
        saveData[key] = value;
        SaveToUserCloud(_collectionPath, saveData);
    }

    internal static void LoadFromUserCloud(string _collectionPath, string subDocPath = null, Action<IDictionary<string, object>> ac = null)
    {
        instance.firestoreManager.LoadFromUserCloud(_collectionPath, subDocPath, ac);
    }

    internal static void SaveToCloud(string docFullPath, Dictionary<string, object> data)
    {
        instance.firestoreManager.SaveToCloud(docFullPath, data);
    }

    internal static void LoadFromCloud(string docFullPath, Action<IDictionary<string, object>> ac)
    {
        instance.firestoreManager.LoadFromCloud(docFullPath, ac);
    }
}
