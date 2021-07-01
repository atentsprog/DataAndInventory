using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 시작시 UI들의 처음 액티브 상태를 설정한다. 
/// </summary>
public class UiFirstActiveManager : MonoBehaviour
{
    public List<GameObject> activeUIs = new List<GameObject>();
    public List<GameObject> inactiveUIs = new List<GameObject>();

    void Start()
    {
        activeUIs.ForEach(x => x.SetActive(true));
        inactiveUIs.ForEach(x => x.SetActive(false));
    }
}
