using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using PbAudioSystem;

public class DialogEditorWindow : EditorWindow
{
    #region Proprety
    int _selectIndex;
    DialogueAsset _curDialogueAsset;
    string[] _selectToolBarArray = new string[3] { "进入条件", "对话列表", "结束事件" };
    const string _defaultPath = "Assets/GameAssets/DialogAssets/";
    #endregion

    #region Window
    [MenuItem("工具/对话编辑器")]
    static void OpenWindow()
    {
        var window = GetWindow<DialogEditorWindow>();
        window.titleContent = new GUIContent("对话编辑");
        window.Show();
    }
    #endregion

    #region Assets
    DialogAsset _copyAsset;
    DialogAsset _curDialogAsset;

    ReorderableList _dialogueList;
    ReorderableList _talkEventList;
    ReorderableList _talkEndEventList;
    #endregion

    private void OnEnable()
    {
        _copyAsset = null;
        _curDialogueAsset = null;

        _wordTextStyle = new GUIStyle();
        _wordTextStyle.fontSize = 20;
        _wordTextStyle.fontStyle = FontStyle.Bold;
        _wordTextStyle.normal.textColor = Color.white;
        _wordTextStyle.alignment = TextAnchor.MiddleCenter;

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
    float _talkAssetRectWidth => position.width - 230;
    Vector2 _scrollPos;
    void DrawDialogPanel()
    {
        EditorGUILayout.BeginHorizontal();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(200));
        _dialogueList.DoLayoutList();
        EditorGUILayout.EndScrollView();
        //GUILayout.Box("", GUILayout.Width(position.width - 210), GUILayout.Height(position.height - 30));
        _talkAssetRect = new Rect(210, 60, _talkAssetRectWidth, position.height - 70);
        GUILayout.BeginArea(_talkAssetRect);
        DrawDialogueAsset(_curDialogueAsset);
        GUILayout.EndArea();
        EditorGUILayout.EndHorizontal();
    }

