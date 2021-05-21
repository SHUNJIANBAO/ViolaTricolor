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


    public string Content;
    public List<TyperRhythm> WordList = new List<TyperRhythm>();
    public List<DelayEvent> DelayEventList = new List<DelayEvent>();

#if UNITY_EDITOR
    public DialogueAsset Copy()
    {
        DialogueAsset asset = ScriptableObject.CreateInstance<DialogueAsset>();
        UnityEditor.EditorUtility.CopySerialized(this, asset);
        return asset;
    }
#endif
}

public enum E_BodyShowType
{
    [InspectorName("隐藏")] Hide,
    [InspectorName("显示")] Show,
    [InspectorName("高亮")] Highlight,
    [InspectorName("置灰")] Gray,
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
    [InspectorName("普通")]Normal,
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
            Word=Word.Replace("<c=", "<color=#");
            Word=Word.Replace("</c>", "</color>");
        }

    }
}