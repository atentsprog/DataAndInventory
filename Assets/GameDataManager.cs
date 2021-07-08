using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public CustomUser userData;
    IEnumerator Start()
    {
        print("데이타 로드중...");

        Task task = LoadUserDataTimeCheck();

        yield return new WaitUntil(() => task.IsCompleted);

        print($"데이타 로드 완료!, {userData.Gold}");
    }

    [ContextMenu("유저 데이터 로드(시간체크)")]
    Task LoadUserDataTimeCheck()
    {
        float startTime = Time.realtimeSinceStartup;
        Task task = FirestoreManager.LoadFromUserCloud("UserData", ds =>
        {
            if (ds.TryGetValue("MyUserData", out userData) == false)
            {
                print("서버에 값 없어서 초기값 할당함");
                userData = new CustomUser();
                userData.Gold = 1000;
                userData.Dia = 10;
            }
            float timeTaken = Time.realtimeSinceStartup - startTime;
            print($"{timeTaken}초 소요됨");
            
            print(userData);
        });

        return task;
    }


    [ContextMenu("유저 데이터 로드")]
    void LoadUserData()
    {
        FirestoreManager.LoadFromUserCloud("UserData", ds =>
        {
            if (ds.TryGetValue("MyUserData", out userData) == false)
            {
                print("서버에 값 없어서 초기값 할당함");
                userData = new CustomUser();
                userData.Gold = 1000;
                userData.Dia = 10;
            }
            print(userData);
        });
    }

    [ContextMenu("유저 데이터 저장")]
    void SaveUserData()
    {
        FirestoreManager.SaveToUserCloud("UserData", ("MyUserData", userData));
    }

    [ContextMenu("변수 1개 불러오기")]
    private void LoadVarialble()
    {
        FirestoreManager.LoadFromUserCloud("UserData", ds =>
        {
            print(ds.GetValue<string>("TempVarialble"));
        });
    }

    [ContextMenu("변수 2개 불러오기")]
    private void LoadTwoVarialble()
    {
        FirestoreManager.LoadFromUserCloud("UserData", ds =>
        {
            print(ds.GetValue<string>("변수2개 저장 A"));
            print(ds.GetValue<string>("변수2개 저장 B"));
        });
    }

    [ContextMenu("변수 1개 저장")]
    private void SaveVarialble()
    {
        FirestoreManager.SaveToUserCloud("UserData", ("TempVarialble", "test"));
    }

    [ContextMenu("변수 2개 저장")]
    private void SaveTwoVarialble()
    {
        FirestoreManager.SaveToUserCloud("UserData", ("변수2개 저장 A", "A"), ("변수2개 저장 B", 11));
    }


    [ContextMenu("변수 서브 문서에 저장")]
    private void SaveSubDoc()
    {
        FirestoreManager.SaveToUserCloud("UserData", "SubCollection/SubDoc", ("TempVarialble", "test"));
    }

    [ContextMenu("변수 서브 문서에서 읽기")]
    private void LoadSubDoc()
    {
        FirestoreManager.LoadFromUserCloud("UserData", "SubCollection/SubDoc", 
            ss =>
            {
                 print(ss.GetValue<string>("TempVarialble"));
            }
        );
    }

}
