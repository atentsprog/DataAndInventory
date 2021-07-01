using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISample
{
    public class QuestUISample : BaseUI<QuestUISample>
    {
        new private void OnEnable()
        {
            base.OnEnable();
            ToastMessage.Instance.ShowToast("퀘스트 UI 열림");
        }

        new private void OnDisable()
        {
            base.OnDisable();
            if (applicationQuit)
                return;
            ToastMessage.Instance.ShowToast("퀘스트 UI 닫힘");
        }

        static bool applicationQuit = false;
        private void OnApplicationQuit() => applicationQuit = true;
    }
}