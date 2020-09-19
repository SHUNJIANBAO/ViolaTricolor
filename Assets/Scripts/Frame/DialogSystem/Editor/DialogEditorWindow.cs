using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditorInternal;

public class DialogEditorWindow : EditorWindow
{
    int _selectIndex;
    TalkAsset _curTalkAsset;
    string[] _selectToolBarArray = new string[3] { "进入条件", "对话列表", "结束事件" };
    const string _defaultPath = "Assets/GameAssets/DialogAssets/";

    [MenuItem("工具/对话编辑器")]
    static void OpenWindow()
    {
        var window = GetWindow<DialogEditorWindow>();
        window.titleContent = new GUIContent("对话编辑");
        window.Show();
    }

    #region Assets
    DialogAsset _copyAsset;
    DialogAsset _curDialogAsset;

    ReorderableList _dialogList;
    ReorderableList _talkAudioEventList;
    ReorderableList _talkEndEventList;
    #endregion

    private void OnEnable()
    {
        _copyAsset = null;
        _curTalkAsset = null;
    }

    private void OnGUI()
    {
        DrawMenuLabel();
        if (_copyAsset != null)
        {
            _selectIndex = GUILayout.Toolbar(_selectIndex, _selectToolBarArray);
            switch (_selectIndex)
            {
                case 0:
                    DrawBeforeDialogPanel();
                    break;
                case 1:
                    DrawDialogPanel();
                    break;
                case 2:
                    DrawAfterDialogPanel();
                    break;
            }
        }
    }

    #region DrawPanel
    void DrawBeforeDialogPanel()
    {
        _copyAsset.OptionName = EditorGUILayout.TextField("选项名称", _copyAsset.OptionName);
        _copyAsset.UnLockType = (E_UnLockType)EditorGUILayout.EnumPopup("解锁条件", _copyAsset.UnLockType);
        switch (_copyAsset.UnLockType)
        {
            case E_UnLockType.None:
                _copyAsset.NeedDialogAsset = null;
                break;
            case E_UnLockType.Talked:
                _copyAsset.NeedDialogAsset = (DialogAsset)EditorGUILayout.ObjectField("所需对话", _copyAsset.NeedDialogAsset, typeof(DialogAsset), false);
                break;
        }
    }

