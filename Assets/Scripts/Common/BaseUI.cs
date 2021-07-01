//using MyColor.SceneTopUI;
using System;
using System.Collections.Generic;
using UnityEngine;


public class SingletonBase : MonoBehaviour
{
    static protected List<SingletonBase> MenuHistory = new List<SingletonBase>();

    public virtual int SortOrder => 0;

    public virtual string HierarchyPath 
    {
        get
        {
            string canvasName = "Canvas";
            if (SortOrder > 0)
            {
                canvasName += SortOrder;
            }

            return $"{canvasName}/" +GetType().ShortName();
        } 
    }

    virtual public void Show() { }

    [HideInInspector]
    public bool completeUiInite = false;
    virtual public void ExecuteOneTimeInit() { }
}


/// <summary>
/// esc키를 누르면 창이 닫힌다. Ese키 눌렀을때 창이 닫히지 않는 UI는 SingletonMonoBehavior를 상속받자
/// Show() : UI 보여주는 함수
/// Close() : UI 닫는 함수
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseUI<T> : SingletonMonoBehavior<T>
where T : SingletonBase
{
    protected bool AllowBackAction => true;


    public override void Show()
    {
        base.Show();

    }

    protected void OnEnable()
    {
        UIStackManager.PushUiStack(transform, CloseCallback);

        AddHistory();
    }


    private void AddHistory()
    {
        MenuHistory.Add(this);
        if (MenuHistory.Count > MaxHistoryCount)
            MenuHistory.RemoveAt(0);
    }

    override protected void OnDisable()
    {
        base.OnDisable();

        UIStackManager.PopUiStack(CacheGameObject.GetInstanceID());
    }

    private readonly int MaxHistoryCount = 5;

    protected void ShowPreviousMenu(BaseUI<T> exceptUI = null)
    {
        if (exceptUI != null) // exceptUI는 제외 menuHistory에서 제거하자.
            MenuHistory.RemoveAll(x => x == exceptUI);

        int lastIndex = MenuHistory.Count - 1;
        if (lastIndex == -1)
        {
            Debug.Log("보여줄 메뉴가 없다");
            return;
        }

        var previousMenu = MenuHistory[lastIndex];
        MenuHistory.RemoveAt(lastIndex);
        previousMenu.Show();
    }
}

