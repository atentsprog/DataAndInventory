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
using UnityEngine.UI;

public class FirestoreStudy : MonoBehaviour
{
    public DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected FirebaseAuth auth;
    protected FirebaseAuth otherAuth;
    protected Dictionary<string, FirebaseUser> userByAuth = new Dictionary<string, FirebaseUser>();

    string _userID;
    public string userID
    {
        get { return _userID; }
        set { _userID = value;
            userIDText.text = value;
        }
    }

    protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    protected bool operationInProgress;
    protected Task previousTask;

    private class WaitForTaskCompletion : CustomYieldInstruction
    {
        private Task task;
        private FirestoreStudy uiHandler;

        public WaitForTaskCompletion(FirestoreStudy uiHandler, Task task)
        {
            uiHandler.previousTask = task;
            uiHandler.operationInProgress = true;
            this.uiHandler = uiHandler;
            this.task = task;
        }

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


    Text requestResult;
    Text userIDText;

    Button updateButton;
    Button setButton;
    Button loginButton;
    Button checkConfigButton;
    Button checkAndConfigButton;

    void Start()
    {
        userIDText = transform.Find("Lables/UserIDText").GetComponent<Text>();
        requestResult = transform.Find("Lables/RequestResult").GetComponent<Text>();


        setButton = transform.Find("Buttons/SetButton").GetComponent<Button>();
        updateButton = transform.Find("Buttons/UpdateButton").GetComponent<Button>();

        checkConfigButton = transform.Find("Buttons/CheckConfigButton").GetComponent<Button>();
        loginButton = transform.Find("Buttons/LoginButton").GetComponent<Button>();
        checkAndConfigButton = transform.Find("Buttons/CheckAndConfigButton").GetComponent<Button>();

        checkConfigButton.AddListener(this, CheckAndFixDependenc);
        loginButton.AddListener(this, InitializeFirebaseAndLogin);
        checkAndConfigButton.AddListener(this, CheckAndFixDependencThenInitializeFirebase);

        setButton.AddListener(this, SetUpdate);
        updateButton.AddListener(this, UpdateData);
    }

    private void CheckAndFixDependenc()
    {
        CheckAndFixDependencThenInitializeFirebase(null);
    }

    private void CheckAndFixDependencThenInitializeFirebase()
    {
        CheckAndFixDependencThenInitializeFirebase(InitializeFirebaseAndLogin);
    }
    private void CheckAndFixDependencThenInitializeFirebase(Action ac)
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            requestResult.text = dependencyStatus.ToString();
            if (dependencyStatus != DependencyStatus.Available)
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
            else
            {
                ac();
            }
        });
    }


    // Options used to setup secondary authentication object.
    private AppOptions otherAuthOptions = new AppOptions
    {
        ApiKey = "",
        AppId = "",
        ProjectId = ""
    };
    protected virtual void InitializeFirebaseAndLogin()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;
        // Specify valid options to construct a secondary authentication object.
        if (otherAuthOptions != null &&
            !(string.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
              string.IsNullOrEmpty(otherAuthOptions.AppId) ||
              string.IsNullOrEmpty(otherAuthOptions.ProjectId)))
        {
            try
            {
                otherAuth = FirebaseAuth.GetAuth(FirebaseApp.Create(
                  otherAuthOptions, "Secondary"));
                otherAuth.StateChanged += AuthStateChanged;
                otherAuth.IdTokenChanged += IdTokenChanged;
            }
            catch (Exception)
            {
                DebugLog("ERROR: Failed to initialize secondary authentication object.");
            }
        }
    }
    private bool fetchingToken = false;
    private void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        FirebaseAuth senderAuth = sender as FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
              task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
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
    private void DebugLog(string log)
    {
        Debug.Log($"DebugLog:{log}");
    }

    Dictionary<string, object> data = new Dictionary<string, object>{
            {"gold", 100},
            {"jewel", 200},
            {"WriteUserIdDoc", 2},
            {"modify", DateTime.Now}
            };

    enum WriteMode
    {
        Update,
        Set
    }

    void SetUpdate()    {

        data = new Dictionary<string, object>{
            {"test", 100}
            };
        data[DateTime.Now.ToString()] = "Set 지금시간";
        StartCoroutine(WriteDoc(WriteMode.Set, data));
    }

    private void UpdateData()
    {
        data[DateTime.Now.ToString()] = "Update 지금시간";
        StartCoroutine(WriteDoc(WriteMode.Update, data));
    }

    private IEnumerator WriteDoc(WriteMode writeMode, IDictionary<string, object> data)
    {
        DocumentReference doc = db.Collection("UserInfo").Document(userID);
        Debug.Log(DictToString(data));
        //Task setTask = writeMode == WriteMode.Set ? doc.SetAsync(data) : doc.UpdateAsync(data);
        Task setTask = doc.SetAsync(data);
        yield return new WaitForTaskCompletion(this, setTask);
        if (!(setTask.IsFaulted || setTask.IsCanceled))
        {
            ResultText = "WriteDoc Ok";
        }
        else
        {
            ResultText = "WriteDoc Error";
        }
    }
   
    string ResultText
    {
        set { requestResult.text = value; }
    }

    protected FirebaseFirestore db
    {
        get
        {
            return FirebaseFirestore.DefaultInstance;
        }
    }

    private IEnumerator ReadDoc(DocumentReference doc)
    {
        Task<DocumentSnapshot> getTask = doc.GetSnapshotAsync();
        yield return new WaitForTaskCompletion(this, getTask);
        if (!(getTask.IsFaulted || getTask.IsCanceled))
        {
            DocumentSnapshot snap = getTask.Result;
            // TODO(rgowman): Handle `!snap.exists()` case.
            IDictionary<string, object> resultData = snap.ToDictionary();
            ResultText = "Ok: " + DictToString(resultData);
        }
        else
        {
            ResultText = "Error";
        }
    }
    private static string DictToString(IDictionary<string, object> d)
    {
        return "{ " + d
            .Select(kv => "(" + kv.Key + ", " + kv.Value + ")")
            .Aggregate("", (current, next) => current + next + ", ")
            + "}";
    }


}
