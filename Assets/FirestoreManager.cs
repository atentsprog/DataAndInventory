using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/////// Sample
/// - SAve
/// FirestoreManager.SaveToUserCloud("UserData", ("변수2개 저장 A", "A"), ("변수2개 저장 B", 11));
///// Load
/// FirestoreManager.LoadFromUserCloud("UserData", ds =>
/// {
///     print(ds.GetValue<string>("변수2개 저장 A"));
///     print(ds.GetValue<string>("변수2개 저장 B"));
/// });
/// </summary>
public class FirestoreManager : MonoBehaviour
{
    public static FirestoreManager instance;

    internal static Task LoadFromUserCloud(string collectionPath, string subPath, Action<DocumentSnapshot> p)
    {
        return instance._LoadFromUserCloud(collectionPath, p, subPath);
    }

    internal static Task LoadFromUserCloud(string collectionPath, Action<DocumentSnapshot> p)
    {
        return instance._LoadFromUserCloud(collectionPath, p);
    }

    internal static Task GetUserSnapshot(string collectionPath, string subPath, Action<DocumentSnapshot> p)
    {
        return instance._LoadFromUserCloud(collectionPath, p, subPath);
    }


    async Task _LoadFromUserCloud(string collectionPath, Action<DocumentSnapshot> p, string subPath = null)
    {
        if (string.IsNullOrEmpty(userID))
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Task.Delay(1000 / 60);
                    if (string.IsNullOrEmpty(userID) == false)
                        break;
                }
            });
        }

        string docPath = $"{collectionPath}/{userID}";
        if(string.IsNullOrEmpty(subPath) == false)
            docPath = $"{docPath}/{subPath}";

        await db.Document(docPath)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(ss =>
            {
                mainThreadFn.Add(() =>
               {
                   p(ss.Result);
               });
            });
    }

    private void Awake()
    {
        instance = this;
    }
    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected FirebaseAuth auth;
    protected Dictionary<string, FirebaseUser> userByAuth = new Dictionary<string, FirebaseUser>();
    public string userID; //firebaseUserID

    protected string ResultFromCloud
    {
        set { Debug.Log(value); }
    }
    List<Action> mainThreadFn = new List<Action>();
    private void Update()
    {
        try
        {
            foreach (var item in mainThreadFn)
                item();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            mainThreadFn.Clear();
        }
    }


    void Start()
    {
        // 파이어베이스 로그인
        CheckAndFixDependencThenInitializeFirebase();
    }


    public void LoadFromCloud(string docFullPath, Action<DocumentSnapshot> ac)
    {
        StartCoroutine(ReadDoc(db.Document(docFullPath), ac));
    }
    public void SaveToCloud(string docFullPath, params (string, object)[] p)
    {
        //Debug.Log(DictToString(data));
        StartCoroutine(WriteDoc(db.Document(docFullPath), p));
    }

    //public void LoadFromUserCloud(string _collectionPath, string subDocPath = null, Action<DocumentSnapshot> ac = null)
    //{
    //    if (IsExistLoginID() == false)
    //        return;

    //    string docPath = $"{_collectionPath}/{userID}";
    //    if (string.IsNullOrEmpty(docPath) == false)
    //        docPath += $"/{subDocPath}";
    //    LoadFromCloud(docPath, ac);
    //}

    static public void SaveToUserCloud(string _collectionPath, params (string, object)[] p)
    {
        instance._SaveToUserCloud(_collectionPath, p);
    }
    static public void SaveToUserCloud(string _collectionPath, string subDocPath = null, params (string, object)[] p)
    {
        instance._SaveToUserCloud(_collectionPath, subDocPath, p);
    }


    public void _SaveToUserCloud(string _collectionPath, params (string, object)[] p)
    {
        _SaveToUserCloud(_collectionPath, null, p);
    }

    public void _SaveToUserCloud(string _collectionPath, string subDocPath = null, params (string, object)[] p)
    {
        StartCoroutine(SaveToUserCloudCo(_collectionPath, subDocPath , p));
    }

    private IEnumerator SaveToUserCloudCo(string _collectionPath, string subDocPath, (string, object)[] p)
    {
        if(string.IsNullOrEmpty(userID))
        {
            Debug.Log("UserID가 설정되지 않았습니다. userID가 설정될때까지 대기합니다");
            while (string.IsNullOrEmpty(userID))
            {
                yield return null;
            }
        }

        string docPath = $"{_collectionPath}/{userID}";
        if (string.IsNullOrEmpty(docPath) == false)
            docPath += $"/{subDocPath}";

        SaveToCloud(docPath, p);
    }

    const string AsyncID = "AsyncID";

    private void SignInAnonymous()
    {
        if (PlayerPrefs.HasKey(AsyncID) == false) //익명 로그인한적이 없는가?
        {
            //앱 삭제나 로그아웃 하기전까지 유지된다.
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(HandleSignInWithUser);
        }
        else
        {
            // 익명 로그인 한적이 있으면 ID가 들어와야 한다. 3초 뒤에도 익명 로그인 안되면 로그인할지 물어보고 로그인 하자.
            LoginWhenBlankLogin();
        }
    }

    Coroutine checkBlankLoginIDHandle;
    private void LoginWhenBlankLogin()
    {
        if(checkBlankLoginIDHandle != null)
            StopCoroutine(checkBlankLoginIDHandle);
        checkBlankLoginIDHandle = StartCoroutine(CheckBlankLoginIDCo());
    }

    private IEnumerator CheckBlankLoginIDCo()
    {
        yield return new WaitForSeconds(3);
        if(string.IsNullOrEmpty(userID))
        {
            Debug.LogWarning("userID 가 없습니다. 네트워크 연결을 확인해주세요");
#if UNITY_EDITOR
            if(UnityEditor.EditorUtility.DisplayDialog("확인", "임시 아이디가 없습니다. 익명 로그인 하시겠습니까?", "확인", "취소"))
            {
                auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(HandleSignInWithUser);
            }
#endif
        }
    }

    protected FirebaseFirestore db
    {
        get
        {
            return FirebaseFirestore.DefaultInstance;
        }
    }

    // Cancellation token source for the current operation.
    protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    // Whether an operation is in progress.
    protected bool operationInProgress;
    protected Task previousTask;

    // Wait for task completion, throwing an exception if the task fails.
    // This could be typically implemented using
    // yield return new WaitUntil(() => task.IsCompleted);
    // however, since many procedures in this sample nest coroutines and we want any task exceptions
    // to be thrown from the top level coroutine (e.g GetKnownValue) we provide this
    // CustomYieldInstruction implementation wait for a task in the context of the coroutine using
    // common setup and tear down code.
    private class WaitForTaskCompletion : CustomYieldInstruction
    {
        private readonly Task task;
        private readonly FirestoreManager uiHandler;

        // Create an enumerator that waits for the specified task to complete.
        public WaitForTaskCompletion(FirestoreManager uiHandler, Task task)
        {
            uiHandler.previousTask = task;
            uiHandler.operationInProgress = true;
            this.uiHandler = uiHandler;
            this.task = task;
        }

        // Wait for the task to complete.
        public override bool keepWaiting
        {
            get
            {
                if (task.IsCompleted)
                {
                    uiHandler.operationInProgress = false;
                    uiHandler.cancellationTokenSource = new CancellationTokenSource();
                    if (task.IsFaulted)
                    {
                        string s = task.Exception.ToString();
                        uiHandler.DebugLog(s);
                    }
                    return false;
                }
                return true;
            }
        }
    }

    private IEnumerator WriteDoc(DocumentReference doc, params (string, object)[] p)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        foreach (var item in p)
            dic[item.Item1] = item.Item2;

        Task setTask = doc.UpdateAsync(dic);
        //WaitForTaskCompletion 클래스 없이 WaitUntil로 기다려도 되지만 예외 발생했을때 로그 확인하기 위해서 커스턴 yield클래스 사용.
        //yield return new WaitUntil(() => setTask.IsCompleted); 
        yield return new WaitForTaskCompletion(this, setTask);
        if (!(setTask.IsFaulted || setTask.IsCanceled))
        {
            ResultFromCloud = "WriteDoc Ok";
        }
        else
        {
            ResultFromCloud = "WriteDoc Error";

            ////아직 데이터 없는 경우 Update할 수 없다. set시도하자
            //Dictionary<string, object> docData = new Dictionary<string, object>(1);
            //docData[field] = data;

            StartCoroutine(SetDoc(doc, dic));
        }
    }
    private IEnumerator SetDoc(DocumentReference doc, object data)
    {
        Task setTask = doc.SetAsync(data);
        yield return new WaitForTaskCompletion(this, setTask);
        if (!(setTask.IsFaulted || setTask.IsCanceled))
        {
            ResultFromCloud = "WriteDoc Ok";
        }
        else
        {
            ResultFromCloud = "WriteDoc Error";
        }
    }

    private IEnumerator ReadDoc(DocumentReference doc, Action<DocumentSnapshot> ac)
    {
        Task<DocumentSnapshot> getTask = doc.GetSnapshotAsync();
        yield return new WaitForTaskCompletion(this, getTask);
        if (!(getTask.IsFaulted || getTask.IsCanceled))
        {
            DocumentSnapshot snap = getTask.Result;

            ac(snap);

            //IDictionary<string, object> resultData = snap.ToDictionary();
            //ac(resultData);
            ResultFromCloud = "Ok: ";
        }
        else
        {
            ResultFromCloud = "Error";
        }
    }

    public static string DictToString(IDictionary<string, object> d)
    {
        return "{ " + d
            .Select(kv => "(" + kv.Key + ", " + kv.Value + ")")
            .Aggregate("", (current, next) => current + next + ", ")
            + "}";
    }

    private void CheckAndFixDependencThenInitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    protected virtual void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;

        SignInAnonymous();
    }


    void HandleSignInWithUser(Task<FirebaseUser> task)
    {
        if (LogTaskCompletion(task, "Sign-in"))
        {
            print($"{task.Result.DisplayName} signed in :{userID = task.Result.UserId}");
            userID = task.Result.UserId;
            PlayerPrefs.SetString(AsyncID, userID);
            PlayerPrefs.Save();
        }
    }

    public void SignOut()
    {
        print($"{userID}를 로그아웃 합니다");

        auth.SignOut();
        PlayerPrefs.DeleteKey(AsyncID);
        PlayerPrefs.Save();
    }

    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugLog(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugLog(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                FirebaseException firebaseEx = exception as FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((AuthError)firebaseEx.ErrorCode).ToString());
                }
                DebugLog(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugLog(operation + " completed");
            complete = true;
        }
        return complete;
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                DebugLog("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                DebugLog("AuthStateChanged Signed in " + user.UserId);
                userID = user.UserId;
            }
        }
    }

    private bool fetchingToken = false;

    // Track ID token changes.
    private void IdTokenChanged(object sender, EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
              task =>
                  {
                      DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8)));
                      fetchingToken = true;
                  }
              );
        }
    }

    private void DebugLog(string log)
    {
        Debug.Log($"DebugLog:{log}");
    }

}
