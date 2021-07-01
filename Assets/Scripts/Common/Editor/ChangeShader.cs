using System.Collections.Generic;
using System.IO;
//using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ChangeShader : ScriptableWizard
{
    [MenuItem("Assets/Change Shader")]
    static void Init()
    {
        ChangeShader changeShader = DisplayWizard<ChangeShader>(
            "Change Shader", "닫기");
        changeShader.InitConfig();
    }

    void OnWizardCreate() //닫기버튼 누를때 실행됨
    {
        Debug.Log("닫기 버튼 눌렀음");
    }

    private void ChangeShaderFn()
    {
        Debug.Log("적용 버튼 눌렀음");

        switch (type)
        {
            case Type.AllFileInDirectory:
                ChangeShaderInDirectory(findPath);
                break;
            case Type.SelectOnly:
                ChangeShaderSelected(findPath);
                break;
        }

        AssetDatabase.SaveAssets();

        SaveConfig();
    }

    private void SaveConfig()
    {
        PlayerPrefs.SetString(FindPathKey, findPath);

        if(toShader != null)
        {
            string shaderPath = AssetDatabase.GetAssetPath(toShader);
            PlayerPrefs.SetString(LastSelectedShaderKey, shaderPath);
        }
    }

    public enum Type
    {
        SelectOnly,
        AllFileInDirectory,
    }
    public Type type;

    public const string FindPathKey = "FindPath";
    public const string LastSelectedShaderKey = "LastSelectedShader";
    private void InitConfig()
    {
        findPath = PlayerPrefs.GetString(FindPathKey, findPath);

        //toShader  마지막에 지정했던 쉐이더로 지정하자.
        string shaderPath = PlayerPrefs.GetString(LastSelectedShaderKey);
        if (string.IsNullOrEmpty(shaderPath) == false)
        {
            toShader = (Shader)AssetDatabase.LoadAssetAtPath(shaderPath, typeof(Shader));
        }
    }

    void OnWizardUpdate()
    {
        helpString = @"Select Only : 하이어라키나 프로젝트UI에서 선택한 게임오브젝트 혹은 메테리얼을 대상으로 변환
All File In Directory : 디렉토리에 있는 모든 메테리얼을 대상으로 변환";
        isValid = true;
    }

    public string findPath = "Assets";
    protected override bool DrawWizardGUI()
    {
        base.DrawWizardGUI();

        // 버튼 이벤트 확인하는 것도 가능.
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.C && e.control && e.alt && e.type == EventType.KeyDown)
            {
                Debug.Log("press Control + alt + C");
            }
        }

        if (GUILayout.Button("쉐이더 교체"))
        {
            ChangeShaderFn();
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("쉐이더 설정 복사"))
            {
                Debug.Log("쉐이더 설정 복사하자");
                CopyShaderConfig();
            }

            if (GUILayout.Button("쉐이더 설정 붙여넣기"))
            {
                Debug.Log("쉐이더 설정 붙여넣기");
                PasteShaderConfig();
            }
        }
        GUILayout.EndHorizontal();

        return true;
    }

    class ShaderPropertyInfo
    {
        public string name;
        public ShaderUtil.ShaderPropertyType type;
        internal Color colorValue;
        internal Vector4 vectorValue;
        internal float floatValue;
        internal Texture textureValue;
        internal List<float> floatArray = new List<float>();

        public ShaderPropertyInfo(string name, ShaderUtil.ShaderPropertyType type)
        {
            this.name = name;
            this.type = type;
        }
    }

    List<ShaderPropertyInfo> properties = null;
    private void CopyShaderConfig()
    {
        //선택된 메테리얼에서 값을 읽자.
        List<Material> toMaterials = GetSelectedMaterials();
        Debug.Assert(toMaterials.Count == 1, "선택한 메테리얼이 1개가 아닙니다");

        Material toMat = toMaterials[0];

        toShader = toMat.shader;

        int count = ShaderUtil.GetPropertyCount(toShader);
        properties = new List<ShaderPropertyInfo>(count);
        for (var i = 0; i < count; ++i)
        {
            var name = ShaderUtil.GetPropertyName(toShader, i);
            var type = ShaderUtil.GetPropertyType(toShader, i);
            properties.Add(new ShaderPropertyInfo(name, type));
        }

        foreach (var item in properties)
        {
            switch (item.type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    item.colorValue = toMat.GetColor(item.name);
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    item.vectorValue = toMat.GetVector(item.name);
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    item.floatValue = toMat.GetFloat(item.name);
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    toMat.GetFloatArray(item.name, item.floatArray);
                    break;
                    //case ShaderUtil.ShaderPropertyType.TexEnv: // 텍스쳐는 복사할 필요 없으므로 제외
                    //    item.textureValue = toMat.GetTexture(item.name);
                    //break;
            }
        }
    }
    private void PasteShaderConfig()
    {
        List<Material> desMaterials = GetSelectedMaterials();
        foreach (var desMat in desMaterials)
        {
            foreach (var item in properties)
            {
                switch (item.type)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        desMat.SetColor(item.name, item.colorValue);
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        desMat.SetVector(item.name, item.vectorValue);
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                        desMat.SetFloat(item.name, item.floatValue);
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                        desMat.SetFloatArray(item.name, item.floatArray); // 테스트 안해봤음.
                        break;
                    //case ShaderUtil.ShaderPropertyType.TexEnv:
                    //    desMat.SetTexture(item.name, item.textureValue);
                    //    break;
                }
            }
        }
    }


    private void ChangeShaderSelected(string findPath)
    {
        // 선택한 게임오브젝트 혹은 메테리얼을 변경하자.
        List<Material> desMaterials = GetSelectedMaterials();
        ChangeMaterialList(desMaterials);
    }

    private static List<Material> GetSelectedMaterials()
    {
        List<Material> desMaterials = new List<Material>();
        foreach (var item in Selection.objects)
        {
            var itemType = item.GetType();
            if (itemType == typeof(Material))
                desMaterials.Add((Material)item);
            else if (itemType == typeof(GameObject))
            {
                var materials = ((GameObject)item).GetComponentsInChildren<Renderer>().Select(x => x.sharedMaterial);
                desMaterials.AddRange(materials);
            }
        }
        return desMaterials;
    }

    private void ChangeMaterialList(List<Material> desMaterials)
    {
        if (toShader == null)
        {
            if (EditorUtility.DisplayDialog("경고", "쉐이더를 지정하지 않았습니다. 계속 진행하겠습니까?", "확인", "취소") == false)
                return;
        }
        var materials = desMaterials.Distinct() // 중복제거
            .Where(x => x.shader != toShader);  // 대상 필터링

        foreach (var mat in materials)
        {
            mat.shader = toShader;
        }
    }

    public Shader toShader;

    void ChangeShaderInDirectory(string dir)
    {
        string[] fs = Directory.GetFiles(dir, "*.mat", SearchOption.AllDirectories);
        List<Material> desMaterials = new List<Material>();
        foreach (string f in fs)
        {
            Material mat = (Material)AssetDatabase.LoadAssetAtPath(f, typeof(Material));
            if (mat == null)
                continue;

            desMaterials.Add(mat);
        }
        ChangeMaterialList(desMaterials);
    }
}