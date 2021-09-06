//using PbAudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbFramework;

public class DialogueAsset : ScriptableObject
{
    public int DialogueId;

    public E_MaskType MaskType;

    public E_DialogType DialogType;
    public E_TalkType TalkType;
    public bool IsNewTalk;
    public bool IsNewPage;


    public GameObject LeftBody;
    public E_BodyShowType LeftBodyShowType;
    public GameObject CenterBody;
    public E_BodyShowType CenterBodyShowType;
    public GameObject RightBody;
    public E_BodyShowType RightBodyShowType;
    public GameObject BottomBody;
    public E_BodyShowType BottomBodyShowType;


    public GameObject Background;
    public AudioClip Bgm;
    public int DubDelay;
    public AudioClip Dub;

    public string TalkerName;
    public E_NamePos NamePos;

    [System.NonSerialized]
    public E_LanguageType LanguageType;

    public LanguageTypeDict<E_LanguageType, ContentData> LanguageDict = new LanguageTypeDict<E_LanguageType, ContentData>();

#if UNITY_EDITOR
    public DialogueAsset Copy()
    {
        DialogueAsset asset = ScriptableObject.CreateInstance<DialogueAsset>();
        UnityEditor.EditorUtility.CopySerialized(this, asset);
        return asset;
    }
#endif
}

[System.Serializable]
public class ContentData
{
    public string Content;
    public List<TyperRhythm> WordList = new List<TyperRhythm>();
    public List<DelayEvent> DelayEventList = new List<DelayEvent>();
}

[System.Serializable]
public class LanguageTypeDict<Key, Value> 
{
    public List<Key> KeyList = new List<Key>();
    public List<Value> ValueList = new List<Value>();

    public int Count => KeyList.Count;

    public void Add(Key key, Value value)
    {
        if (KeyList.Contains(key))
        {
            int index = KeyList.IndexOf(key);
            ValueList[index] = value;

            return;
        }
        KeyList.Add(key);
        ValueList.Add(value);
    }

    public void Remove(Key key)
    {
        int index = KeyList.IndexOf(key);
        KeyList.RemoveAt(index);
        ValueList.RemoveAt(index);

    }

    public bool TryGetValue(Key key, out Value value)
    {
        int index = KeyList.IndexOf(key);
        if (ValueList.Count >= index)
        {
            value = ValueList[index];
            return true;
        }
        value = default(Value);
        return false;
    }
}

public enum E_LanguageType
{
    [InspectorName("中文")] CN,
    [InspectorName("日文")] JP,
    [InspectorName("英文")] EN
}

public enum E_BodyShowType
{
    [InspectorName("隐藏")] Hide = 1,
    [InspectorName("显示")] Show = 0,
    [InspectorName("高亮")] Highlight = 2,
    [InspectorName("置灰")] Gray = 3,
}

public enum E_NamePos
{
    [InspectorName("无")] None,
    [InspectorName("左")] Left,
    [InspectorName("右")] Right
}

public enum E_BodyPos
{
    [InspectorName("左")] Left,
    [InspectorName("右")] Right,
    [InspectorName("中")] Center,
    [InspectorName("下")] Bottom,
}

public enum E_DialogType
{
    [InspectorName("普通")] Normal,
    [InspectorName("全屏")] FullScreen
}

public enum E_TalkType
{
    [InspectorName("讲话")] Talk,
    [InspectorName("思考")] Think,
    [InspectorName("旁白")] Voiceover
}

public enum E_EventType
{
    [InspectorName("播放音效")] PlayAudio,
    [InspectorName("切换立绘")] ChangeBody,
    [InspectorName("场景名")] Hint,
    [InspectorName("标注")] Label,
}

[System.Serializable]
public class DelayEvent
{
    public int Delay; //延迟多少个字执行
    public E_EventType EventType;

    public E_AudioType AuidoType;
    public AudioClip Audio;

    public GameObject Body;
    public E_BodyPos BodyPos;
    public E_BodyShowType BodyShowType;

    public float HintTime = -1;
    public string StringValue;
}

[System.Serializable]
public class TyperRhythm
{
    public string Word;
    public float WaitTime;
    public bool IsDrective;
    public TyperRhythm(string word, float waitTime, bool isDrective)
    {
        Word = word;
        WaitTime = waitTime;
        IsDrective = isDrective;
        if (isDrective)
        {
            Word = Word.Replace("<c=", "<color=#");
            Word = Word.Replace("</c>", "</color>");
        }

    }
}