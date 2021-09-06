using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditorInternal;
//using PbAudioSystem;
using System.Text.RegularExpressions;
using PbFramework;

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
    DialogAsset _curDialogAsset;

    ReorderableList _dialogueList;
    ReorderableList _talkEventList;
    ReorderableList _talkEndEventList;
    ReorderableList _recordDataList;
    #endregion

    private void OnEnable()
    {
        _curDialogAsset = null;
        _curDialogueAsset = null;

        _wordTextStyle = new GUIStyle();
        _wordTextStyle.fontSize = 20;
        _wordTextStyle.fontStyle = FontStyle.Bold;
        _wordTextStyle.normal.textColor = Color.white;
        _wordTextStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnDisable()
    {
        AssetDatabase.SaveAssets();
    }
    private void OnGUI()
    {
        DrawMenuLabel();
        if (_curDialogAsset != null)
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
            EditorUtility.SetDirty(_curDialogAsset);
        }
    }

    #region InitAsset

    public void RefreshAsset(DialogAsset asset)
    {
        InitTalkAsset(asset);
        InitTalkEndEventList(asset);
        InitRecordDataList(asset);
    }

    void InitTalkAsset(DialogAsset asset)
    {
        _dialogueList = new ReorderableList(asset.DialogueAssetList, typeof(DialogueAsset));
        _dialogueList.headerHeight = 0;
        _dialogueList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var talkAsset = asset.DialogueAssetList[index];

            if (talkAsset.LanguageDict.Count == 0)
            {
                talkAsset.LanguageDict.Add(E_LanguageType.CN, new ContentData());
                talkAsset.LanguageDict.Add(E_LanguageType.JP, new ContentData());
                talkAsset.LanguageDict.Add(E_LanguageType.EN, new ContentData());
            }

            var languageType = talkAsset.LanguageType;
            talkAsset.LanguageDict.TryGetValue(languageType, out ContentData data);

            GUI.Label(rect, (index + 1).ToString() + ":   " + data.Content);
            //GUI.Label(rect, asset.DialogueAssetList[index].DialogueId.ToString());
            if (isFocused)
            {
                _curDialogueAsset = talkAsset;
                InitTalkEventList(_curDialogueAsset);
            }
        };
        if (asset.DialogueAssetList != null && asset.DialogueAssetList.Count > 0)
        {
            _curDialogueAsset = asset.DialogueAssetList[0];
            if (_curDialogueAsset.LanguageDict.Count == 0)
            {
                _curDialogueAsset.LanguageDict.Add(E_LanguageType.CN, new ContentData());
                _curDialogueAsset.LanguageDict.Add(E_LanguageType.JP, new ContentData());
                _curDialogueAsset.LanguageDict.Add(E_LanguageType.EN, new ContentData());
            }
            InitTalkEventList(_curDialogueAsset);
        }
        _dialogueList.onAddCallback = AddDialogueAsset;
        _dialogueList.onRemoveCallback = RemoveDialogueAsset;
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


    void InitTalkEventList(DialogueAsset dialogueAsset)
    {
        if (dialogueAsset == null) return;
        dialogueAsset.LanguageDict.TryGetValue(dialogueAsset.LanguageType, out ContentData data);
        _talkEventList = new ReorderableList(data.DelayEventList, typeof(DelayEvent));
        _talkEventList.headerHeight = 0;
        _talkEventList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            float posX = rect.position.x;
            int width = 120;
            int height = 20;
            EditorGUIUtility.labelWidth = 60;

            Rect tempRect = new Rect(posX, rect.position.y, width, height);
            data.DelayEventList[index].Delay = EditorGUI.IntField(tempRect, "等待字数", data.DelayEventList[index].Delay);
            posX += width;
            tempRect = new Rect(posX, rect.position.y, width, height);
            data.DelayEventList[index].EventType = (E_EventType)EditorGUI.EnumPopup(tempRect, data.DelayEventList[index].EventType);

            switch (data.DelayEventList[index].EventType)
            {
                case E_EventType.PlayAudio:
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].AuidoType = (E_AudioType)EditorGUI.EnumPopup(tempRect, "声音类型", data.DelayEventList[index].AuidoType);
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].Audio = (AudioClip)EditorGUI.ObjectField(tempRect, "音效", data.DelayEventList[index].Audio, typeof(AudioClip), false);
                    break;
                case E_EventType.ChangeBody:
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].BodyPos = (E_BodyPos)EditorGUI.EnumPopup(tempRect, "立绘位置", data.DelayEventList[index].BodyPos);

                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].BodyShowType = (E_BodyShowType)EditorGUI.EnumPopup(tempRect, "显示类型", data.DelayEventList[index].BodyShowType);

                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    EditorGUIUtility.labelWidth = 40;
                    data.DelayEventList[index].Body = (GameObject)EditorGUI.ObjectField(tempRect, "立绘", data.DelayEventList[index].Body, typeof(GameObject), false);

                    break;
                case E_EventType.Hint:
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].HintTime = EditorGUI.FloatField(tempRect, "显示时长", data.DelayEventList[index].HintTime);
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].StringValue = EditorGUI.TextField(tempRect, "提示文字", data.DelayEventList[index].StringValue);
                    break;
                case E_EventType.Label:
                    posX += width;
                    width = 200;
                    tempRect = new Rect(posX, rect.position.y, width, height);
                    data.DelayEventList[index].StringValue = EditorGUI.TextField(tempRect, "文字", data.DelayEventList[index].StringValue);
                    break;
                default:
                    break;
            }
        };

    }

    void InitRecordDataList(DialogAsset asset)
    {
        _recordDataList = new ReorderableList(asset.NodeRecordDataList, typeof(RecordData));
        _recordDataList.headerHeight = 0;
        _recordDataList.elementHeight = 80;
        _recordDataList.drawElementCallback = DrawRecordDataElement;
    }

    void DrawRecordDataElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        _recordIndex = 0;
        rect.y += 2;
        EditorGUIUtility.labelWidth = 60;
        var data = _recordDataList.list[index] as NoteRecordData;
        data.CatalogType = (E_CatalogType)EditorGUI.EnumPopup(GetRecordDataRect(rect), "类型", data.CatalogType);
        data.Title = EditorGUI.TextField(GetRecordDataRect(rect), "标题", data.Title);
        data.Page = EditorGUI.IntField(GetRecordDataRect(rect), "页数", data.Page);
        //data.Text = EditorGUI.TextArea(GetRecordDataRect(rect, 3), data.Text);

        data.Text = (Sprite)EditorGUI.ObjectField(GetRecordDataRect(rect), "内容", data.Text, typeof(Sprite), false);

    }

    int _recordIndex = 0;
    Rect GetRecordDataRect(Rect rect, int count = 1)
    {
        var result = new Rect(rect.x, rect.y + _recordIndex * EditorGUIUtility.singleLineHeight, rect.width, count * EditorGUIUtility.singleLineHeight);
        _recordIndex += count;
        return result;
    }

    void AddDialogueAsset(ReorderableList reorderable)
    {
        DialogueAsset dialogueAsset = ScriptableObject.CreateInstance<DialogueAsset>();
        if (reorderable.list.Count > 0)
        {
            var list = reorderable.list as List<DialogueAsset>;
            var maxId = list.Max(i => i.DialogueId);
            var asset = list.Find(i => i.DialogueId == maxId);
            dialogueAsset = asset.Copy();
            dialogueAsset.DialogueId = maxId + 1;
        }
        else
        {
            dialogueAsset.DialogueId = 1;
        }
        dialogueAsset.name = dialogueAsset.DialogueId.ToString();
        _curDialogAsset.DialogueAssetList.Add(dialogueAsset);

        AssetDatabase.AddObjectToAsset(dialogueAsset, _curDialogAsset);
        AssetDatabase.SaveAssets();
    }

    void RemoveDialogueAsset(ReorderableList reorderable)
    {
        _curDialogAsset.DialogueAssetList.Remove(_curDialogueAsset);
        AssetDatabase.RemoveObjectFromAsset(_curDialogueAsset);
        AssetDatabase.SaveAssets();
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

    E_LanguageType _lastLanguageType = E_LanguageType.CN;
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
        else
        {
            asset.TalkType = (E_TalkType)EditorGUILayout.EnumPopup("内容类型", asset.TalkType);
        }
        asset.MaskType = (E_MaskType)EditorGUILayout.EnumPopup("显示动画", asset.MaskType);
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
        }
        EditorGUILayout.BeginHorizontal();
        asset.LanguageType = (E_LanguageType)EditorGUILayout.EnumPopup("语言类型", asset.LanguageType);
        if (asset.LanguageType != _lastLanguageType)
        {
            _lastLanguageType = asset.LanguageType;
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("事件列表");
        _eventScrollPos = EditorGUILayout.BeginScrollView(_eventScrollPos, GUILayout.Height(90));
        _talkEventList?.DoLayoutList();
        EditorGUILayout.EndScrollView();


        GUILayout.Label("对话内容");
        EditorGUILayout.BeginHorizontal();
        asset.LanguageDict.TryGetValue(asset.LanguageType, out ContentData data);
        data.Content = EditorGUILayout.TextArea(data.Content, GUILayout.Height(80));
        if (GUILayout.Button("生成", GUILayout.Width(80), GUILayout.Height(80)))
        {
            data.WordList = new List<TyperRhythm>();


            for (int i = 0; i < data.Content.Length; i++)
            {
                bool isDirective = false;
                string word = null;
                if (data.Content[i] == '<')
                {
                    int count = 0;
                    for (int j = i; j < data.Content.Length; j++)
                    {
                        word += data.Content[j];
                        if (data.Content[j] == '>')
                        {
                            Regex reg = new Regex(@"<c=[0-9a-fA-F]{6}");
                            if (word.Equals("<b>") || word.Equals("</b>") || reg.IsMatch(word) || word.Equals("</c>"))
                            {
                                isDirective = true;
                            }
                            break;
                        }
                        count++;
                    }
                    if (isDirective)
                        i += count;
                    else
                        word = data.Content[i].ToString();
                }
                else
                {
                    word = data.Content[i].ToString();
                }
                TyperRhythm typerWord = new TyperRhythm(word, 0.1f, isDirective);
                data.WordList.Add(typerWord);
            }

            //SaveDialogAsset(_curDialogAsset, _curDialogAsset);
        }
        EditorGUILayout.EndHorizontal();
        if (data.WordList.Count > 0)
        {
            DrawTyperWordList(data.WordList);
        }


    }

    float _wordWidth = 30;
    void DrawTyperWordList(List<TyperRhythm> wordList)
    {
        int count = 0;
        EditorGUILayout.BeginHorizontal();
        foreach (var word in wordList)
        {
            if (word.IsDrective)
            {
                if (word.Word.StartsWith("<color=#"))
                {
                    Color newColor;
                    string colorStr = word.Word.Substring(7, 7);
                    ColorUtility.TryParseHtmlString(colorStr, out newColor);
                    _wordTextStyle.normal.textColor = newColor;
                }
                else if (word.Word.Equals("</color>"))
                {
                    _wordTextStyle.normal.textColor = Color.white;
                }
                else if (word.Word.Equals("<b>"))
                {
                    _wordTextStyle.fontStyle = FontStyle.Bold;
                }
                else if (word.Word.Equals("</b>"))
                {
                    _wordTextStyle.fontStyle = FontStyle.Normal;
                }
                continue;
            }
            count++;
            if (count * (_wordWidth + 7) >= _talkAssetRectWidth)
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



    #region BeforeDialogPanel
    void DrawBeforeDialogPanel()
    {
        _curDialogAsset.OptionName = EditorGUILayout.TextField("选项名称", _curDialogAsset.OptionName);
        _curDialogAsset.UnLockType = (E_UnLockType)EditorGUILayout.EnumPopup("解锁条件", _curDialogAsset.UnLockType);
        switch (_curDialogAsset.UnLockType)
        {
            case E_UnLockType.None:
                _curDialogAsset.NeedDialogAsset = null;
                break;
            case E_UnLockType.Talked:
                _curDialogAsset.NeedDialogAsset = (DialogAsset)EditorGUILayout.ObjectField("所需对话", _curDialogAsset.NeedDialogAsset, typeof(DialogAsset), false);
                break;
        }
    }

    #endregion



    #region DialogPanel
    Rect _talkAssetRect;
    float _talkAssetRectWidth => position.width - 230;
    Vector2 _scrollPos;
    void DrawDialogPanel()
    {
        EditorGUILayout.BeginHorizontal();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Width(200));
        _dialogueList.DoLayoutList();
        EditorGUILayout.EndScrollView();
        _talkAssetRect = new Rect(210, 60, _talkAssetRectWidth, position.height - 70);
        GUILayout.BeginArea(_talkAssetRect);
        DrawDialogueAsset(_curDialogueAsset);
        GUILayout.EndArea();
        EditorGUILayout.EndHorizontal();
    }
    #endregion



    #region AfterDialogPanel
    Vector2 _afterScrollPos;
    void DrawAfterDialogPanel()
    {
        _afterScrollPos = EditorGUILayout.BeginScrollView(_afterScrollPos);
        _curDialogAsset.TalkEndEventType = (E_TalkEndEventType)EditorGUILayout.EnumPopup("结束事件类型", _curDialogAsset.TalkEndEventType);
        switch (_curDialogAsset.TalkEndEventType)
        {
            case E_TalkEndEventType.Transition:
                _curDialogAsset.MaskType = (E_MaskType)EditorGUILayout.EnumPopup("过渡类型", _curDialogAsset.MaskType);
                _curDialogAsset.LinkedDialogAsset = (DialogAsset)EditorGUILayout.ObjectField("连接对话", _curDialogAsset.LinkedDialogAsset, typeof(DialogAsset), false);
                break;
            case E_TalkEndEventType.Night:
                _curDialogAsset.LinkedDialogAsset = (DialogAsset)EditorGUILayout.ObjectField("连接对话", _curDialogAsset.LinkedDialogAsset, typeof(DialogAsset), false);
                _recordDataList?.DoLayoutList();
                break;
            case E_TalkEndEventType.Select:
                _talkEndEventList.DoLayoutList();
                break;
            case E_TalkEndEventType.GameOver:
                break;
        }
        EditorGUILayout.EndScrollView();
    }

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
        if (_curDialogAsset != null)
        {
            string path = AssetDatabase.GetAssetPath(_curDialogAsset);
            if (GUILayout.Button("另存为", GUILayout.Width(80)))
            {
                SaveAsDialogAsset(_curDialogAsset);
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
            RefreshAsset(_curDialogAsset);
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
        //_curDialogAsset = _curDialogAsset.Copy();
        RefreshAsset(_curDialogAsset);
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
        var tempAsset = ScriptableObject.CreateInstance<DialogAsset>();
        EditorUtility.CopySerialized(newAsset, tempAsset);
        AssetDatabase.CreateAsset(tempAsset, assetPath);

        //foreach (var talkAsset in newAsset.DialogueAssetList)
        //{
        //    talkAsset.name = talkAsset.DialogueId.ToString();
        //    AssetDatabase.AddObjectToAsset(talkAsset, newAsset);
        //}
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        _curDialogAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DialogAsset)) as DialogAsset;
        return true;
    }
    #endregion
}
