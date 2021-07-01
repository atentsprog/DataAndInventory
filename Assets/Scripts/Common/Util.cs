using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class Util
{

    static public void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    public class TempRectInfo<T> where T : SingletonBase
    {
        public T t;
        public RectTransform rt;
        public TempRectInfo(T t) {
            this.t = t;
        }
        public TempRectInfo(T t, RectTransform rt)
        {
            this.t = t;
            this.rt = rt;
        }
    }
    public static TempRectInfo<T> InstantiateSingleton<T>() where T : SingletonBase
    {
        // 1 씬에 있는 루트 오브젝트 돌면서 꺼져 있는 오브젝트 중에 T 타입이 있는지 확인하자.
        T existSceneComponent = (T)GetAllObjectsOnlyInScene<T>();
        if (existSceneComponent != null)
            return new TempRectInfo<T>(existSceneComponent);


        // 2. 리소스폴더(어셋번들) 경로 확인.
        var type = typeof(T);
        string name = type.ShortName();
        GameObject resoruceGo = (GameObject)Resources.Load(name);
        if (resoruceGo != null)
        {
            GameObject loadGo = Instantiate(resoruceGo);
            var c = loadGo.GetComponent<T>();
            Debug.Assert(c != null, $"{name} 로드한 게임오브젝트에 컴포넌트가 없다");
            RectTransform rt = resoruceGo.GetComponent<RectTransform>();
            return new TempRectInfo<T>(c, rt);
        }

        // 3. 인스턴스 생성
        GameObject newComponent = new GameObject(name, type);
        T t = (T)newComponent.GetComponent(typeof(T));
        return new TempRectInfo<T>(t);
        //return (T)newComponent.GetComponent(typeof(T));
    }

    static SingletonBase GetAllObjectsOnlyInScene<T>() where T : Component
    {
        foreach (UnityEngine.Object go in Resources.FindObjectsOfTypeAll(typeof(T)))
        {
            if (!(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave || go.hideFlags == HideFlags.HideInHierarchy))
            {
                T t = (T)go;
                
                //Debug.LogWarning($"{typeof(T)} :: ({go}: {go.GetType()} {t.gameObject}");


                //에디터상에서 삭제한 오브젝트도 있는것으로 되어서 사용안함. 부모 없는 경우도 null 반환 시킴
                if (t.transform.parent == null)
                    return null;

                return (SingletonBase)go;
            }
        }

        return null;
    }

    #region Instantiate 래핑

    internal static GameObject Instantiate(string prefabPath)
    {
        GameObject o = (GameObject)Resources.Load(prefabPath);

        return Instantiate(o, Vector3.zero, Quaternion.identity);
    }

    internal static GameObject Instantiate(string prefabPath, Transform parent)
    {
        GameObject _object = (GameObject)Resources.Load(prefabPath);
        GameObject newGo = Object.Instantiate(_object, parent);

        InitObject(_object, newGo);
        return newGo;
    }

    private static void InitObject(UnityEngine.Object original, UnityEngine.Object newCompont)
    {
        newCompont.name = original.name;
    }

    internal static GameObject Instantiate(GameObject prefab)
    {
        return Util.Instantiate(prefab, Vector3.zero, Quaternion.identity);
    }

    internal static GameObject Instantiate(GameObject prefab, Vector3 worldPosition)
    {
        return Util.Instantiate(prefab, worldPosition, Quaternion.identity);
    }

    internal static GameObject Instantiate(Object _object, Transform parent)
    {
        GameObject newGo = (GameObject)Object.Instantiate(_object, parent);

        InitObject(_object, newGo);
        return newGo;
    }

    internal static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject newGo = Object.Instantiate(prefab, position, rotation);

        InitObject(prefab, newGo);
        return newGo;
    }

    internal static T Instantiate<T>(T prefab) where T : Component
    {
        return Instantiate<T>(prefab, Vector3.zero, Quaternion.identity);
    }

    internal static T Instantiate<T>(T prefab, Vector3 position) where T : Component
    {
        return Instantiate<T>(prefab, position, Quaternion.identity);
    }

    internal static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        T newGo = UnityEngine.Object.Instantiate<T>(prefab, position, rotation);

        InitObject(prefab, newGo);
        return newGo;
    }

    #endregion Instantiate 래핑
}