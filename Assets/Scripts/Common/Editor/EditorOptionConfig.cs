using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class EditorOptionConfig : EditorWindow
{
    [MenuItem("Tools/Option")]
    static void Init()
    {
        EditorOptionConfig editorOptionConfig = (EditorOptionConfig)GetWindow(typeof(EditorOptionConfig));
        editorOptionConfig.Load();
    }

    private void Load()
    {
        GetUserGold();
    }

    private void GetUserGold()
    {
        goldText = PlayerPrefs.GetInt("gold").ToString();
    }

    Vector2 mPos = Vector2.zero;
    private string goldText;

    void OnGUI()
    {
        mPos = GUILayout.BeginScrollView(mPos);

        GUILayout.BeginHorizontal();
        goldText = GUILayout.TextField(goldText);
        if (GUILayout.Button("Set Gold"))
        {
            UserData.SetGold(int.Parse(goldText));
        }
        if (GUILayout.Button("↻"))
        {
            GetUserGold();
        }
        GUILayout.EndHorizontal();

        for (OptionType i = OptionType.StartIndex + 1; i < OptionType.LastIndex; i++)
        {
            GUILayout.BeginHorizontal();
            {
                bool tempBool = EditorOption.Options[i];

                EditorOption.Options[i] = GUILayout.Toggle(EditorOption.Options[i], i.ToString());

                if (tempBool != EditorOption.Options[i])
                {
                    string key = "DevOption_" + i;
                    EditorPrefs.SetInt(key, EditorOption.Options[i] == true ? 1 : 0);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}