    Rect _talkAssetRect;
    Vector2 _scrollPos;
    void DrawDialogPanel()
    {
        EditorGUILayout.BeginHorizontal();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(200));
        _dialogList.DoLayoutList();
        EditorGUILayout.EndScrollView();
        GUILayout.Box("", GUILayout.Width(position.width - 210), GUILayout.Height(position.height - 30));
        _talkAssetRect = new Rect(210, 60, position.width - 230, position.height - 70);
        GUILayout.BeginArea(_talkAssetRect);
        DrawTalkAsset(_curTalkAsset);
        GUILayout.EndArea();
        EditorGUILayout.EndHorizontal();
    }

    void DrawAfterDialogPanel()
    {
        _copyAsset.TalkEndEventType = (E_TalkEndEventType)EditorGUILayout.EnumPopup("结束事件类型", _copyAsset.TalkEndEventType);
        switch (_copyAsset.TalkEndEventType)
        {
            case E_TalkEndEventType.Night:
                break;
            case E_TalkEndEventType.Select:
                _talkEndEventList.DoLayoutList();
                break;
            case E_TalkEndEventType.GameOver:
                break;
        }
    }
    #endregion



    #region BeforeDialogPanel

    #endregion



    #region DialogPanel
    void InitDialogAsset(DialogAsset asset)
    {
        _dialogList = new ReorderableList(asset.TalkAssetsList, typeof(TalkAsset));
        _dialogList.headerHeight = 0;
        _dialogList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var talkAsset = asset.TalkAssetsList[index];
            GUI.Label(rect, asset.TalkAssetsList[index].TalkId.ToString());
            if (isFocused)
            {
                _curTalkAsset = talkAsset;
                InitAudioEventList(_curTalkAsset);
            }
        };
        if (asset.TalkAssetsList != null && asset.TalkAssetsList.Count > 0)
        {
            _curTalkAsset = asset.TalkAssetsList[0];
            InitAudioEventList(_curTalkAsset);
        }
        _dialogList.onAddCallback = AddTalkAsset;
    }

    void InitTalkEndEventList(DialogAsset asset)
    {
        _talkEndEventList = new ReorderableList(asset.SelectDialogAssetList, typeof(DialogAsset));
        _talkEndEventList.drawHeaderCallback = (rect) =>
        {
            GUI.Label(rect, "选项列表");
        };
        _talkEndEventList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var talkAsset = asset.TalkAssetsList[index];
            asset.SelectDialogAssetList[index] = (DialogAsset)EditorGUI.ObjectField(rect, asset.SelectDialogAssetList[index], typeof(DialogAsset), false);
        };
        _talkEndEventList.onAddCallback = (list) =>
        {
            DialogAsset tempAsset = ScriptableObject.CreateInstance<DialogAsset>();
            asset.SelectDialogAssetList.Add(tempAsset);
            int index = asset.SelectDialogAssetList.IndexOf(tempAsset);
            string assetPath = EditorUtility.OpenFilePanel("Load", _defaultPath, "asset");
            if (string.IsNullOrEmpty(assetPath))
            {
                return;
            }
            assetPath = assetPath.Replace(Application.dataPath, "Assets");

            tempAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogAsset)) as DialogAsset;
            asset.SelectDialogAssetList[index] = tempAsset;
        };
    }


    void InitAudioEventList(TalkAsset talkAsset)
    {
        if (talkAsset == null) return;
        _talkAudioEventList = new ReorderableList(talkAsset.AudioEventList, typeof(AudioEvent));
        _talkAudioEventList.headerHeight = 0;
        _talkAudioEventList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            float posX = rect.position.x;
            int width = 100;
            int height = 20;

            Rect tempRect = new Rect(posX, rect.position.y, width, height);
            talkAsset.AudioEventList[index].AudioDelay = EditorGUI.IntField(tempRect, "等待字数", talkAsset.AudioEventList[index].AudioDelay);

            posX += width;
            width = 200;
            tempRect = new Rect(posX, rect.position.y, width, height);
            talkAsset.AudioEventList[index].Audio = (AudioClip)EditorGUI.ObjectField(tempRect, "音效", talkAsset.AudioEventList[index].Audio, typeof(AudioClip), false);
        };

    }


    void AddTalkAsset(ReorderableList reorderable)
    {
        TalkAsset talkAsset = ScriptableObject.CreateInstance<TalkAsset>();
        if (reorderable.list.Count > 0)
        {
            var list = reorderable.list as List<TalkAsset>;
            talkAsset.TalkId = list.Max(i => i.TalkId) + 1;
        }
        else
        {
            talkAsset.TalkId = 1;
        }
        talkAsset.name = talkAsset.TalkId.ToString();
        _copyAsset.TalkAssetsList.Add(talkAsset);
    }

    Vector2 _talkScrollPos;
    Vector2 _audioEventScrollPos;
    void DrawTalkAsset(TalkAsset asset)
    {
        if (asset == null) return;
        _talkScrollPos = EditorGUILayout.BeginScrollView(_talkScrollPos);
        EditorGUILayout.BeginHorizontal();
        asset.Body = (Sprite)EditorGUILayout.ObjectField(asset.Body, typeof(Sprite), false, GUILayout.Width(150), GUILayout.Height(250));
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 90;
        asset.BodyPos = (E_BodyPos)EditorGUILayout.EnumPopup("立绘显示类型", asset.BodyPos);
        EditorGUIUtility.labelWidth = 60;
        asset.TalkerName = EditorGUILayout.TextField("角色名", asset.TalkerName);
        asset.TalkEvent = (E_TalkEvent)EditorGUILayout.EnumPopup("触发事件", asset.TalkEvent);
        GUILayout.Label("背景图片");
        asset.Background = (Sprite)EditorGUILayout.ObjectField(asset.Background, typeof(Sprite), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 90;
        asset.DubDelay = EditorGUILayout.IntField("配音等待字数", asset.DubDelay);
        EditorGUIUtility.labelWidth = 60;
        asset.Dub = (AudioClip)EditorGUILayout.ObjectField("角色配音", asset.Dub, typeof(AudioClip), false);
        asset.Bgm = (AudioClip)EditorGUILayout.ObjectField("背景音乐", asset.Bgm, typeof(AudioClip), false);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("音频列表");
        _audioEventScrollPos = EditorGUILayout.BeginScrollView(_audioEventScrollPos, GUILayout.Height(90));
        _talkAudioEventList?.DoLayoutList();
        EditorGUILayout.EndScrollView();
        GUILayout.Label("对话内容");
        asset.Content = EditorGUILayout.TextArea(asset.Content, GUILayout.Height(80));

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        //GUILayout.Label("背景图");
        //asset.Background = (Sprite)EditorGUILayout.ObjectField(asset.Background, typeof(Texture2D), false);//, GUILayout.Width(192*3), GUILayout.Height(108*3));


        EditorGUILayout.EndScrollView();
    }
    #endregion



    #region AfterDialogPanel

    #endregion


    #region MenuButtons
    void DrawMenuLabel()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("新建", GUILayout.Width(80)))
        {
            CreateNewDialogAsset();
        }
        if (GUILayout.Button("加载", GUILayout.Width(80)))
        {
            LoadDiaologAsset();
        }
        if (_copyAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(_curDialogAsset);
            if (!string.IsNullOrEmpty(path))
            {
                if (GUILayout.Button("保存", GUILayout.Width(80)))
                {
                    SaveDialogAsset(_curDialogAsset, _copyAsset);
                }
            }
            if (GUILayout.Button("另存为", GUILayout.Width(80)))
            {
                SaveAsDialogAsset(_copyAsset);
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void CreateNewDialogAsset()
    {
        _curDialogAsset = ScriptableObject.CreateInstance<DialogAsset>();
        if (SaveAsDialogAsset(_curDialogAsset))
        {
            _copyAsset = _curDialogAsset.Copy();
            InitDialogAsset(_copyAsset);
            InitTalkEndEventList(_copyAsset);
        }
    }

    /// <summary>
    /// 加载对话资源
    /// </summary>
    void LoadDiaologAsset()
    {
        string assetPath = EditorUtility.OpenFilePanel("Load", _defaultPath, "asset");
        if (string.IsNullOrEmpty(assetPath)) return;
        assetPath = assetPath.Replace(Application.dataPath, "Assets");

        _curDialogAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogAsset)) as DialogAsset;
        _copyAsset = _curDialogAsset.Copy();
        InitDialogAsset(_copyAsset);
        InitTalkEndEventList(_copyAsset);
    }

    void SaveDialogAsset(DialogAsset dialogAsset, DialogAsset copyAsset)
    {
        foreach (var talkAsset in dialogAsset.TalkAssetsList)
        {
            AssetDatabase.RemoveObjectFromAsset(talkAsset);
        }
        foreach (var selectAsset in dialogAsset.SelectDialogAssetList)
        {
            AssetDatabase.RemoveObjectFromAsset(selectAsset);
        }
        dialogAsset.TalkAssetsList.Clear();
        dialogAsset.SelectDialogAssetList.Clear();

        dialogAsset.OptionName = copyAsset.OptionName;
        dialogAsset.UnLockType = copyAsset.UnLockType;
        dialogAsset.NeedDialogAsset = copyAsset.NeedDialogAsset;

        dialogAsset.TalkEndEventType = copyAsset.TalkEndEventType;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        foreach (var talkAsset in copyAsset.TalkAssetsList)
        {
            var tempTalkAsset = talkAsset.Copy();
            tempTalkAsset.name = tempTalkAsset.TalkId.ToString();
            AssetDatabase.AddObjectToAsset(tempTalkAsset, dialogAsset);
            dialogAsset.TalkAssetsList.Add(tempTalkAsset);
        }
        foreach (var seleteAsset in copyAsset.SelectDialogAssetList)
        {
            dialogAsset.SelectDialogAssetList.Add(seleteAsset);
        }

        EditorUtility.SetDirty(dialogAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    bool SaveAsDialogAsset(DialogAsset newAsset)
    {
        if (!Directory.Exists(_defaultPath))
        {
            Directory.CreateDirectory(_defaultPath);
        }
        AssetDatabase.Refresh();
        string assetPath = EditorUtility.SaveFilePanelInProject("Save", "New DialogAsset", "asset", "", _defaultPath);
        if (string.IsNullOrEmpty(assetPath)) return false;
        AssetDatabase.CreateAsset(newAsset, assetPath);

        foreach (var talkAsset in newAsset.TalkAssetsList)
        {
            talkAsset.name = talkAsset.TalkId.ToString();
            AssetDatabase.AddObjectToAsset(talkAsset, newAsset);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _curDialogAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogAsset)) as DialogAsset;
        return true;
    }
    #endregion
}
