using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CreateBodyPrefab : EditorWindow
{
    [MenuItem("工具/创建立绘")]
    static void OpenWindow()
    {
        var window = GetWindow<CreateBodyPrefab>();
        window.titleContent = new GUIContent("创建立绘");
        window.Show();
    }
    const string _demoPath = "Assets/Editor/BodyDemo.prefab";

    string _prefabName;
    Sprite _body;
    Sprite _normalFace;
    AnimationClip _normalFaceClip;
    private void OnGUI()
    {
        _prefabName = EditorGUILayout.TextField("预制体名", _prefabName);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("立绘");
        _body = (Sprite)EditorGUILayout.ObjectField(_body, typeof(Sprite), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("默认表情");
        _normalFace = (Sprite)EditorGUILayout.ObjectField( _normalFace, typeof(Sprite), false);
        EditorGUILayout.EndHorizontal();
        _normalFaceClip = (AnimationClip)EditorGUILayout.ObjectField("默认表情动画", _normalFaceClip, typeof(AnimationClip), false);
        if (GUILayout.Button("创建"))
        {
            if (string.IsNullOrEmpty(_prefabName))
            {
                ShowNotification(new GUIContent("预制体名不能为空"));
                return;
            }
            if (_body == null)
            {
                ShowNotification(new GUIContent("立绘不能为空"));
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
        bodyImage.sprite = _body;
        bodyImage.SetNativeSize();
        var body = go.GetComponent<Body>();
        body.SetFace(_normalFace);
        body.SetFaceAnimation(_normalFaceClip);

        PrefabUtility.SaveAsPrefabAssetAndConnect(go, "Assets/GameAssets/Prefabs/Body/" + _prefabName + ".prefab", InteractionMode.AutomatedAction);
        DestroyImmediate(go);
    }
}