    void DrawAfterDialogPanel()
    {
        _copyAsset.TalkEndEventType = (E_TalkEndEventType)EditorGUILayout.EnumPopup("结束事件类型", _copyAsset.TalkEndEventType);
        switch (_copyAsset.TalkEndEventType)
        {
            case E_TalkEndEventType.Night:
                _copyAsset.LinkedDialogAsset = (DialogAsset)EditorGUILayout.ObjectField("连接对话", _copyAsset.LinkedDialogAsset, typeof(DialogAsset), false);
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
    void InitTalkAsset(DialogAsset asset)
    {
        _dialogueList = new ReorderableList(asset.DialogueAssetList, typeof(DialogueAsset));
        _dialogueList.headerHeight = 0;
        _dialogueList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var talkAsset = asset.DialogueAssetList[index];
            GUI.Label(rect, asset.DialogueAssetList[index].DialogueId.ToString());
            if (isFocused)
            {
                _curDialogueAsset = talkAsset;
                InitAudioEventList(_curDialogueAsset);
            }
        };
        if (asset.DialogueAssetList != null && asset.DialogueAssetList.Count > 0)
        {
            _curDialogueAsset = asset.DialogueAssetList[0];
            InitAudioEventList(_curDialogueAsset);
        }
        _dialogueList.onAddCallback = AddDialogueAsset;
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
            var talkAsset = asset.DialogueAssetList[index];
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


    void InitAudioEventList(DialogueAsset dialogueAsset)
    {
        if (dialogueAsset == null) return;
        _talkEventList = new ReorderableList(dialogueAsset.DelayEventList, typeof(DelayEvent));
        _talkEventList.headerHeight = 0;
        _talkEventList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            float posX = rect.position.x;
            int width = 120;
            int height = 20;
            EditorGUIUtility.labelWidth = 60;

            Rect tempRect = new Rect(posX, rect.position.y, width, height);
            dialogueAsset.DelayEventList[index].Delay = EditorGUI.IntField(tempRect, "等待字数", dialogueAsset.DelayEventList[index].Delay);
            posX += width;
            tempRect = new Rect(posX, rect.position.y, width, height);
            dialogueAsset.DelayEventList[index].EventType = (E_EventType)EditorGUI.EnumPopup(tempRect, dialogueAsset.DelayEventList[index].EventType);

            switch (dialogueAsset.DelayEventList[index].EventType)
            {
                case E_EventType.PlayAudio:
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    dialogueAsset.DelayEventList[index].AuidoType = (E_AudioType)EditorGUI.EnumPopup(tempRect, "声音类型", dialogueAsset.DelayEventList[index].AuidoType);
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    dialogueAsset.DelayEventList[index].Audio = (AudioClip)EditorGUI.ObjectField(tempRect, "音效", dialogueAsset.DelayEventList[index].Audio, typeof(AudioClip), false);
                    break;
                case E_EventType.ChangeBody:
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    dialogueAsset.DelayEventList[index].BodyPos = (E_BodyPos)EditorGUI.EnumPopup(tempRect, "立绘位置", dialogueAsset.DelayEventList[index].BodyPos);

                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    dialogueAsset.DelayEventList[index].BodyShowType = (E_BodyShowType)EditorGUI.EnumPopup(tempRect, "显示类型", dialogueAsset.DelayEventList[index].BodyShowType);

                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    EditorGUIUtility.labelWidth = 40;
                    dialogueAsset.DelayEventList[index].Body = (GameObject)EditorGUI.ObjectField(tempRect, "立绘", dialogueAsset.DelayEventList[index].Body, typeof(GameObject), false);

                    break;
                default:
                    break;
            }
        };

    }


    void AddDialogueAsset(ReorderableList reorderable)
    {
        DialogueAsset dialogueAsset = null;
        if (reorderable.list.Count > 0)
        {
            dialogueAsset = ScriptableObject.CreateInstance<DialogueAsset>();
            var list = reorderable.list as List<DialogueAsset>;
            var maxId = list.Max(i => i.DialogueId);
            var asset = list.Find(i => i.DialogueId == maxId);
            dialogueAsset = asset.Copy();
            dialogueAsset.DialogueId = maxId + 1;
        }
        else
        {
            dialogueAsset = ScriptableObject.CreateInstance<DialogueAsset>();
            dialogueAsset.DialogueId = 1;
        }
        dialogueAsset.name = dialogueAsset.DialogueId.ToString();
        _copyAsset.DialogueAssetList.Add(dialogueAsset);
    }

    Vector2 _talkScrollPos;
    Vector2 _eventScrollPos;
    void DrawDialogueAsset(DialogueAsset asset)
    {
        if (asset == null) return;
        _talkScrollPos = EditorGUILayout.BeginScrollView(_talkScrollPos);
        DrawDialogueView(asset);
        EditorGUILayout.EndScrollView();
    }

    void DrawDialogueView(DialogueAsset asset)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 60;
        asset.DialogType = (E_DialogType)EditorGUILayout.EnumPopup("对话框类型", asset.DialogType);
        if (asset.DialogType == E_DialogType.FullScreen)
        {
            EditorGUIUtility.labelWidth = 90;
            asset.IsNewTalk = EditorGUILayout.Toggle("是否为新段落", asset.IsNewTalk);
            asset.IsNewPage = EditorGUILayout.Toggle("是否为新的一页", asset.IsNewPage);
        }
        GUILayout.Label("背景");
        asset.Background = (GameObject)EditorGUILayout.ObjectField(asset.Background, typeof(GameObject), false);
        asset.Bgm = (AudioClip)EditorGUILayout.ObjectField("背景音乐", asset.Bgm, typeof(AudioClip), false);
        EditorGUILayout.EndHorizontal();


        if (asset.DialogType == E_DialogType.Normal)
        {
            EditorGUIUtility.labelWidth = 60;

            EditorGUILayout.BeginHorizontal();
            asset.LeftBody = (GameObject)EditorGUILayout.ObjectField("立绘(左)", asset.LeftBody, typeof(GameObject), false);
            asset.LeftBodyShowType = (E_BodyShowType)EditorGUILayout.EnumPopup("显示类型", asset.LeftBodyShowType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            asset.RightBody = (GameObject)EditorGUILayout.ObjectField("立绘(右)", asset.RightBody, typeof(GameObject), false);
            asset.RightBodyShowType = (E_BodyShowType)EditorGUILayout.EnumPopup("显示类型", asset.RightBodyShowType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            asset.CenterBody = (GameObject)EditorGUILayout.ObjectField("立绘(中)", asset.CenterBody, typeof(GameObject), false);
            asset.CenterBodyShowType = (E_BodyShowType)EditorGUILayout.EnumPopup("显示类型", asset.CenterBodyShowType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            asset.BottomBody = (GameObject)EditorGUILayout.ObjectField("立绘(下)", asset.BottomBody, typeof(GameObject), false);
            asset.BottomBodyShowType = (E_BodyShowType)EditorGUILayout.EnumPopup("显示类型", asset.BottomBodyShowType);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            asset.NamePos = (E_NamePos)EditorGUILayout.EnumPopup("名字位置", asset.NamePos);
            asset.TalkerName = EditorGUILayout.TextField("角色名", asset.TalkerName);

            EditorGUIUtility.labelWidth = 90;
            asset.DubDelay = EditorGUILayout.IntField("配音等待字数", asset.DubDelay);
            asset.Dub = (AudioClip)EditorGUILayout.ObjectField("角色配音", asset.Dub, typeof(AudioClip), false);
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("事件列表");
            _eventScrollPos = EditorGUILayout.BeginScrollView(_eventScrollPos, GUILayout.Height(90));
            _talkEventList?.DoLayoutList();
            EditorGUILayout.EndScrollView();

        }


        GUILayout.Label("对话内容");
        EditorGUILayout.BeginHorizontal();
        asset.Content = EditorGUILayout.TextArea(asset.Content, GUILayout.Height(80));
        if (GUILayout.Button("生成", GUILayout.Width(80), GUILayout.Height(80)))
        {
            asset.WordList = new List<TyperRhythm>();
            foreach (var word in asset.Content)
            {
                TyperRhythm typerWord = new TyperRhythm(word, 0.05f);
                asset.WordList.Add(typerWord);
            }
        }
        EditorGUILayout.EndHorizontal();
        if (asset.WordList.Count > 0)
        {
            DrawTyperWordList(asset.WordList);
        }


    }

    float _wordWidth = 30;
    void DrawTyperWordList(List<TyperRhythm> wordList)
    {
        int count = 0;
        EditorGUILayout.BeginHorizontal();
        foreach (var word in wordList)
        {
            count++;
            if (count*(_wordWidth+7) >= _talkAssetRectWidth)
            {
                count = 0;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
            DrawTyperWord(word, _wordWidth);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    GUIStyle _wordTextStyle;
    void DrawTyperWord(TyperRhythm word, float width)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.TextField(word.Word.ToString(), _wordTextStyle, GUILayout.Width(width), GUILayout.Height(width));
        word.WaitTime = EditorGUILayout.FloatField(word.WaitTime, GUILayout.Width(width));
        EditorGUILayout.EndVertical();
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
            InitTalkAsset(_copyAsset);
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
        InitTalkAsset(_copyAsset);
        InitTalkEndEventList(_copyAsset);
    }

    void SaveDialogAsset(DialogAsset dialogAsset, DialogAsset copyAsset)
    {
        foreach (var talkAsset in dialogAsset.DialogueAssetList)
        {
            AssetDatabase.RemoveObjectFromAsset(talkAsset);
        }
        dialogAsset.DialogueAssetList.Clear();

        dialogAsset.OptionName = copyAsset.OptionName;
        dialogAsset.UnLockType = copyAsset.UnLockType;
        dialogAsset.NeedDialogAsset = copyAsset.NeedDialogAsset;
        dialogAsset.LinkedDialogAsset = copyAsset.LinkedDialogAsset;
        dialogAsset.SelectDialogAssetList = new List<DialogAsset>(copyAsset.SelectDialogAssetList);

        dialogAsset.TalkEndEventType = copyAsset.TalkEndEventType;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        foreach (var talkAsset in copyAsset.DialogueAssetList)
        {
            var tempTalkAsset = talkAsset.Copy();
            tempTalkAsset.name = tempTalkAsset.DialogueId.ToString();
            AssetDatabase.AddObjectToAsset(tempTalkAsset, dialogAsset);
            dialogAsset.DialogueAssetList.Add(tempTalkAsset);
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

        foreach (var talkAsset in newAsset.DialogueAssetList)
        {
            talkAsset.name = talkAsset.DialogueId.ToString();
            AssetDatabase.AddObjectToAsset(talkAsset, newAsset);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _curDialogAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogAsset)) as DialogAsset;
        return true;
    }
    #endregion
}
