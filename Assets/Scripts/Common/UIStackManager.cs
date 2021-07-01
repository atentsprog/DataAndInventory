
using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// esc누르면 UI닫는다
/// </summary>
public class UIStackManager : SingletonMonoBehavior<UIStackManager>
{
    public class UICloseInfoStack
    {
        internal List<UICloseInfo> previousHistory = new List<UICloseInfo>();

        public void Push(UICloseInfo newHistory)
        {
            previousHistory.Add(newHistory);
        }

        public bool Pop()
        {
            bool succeedCloseUI = false;
            while (succeedCloseUI == false && previousHistory.Count > 0)
            {
                int excuteIndex = previousHistory.Count - 1;
                UICloseInfo uICloser = previousHistory[excuteIndex];
                succeedCloseUI = uICloser.CloseUI();
                if (succeedCloseUI == false)
                    previousHistory.RemoveAt(excuteIndex);
            }
            return succeedCloseUI;
        }

        public void Remove(int instanceID)
        {
            previousHistory.Remove(previousHistory.Find(p => p.instanceID == instanceID));
        }

        public int Count
        {
            get { return previousHistory.Count; }
        }
    }


    public class UICloseInfo
    {
        public UICloseInfo(Transform tr, Action closeFn)
        {
            this.tr = tr;
            this.instanceID = tr.gameObject.GetInstanceID();
            this.closeFn = closeFn;
        }

        public int instanceID;
        public Transform tr;
        public Action closeFn;

        /// <summary>
        ///
        /// </summary>
        /// <returns>창닫은게 있으면 true, 없으면 false</returns>
        public bool CloseUI()
        {
            //if (closeFn != null)
            //    return closeFn();

            if (tr != null && tr.gameObject.activeInHierarchy == true)
            {
                if (closeFn != null)
                {
                    closeFn();
                }
                else
                {
                    tr.gameObject.SetActive(false);
                }
                return true;
            }

            return false;
        }
    }

    private static UICloseInfoStack uICloserStack = new UICloseInfoStack();

    override protected void Awake()
    {
        if (IsInitInstance)
        {
            Destroy(gameObject);
                
            Debug.LogError($"{typeof(UIStackManager)} : 이미 초기화되었습니다, 불필요한 생성입니다 {transform.GetPath()} ", this);
            return;
        }
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MoveBack();     //Escape 키를 눌렀을때.
        }
    }

    internal static void PushUiStack(Transform tr, Action ac = null)
    {
        UICloseInfo uICloser = new UICloseInfo(tr, ac);
        uICloserStack.Push(uICloser);
    }

    static public Action<int> OnExecutePopUiStackCallBack;

    internal static void PopUiStack(int instanceID)
    {
        uICloserStack.Remove(instanceID);

        if (OnExecutePopUiStackCallBack != null)
            OnExecutePopUiStackCallBack(uICloserStack.Count);
    }

    /// <summary>
    /// 가능하면 여기에 로직 추가하지 말고 UI에 esc누르면 특정 행동하도록 함수를 설정해주세요.
    /// </summary>
    public void MoveBack()
    {
        // UI닫기
        if (uICloserStack.Count > 0)
        {
            //창닫기 명령 실행.
            if (uICloserStack.Pop())   //닫은창이 있으면 true;
                return;
        }

#if UNITY_EDITOR
        //뒤로갈께 없으니 게임을 종료할것인지 묻자.
        Debug.Log("뒤로갈께 없으니 게임을 종료할것인지 물어보자. Android");
#endif
    }
}