/// <summary>
/// 싱글턴, 동적 로드 가능
/// OnInit : 1회성 초기화시 사용
/// Show : 나타나게 할때 호출한다
/// Close : 사라지게 할때 호출한다
/// OnShow : 나타날때 호출된다
/// 
/// IsInitInstance : 인스턴스 초기화 되었는지 확인.
/// IsActive : 초기화된 인스턴스가 하이어라키 상에서 활성화 중인지 확인(activeInHierarchy == true)
/// Disable될때 부셔야 하는건 childObject에 추가하면 부셔진다.
/// </summary>
public class SingletonMonoBehavior<T> : SingletonBase
where T : SingletonBase
{
    static bool applicationQuit = false;
    private void OnApplicationQuit() => applicationQuit = true;
    
    static protected T m_instance;
    static public T Instance
    {
        get
        {
            
            if (m_instance == null)
            {
                if (applicationQuit)
                    return null;
                SetInstance(Util.InstantiateSingleton<T>());
                
                //m_instance.gameObject.SetActive(false); // 여기서 꺼버리면 Start에서 코루틴호출하는게 꺼진다 -> 강제로 끄면 안된다
            }

            return m_instance;
        }
    }

    private static void SetInstance(Util.TempRectInfo<T> _instance)
    {
        string originalPath = _instance.t.HierarchyPath;

        // 부모가 없으면 리소스 폴더에서 로드 하자 리소스 폴더에도 없다면 만들자.(CreateHierarchy)
        string rootParentPath = GetRootParentPath(originalPath);
        if (string.IsNullOrEmpty(rootParentPath) == false)
        {
            string parentFullpaht = GetParentFullPath(originalPath);
            Transform existParent = GetExistHierarchy(parentFullpaht);

            if (existParent == null)
            {
                existParent = CreateHierarchy(parentFullpaht);
            }

            if (existParent)
            {
                _instance.t.transform.SetParent(existParent);
                var rt = _instance.t.GetComponent<RectTransform>();
                if (_instance.rt != null)
                {
                    rt.anchoredPosition3D = _instance.rt.anchoredPosition3D;
                    rt.offsetMin = _instance.rt.offsetMin;
                    rt.offsetMax = _instance.rt.offsetMax;
                    rt.pivot = _instance.rt.pivot;

                    rt.anchorMin = _instance.rt.anchorMin;
                    rt.anchorMax = _instance.rt.anchorMax;
                    rt.anchoredPosition = _instance.rt.anchoredPosition;
                    rt.sizeDelta = _instance.rt.sizeDelta;
                }
            }
        }

        m_instance = _instance.t;
        m_instance.name = GetUIName(originalPath);
        DontDestroyOnLoad(m_instance.gameObject.transform.root);

        //어웨이크에서 실행된 경우 여기서 ExecuteOneTimeInit 할 필요 없다.
        if(m_instance.completeUiInite == false)
            m_instance.ExecuteOneTimeInit();


        RectTransform rectTransform = m_instance.gameObject.GetComponent<RectTransform>();
        if (rectTransform)
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
        }

        /// 최초 '/' 앞에 있는 경로
        string GetRootParentPath(string _originalPath)
        {
            string parentGoName = null;
            int firstSlashIndex = _originalPath.IndexOf('/');
            if (firstSlashIndex > -1)
                parentGoName = _originalPath.Substring(0, firstSlashIndex);

            return parentGoName;
        }

        /// 자신(마지막'/' 이후 경로를 제외한 경로)을 제외한 경로 리턴.
        string GetParentFullPath(string _originalPath)
        {
            string parentFullPath = null;
            int lastSlashIndex = _originalPath.LastIndexOf('/');
            if (lastSlashIndex > -1)
                parentFullPath = _originalPath.Substring(0, lastSlashIndex);

            return parentFullPath;
        }

        Transform CreateHierarchy(string path)
        {
            //parentPath를 '/'로 분해한다.
            // 있는지 확인하고 없으면 만든다.
            //있다면 현재 트랜스폼으로 지정.

            string[] paths = path.Split('/');
            if (paths.Length < 1) // 만들께 없다.
            {
                var go = GameObject.Find(path);
                if (go == null)
                    go = new GameObject(path);
                return go.transform;
            }

            // 첫번째 아이템은 GameObject.Find로 씬에 있는걸 찾는다.
            string rootPath = paths[0];
            GameObject rootGo = GameObject.Find(rootPath);

            // 씬에 없다면 리소스 폴더에 있는걸 생성.
            if (rootGo == null)
            {
                var obj = Resources.Load(rootPath, typeof(GameObject));
                rootGo = (GameObject)Instantiate(obj);
                rootGo.name = obj.name;
            }

            if (rootGo == null)
                rootGo = new GameObject(rootPath);

            Transform currentTr = rootGo.transform;
            for (int i = 1; i < paths.Length; i++)
            {
                var item = paths[i];
                Transform existTr = currentTr.Find(item)?? new GameObject(item).transform;
                existTr.SetParent(currentTr);
                currentTr = existTr;
            }

            return currentTr;
        }

        Transform GetExistHierarchy(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            // 루트 오브젝트를 찾는다.
            string[] paths = path.Split('/');
            GameObject rootGo = GameObject.Find(paths[0]);

            if (rootGo == null)
                return null;

            if (paths.Length == 1)
            {
                return rootGo.transform;
            }

            // 하위 패스 검사 결과를 리턴하자.
            int rootPathLength = path.IndexOf('/');
            string childPath = path.Substring(rootPathLength + 1, path.Length - (rootPathLength + 1));
            return rootGo.transform.Find(childPath);
        }

        string GetUIName(string _originalPath)
        {
            //originalPath 에서 마지막 '/' 뒤에 있는 경로를 리턴한다.
            string result = _originalPath;

            int lastSlashIndex = _originalPath.LastIndexOf('/');

            if (lastSlashIndex > 0)
                result = _originalPath.Substring(lastSlashIndex + 1, _originalPath.Length - lastSlashIndex - 1);

            return result;
        }
    }


    static public bool IsInitInstance
    {
        get { return m_instance != null; }
    }

    //instance로 처음 접근한게 아니라Awake로 켜진것이라면 인스턴스를 초기화 한다.
    virtual protected void Awake()
    {
        if (completeUiInite == false)
        {
            m_instance = GetComponent<T>();
            ExecuteOneTimeInit();
        }
    }

    private List<GameObject> childObject;

    protected List<GameObject> ChildObject
    {
        get
        {
            if (childObject == null)
                childObject = new List<GameObject>();
            return childObject;
        }
    }

    virtual protected void OnDestroy()
    {
        m_instance = null;
    }

    private void DestroyChildObject()
    {
        if (childObject == null)
            return;
        foreach (var item in childObject)
        {
            if (item == null)
                continue;
            Destroy(item);
        }
    }

    static public bool IsActive
    {
        get
        {
            return IsInitInstance && Instance.gameObject.activeInHierarchy == true;
        }
    }

    public override void Show()
    {
        Show();
    }

    public void Show(bool force = true)
    {
        CacheGameObject.SetActive(true);

        if (force)
        {
            Transform parent = CacheGameObject.transform.parent;
            do
            {
                if (parent == null)
                    break;
                parent.gameObject.SetActive(true);
                parent = parent.parent;
            } while (CacheGameObject.activeInHierarchy == false);
        }

        // 다른 UI들보다 먼저 보이도록 하이어라키 가장 아래로 내리자.
        CacheGameObject.transform.SetAsLastSibling();

        OnShow();
    }


    override public void ExecuteOneTimeInit()
    {
        Debug.Assert(completeUiInite == false, "ExecuteOneTimeInit 한번만 실행될꺼라 확신한다. 이 로그가 보인다면 로직 점검하자");
        if (completeUiInite)
            return;
        completeUiInite = true;


        OnInit();
    }

    /// <summary>
    /// 인슽턴스 초기화될때 최초 1회 실행되는것을 보장.
    /// </summary>
    virtual protected void OnInit()
    {
    }

    virtual protected void OnShow()
    {
    }

    private GameObject m_gameObject;

    public GameObject CacheGameObject
    {
        get
        {
            if (m_gameObject == null)
                m_gameObject = gameObject;
            return m_gameObject; //아래와 같은 의미
            //return m_gameObject??(m_gameObject = gameObject); //아래와 같은 의미
            //return m_gameObject ??= gameObject;
        }
    }

    /// <summary>
    /// 명확히 끄는 함수, gameObject.SetActive(false)는 UI를 끄는건지 잠시 비활성화하는 건지 알 수 없다. 그래서 명확하게 하기 위해서 Close함수를 호출해서 끌때 처리해야하는 로직을 실행하자(켤때도 마찬가지 Show강제 사용)
    /// ESC눌렀을때도 호출된다.
    /// </summary>
    virtual public void Close()
    {
        if (CacheGameObject)
            CacheGameObject.SetActive(false);
    }

    protected void CloseCallback()
    {
        Close();
    }


    virtual protected void OnDisable()
    {
        DestroyChildObject();
    }
}