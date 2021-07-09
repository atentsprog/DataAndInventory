using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData instance;

    private void Awake()
    {
        instance = this;
    }
    public UserDataServer userDataServer;
    public bool isLoadComplete = false;

    private void Start()
    {    
        FirestoreManager.LoadFromUserCloud(UserInfo, (DocumentSnapshot ds ) =>
        {
            if (ds.TryGetValue("MyUserInfo", out userDataServer) == false)
            {
                //print("서버에 UserData가 없다. 초기값을 설정하자.");
                //userDataServer = new UserDataServer();
                userDataServer.Gold = 1000;
                userDataServer.Dia = 10;
                userDataServer.InventoryItems = new List<InventoryItemServer>();
            }
            else
            {
                foreach (var item in userDataServer.InventoryItems)
                {
                    item.GetDate = item.GetDate.AddHours(9);
                }
            }

            if (userDataServer.InventoryItems == null)
                userDataServer.InventoryItems = new List<InventoryItemServer>();

            isLoadComplete = true;
            InventoryUI.instance.RefreshUI();
            MoneyUI.instance.RefreshUI();
        });
    }


    [ContextMenu("SaveUserData")]
    private void Save()
    {
        userDataServer = new UserDataServer();
        userDataServer.Gold = 1;
        userDataServer.Dia = 2;
        userDataServer.InventoryItems = new List<InventoryItemServer>();
        userDataServer.InventoryItems.Add(new InventoryItemServer()
        {
            ID = 1,
            UID = 1,
            Count = 1,
            GetDate = DateTime.Now.AddDays(-7)
        });
        userDataServer.InventoryItems.Add(new InventoryItemServer()
        {
            ID = 1,
            UID = 2,
            Count = 4,
            GetDate = DateTime.Now
        });
        //Dictionary<string, object> dic = new Dictionary<string, object>();
        //dic["MyUserInfo"] = userDataServer;
        //FirestoreData.SaveToUserCloud("UserInfo", dic);
        FirestoreManager.SaveToUserServer(UserInfo, ("MyUserInfo", userDataServer));
    }
    const string UserInfo = "UserInfo";

    [ContextMenu("변수 2개 저장")]
    private void Save2Variables()
    {
        FirestoreManager.SaveToUserServer(UserInfo, ("Key1", "Value1"), ("Key2", 1));
    }

    internal void SellItem(int sellPrice, InventoryItemServer inventoryItemInfo)
    {
        userDataServer.Gold += sellPrice;
        userDataServer.InventoryItems.Remove(inventoryItemInfo);
        // 서버에 에서 삭제하자.
        FirestoreManager.SaveToUserServer(UserInfo, ("MyUserInfo", userDataServer));
    }
    internal void ItemBuy(int buyPrice, InventoryItemServer newItem)
    {
        userDataServer.Gold -= buyPrice;
        userDataServer.InventoryItems.Add(newItem);
        //// 서버에에서 추가하자.
        FirestoreManager.SaveToUserServer(UserInfo, ("MyUserInfo", userDataServer));
    }

    #region 파이어 베이스 쿼리 테스트
    protected FirebaseFirestore db
    {
        get
        {
            return FirebaseFirestore.DefaultInstance;
        }
    }

    [ContextMenu("삭제테스트")]
    void DeleteTemp()
    {
        DocumentReference cityRef = FirebaseFirestore.DefaultInstance.Document("UserInfo/" +
            FirestoreManager.instance.userID
            );

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "Key1", FieldValue.Delete }
        };
        cityRef.UpdateAsync(updates);
    }


    [ContextMenu("임시 도시 데이터 추가")]
    void InsertTextData()
    {
        CollectionReference citiesRef = db.Collection("cities");
        citiesRef.Document("SF").SetAsync(new Dictionary<string, object>(){
    { "Name", "San Francisco" },
    { "State", "CA" },
    { "Country", "USA" },
    { "Capital", false },
    { "Population", 860000 },
    { "Regions", new ArrayList{"west_coast", "norcal"} }
});
        citiesRef.Document("LA").SetAsync(new Dictionary<string, object>(){
    { "Name", "Los Angeles" },
    { "State", "CA" },
    { "Country", "USA" },
    { "Capital", false },
    { "Population", 3900000 },
    { "Regions", new ArrayList{"west_coast", "socal"} }
});
        citiesRef.Document("DC").SetAsync(new Dictionary<string, object>(){
    { "Name", "Washington D.C." },
    { "State", null },
    { "Country", "USA" },
    { "Capital", true },
    { "Population", 680000 },
    { "Regions", new ArrayList{"east_coast"} }
});
        citiesRef.Document("TOK").SetAsync(new Dictionary<string, object>(){
    { "Name", "Tokyo" },
    { "State", null },
    { "Country", "Japan" },
    { "Capital", true },
    { "Population", 9000000 },
    { "Regions", new ArrayList{"kanto", "honshu"} }
});
        citiesRef.Document("BJ").SetAsync(new Dictionary<string, object>(){
    { "Name", "Beijing" },
    { "State", null },
    { "Country", "China" },
    { "Capital", true },
    { "Population", 21500000 },
    { "Regions", new ArrayList{"jingjinji", "hebei"} }
});
    }

    [ContextMenu("CA인 모든 도시를 반환")]
    void Test1()
    {
        CollectionReference citiesRef = db.Collection("cities");
        Query query = citiesRef.WhereEqualTo("State", "CA");
        query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
            {
                Debug.Log(String.Format("Document {0} returned by query State=CA", documentSnapshot.Id));
            }
        });
    }

    [ContextMenu("모든 수도를 반환")]
    void Test2()
    {
        CollectionReference citiesRef = db.Collection("cities");
        Query query = citiesRef.WhereEqualTo("Capital", true);
        query.GetSnapshotAsync().ContinueWithOnMainThread((querySnapshotTask) =>
        {
            foreach (DocumentSnapshot documentSnapshot in querySnapshotTask.Result.Documents)
            {
                Debug.Log(String.Format("Document {0} returned by query Capital=true", documentSnapshot.Id));
            }
        });
    }

    [ContextMenu("쿼리 객체를 만든 후 get() 함수를 사용하여 결과를 검색")]
    void Test3()
    {
        Query capitalQuery = db.Collection("cities").WhereEqualTo("Capital", true);
        capitalQuery.GetSnapshotAsync().ContinueWithOnMainThread(task => {
            QuerySnapshot capitalQuerySnapshot = task.Result;
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                Debug.Log(String.Format("Document data for {0} document:", documentSnapshot.Id));
                Dictionary<string, object> city = documentSnapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in city)
                {
                    Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                }

                // Newline to separate entries
                Debug.Log("");
            };
        });
    }
    #endregion 파이어 베이스 쿼리 테스트

}
[System.Serializable]
[FirestoreData]
public sealed class UserDataServer
{
    [SerializeField] private int gold;
    [SerializeField] private int dia;
    [SerializeField] private string name;
    [SerializeField] private int iD;
    [SerializeField] private List<InventoryItemServer> inventoryItems;

