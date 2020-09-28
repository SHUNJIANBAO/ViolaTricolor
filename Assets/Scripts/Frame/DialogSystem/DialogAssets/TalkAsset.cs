using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkAsset : ScriptableObject
{
    public int TalkId;
    //public Sprite Icon;
    public Body Body;
    public Sprite FaceSprite;
    public AnimationClip FaceAnimation;
    public Sprite Background;
    public AudioClip Bgm;
    public int DubDelay;
    public AudioClip Dub;
    public string TalkerName;
    public E_BodyPos BodyPos;
    public E_TalkEvent TalkEvent;

    public E_DialogType DialogType;
    public bool IsNewTalk;
    public bool IsNewPage;

    public List<AudioEvent> AudioEventList = new List<AudioEvent>();
    //public List<TyperRhythm> WordList = new List<TyperRhythm>();

    public string Content;

    public TalkAsset Copy()
    {
        TalkAsset asset = ScriptableObject.CreateInstance<TalkAsset>();
        asset.TalkId = TalkId;
        //asset.Icon = Icon;
        asset.Body = Body;
        asset.FaceSprite = FaceSprite;
        asset.FaceAnimation = FaceAnimation;
        asset.Background = Background;
        asset.Bgm = Bgm;
        asset.DubDelay = DubDelay;
        asset.Dub = Dub;
        asset.TalkerName = TalkerName;
        asset.AudioEventList = new List<AudioEvent>(AudioEventList);
        asset.BodyPos = BodyPos;
        asset.TalkEvent = TalkEvent;
        asset.Content = Content;
        asset.DialogType = DialogType;
        asset.IsNewTalk = IsNewTalk;
        asset.IsNewPage = IsNewPage;
        //asset.WordList = new List<TyperRhythm>(WordList);
        return asset;
    }
}

[System.Flags]
public enum E_TalkEvent
{
    Nothing,
    Shake,//抖动
    Magnify,//放大
    Sway,  //左右移动
}

public enum E_BodyPos
{
    None,
    Left,
    Right,
    Center,
    Bottom,
    OnlyLeft,
    OnlyRight,
    OnlyCenter,
    OnlyBottom,
}

public enum E_DialogType
{
    Normal,
    FullScreen
}

[System.Serializable]
public class AudioEvent
{
    public int AudioDelay; //音效延迟多少个字播放
    public AudioClip Audio;
}

[System.Serializable]
public struct TyperRhythm
{
    public char Word;
    public float WaitTime;
    public TyperRhythm(char word, float waitTime)
    {
        Word = word;
        WaitTime = waitTime;
    }
}