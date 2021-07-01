using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    static CoroutineManager instance;
    static CoroutineManager Instance
    {
        get {
            if (instance == null)
                instance = new GameObject(nameof(CoroutineManager), typeof(CoroutineManager)).GetComponent<CoroutineManager>();
            return instance;
        }
    }

    internal static void DelayCoroutine(float delayTime, Action action)
    {
        if (action == null)
            return;
        Instance.StartCoroutine(Instance.RunDelayCoroutineCo(delayTime, action));
    }

    private IEnumerator RunDelayCoroutineCo(float delayTime, Action action)
    {
        yield return new WaitForSeconds(delayTime);

        if(action!= null)
            action();
    }
}