    [FirestoreProperty] public int Gold { get { return gold; } set {
            gold = value;
            //if(MoneyUI.instance != null)
            //    MoneyUI.instance.RefreshUI();// < --이 로직 있으면 서버에서 값 가져올때 에러 발생한다
        } }

    [FirestoreProperty] public int Dia { get => dia; set
        {
            dia = value;

            //if (MoneyUI.instance != null)
            //    MoneyUI.instance.RefreshUI();//< --이 로직 있으면 서버에서 값 가져올때 에러 발생한다
        }
    }
    [FirestoreProperty] public string Name { get => name; set => name = value; }
    [FirestoreProperty] public int ID { get => iD; set => iD = value; }
    [FirestoreProperty]
    public List<InventoryItemServer> InventoryItems { get => inventoryItems; set => inventoryItems = value; }
}

//[System.Serializable]
//public class InventoryItemServer
//{
//    public int itemID;
//    public int count;
//    public string getDate; //획득한 시간.

//}

[System.Serializable]
[FirestoreData]
public sealed class InventoryItemServer
{
    [SerializeField] private int uID;
    [SerializeField] private int iD;
    [SerializeField] private int count;
    [SerializeField] private int enchant;
    [SerializeField] private string getDate;


    [FirestoreProperty] public int UID { get => uID; set => uID = value; }
    [FirestoreProperty] public int ID { get => iD; set => iD = value; }
    [FirestoreProperty] public int Count { get => count; set => count = value; }
    [FirestoreProperty] public int Enchant { get => enchant; set => enchant = value; }
    [FirestoreProperty] public DateTime GetDate { get => DateTime.Parse(getDate); set => getDate = value.ToString(); }

    public override bool Equals(object obj)
    {
        if (!(obj is InventoryItemServer))
        {
            return false;
        }

        InventoryItemServer other = (InventoryItemServer)obj;
        return UID == other.UID;
    }

    public override int GetHashCode()
    {
        return UID;
    }

    internal ShopItemInfo GetShopItemInfo()
    {
        return ShopItemData.instance.shopItems.Find(x => x.itemID == ID);
    }
}