using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CreateBodyPrefab : EditorWindow
{
    public enum E_PrefabType
    {
        Body,
        Background,
    }

    [MenuItem("工具/创建预制体")]
    static void OpenWindow()
    {
        var window = GetWindow<CreateBodyPrefab>();
        window.titleContent = new GUIContent("创建预制体");
        window.Show();
    }
    const string _demoPath = "Assets/Editor/BodyDemo.prefab";

    string _prefabName;
    Sprite _image;
    E_PrefabType _prefabType;
    //RuntimeAnimatorController _animatorController;
    private void OnGUI()
    {
        _prefabName = EditorGUILayout.TextField("预制体名", _prefabName);
        _prefabType = (E_PrefabType)EditorGUILayout.EnumPopup("预制体类型",_prefabType);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("默认图片");
        _image = (Sprite)EditorGUILayout.ObjectField(_image, typeof(Sprite), false);
        EditorGUILayout.EndHorizontal();
        //_animatorController = (RuntimeAnimatorController)EditorGUILayout.ObjectField("动画", _animatorController, typeof(RuntimeAnimatorController), false);
        if (GUILayout.Button("创建"))
        {
            if (string.IsNullOrEmpty(_prefabName))
            {
                ShowNotification(new GUIContent("预制体名不能为空"));
                return;
            }
            if (_image == null)
            {
                ShowNotification(new GUIContent("图片不能为空"));
                return;
            }
            CreateBody();
            ShowNotification(new GUIContent("创建成功"));
        }
    }

    void CreateBody()
    {
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(_demoPath);
        Transform canvas = GameObject.Find("Canvas").transform;
        GameObject go = GameObject.Instantiate(obj, canvas, false);
        go.name = _prefabName;
        var bodyImage = go.GetComponent<Image>();
        bodyImage.sprite = _image;
        bodyImage.SetNativeSize();
        //var bodyAnimator = go.GetComponent<Animator>();
        //bodyAnimator.runtimeAnimatorController = _animatorController;

        switch (_prefabType)
        {
            case E_PrefabType.Body:
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, "Assets/GameAssets/Prefabs/Body/" + _prefabName + ".prefab", InteractionMode.AutomatedAction);
                break;
            case E_PrefabType.Background:
                PrefabUtility.SaveAsPrefabAssetAndConnect(go, "Assets/GameAssets/Prefabs/Background/" + _prefabName + ".prefab", InteractionMode.AutomatedAction);
                break;
        }
        DestroyImmediate(go);
    }
}
