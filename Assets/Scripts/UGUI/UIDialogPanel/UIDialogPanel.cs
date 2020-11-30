using DG.Tweening;
using PbAudioSystem;
using PbUISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIDialogPanel : UIPanelBase
{
    #region 参数
    public float TransitionTime = 0.5f;
    float _typerSpeed;

    bool _isSkip;
    bool _isSkiping;
    bool _isTyping;
    bool _isBodyMoving;
    public bool IsTalking
    {
        get { return _isTyping || _isBodyMoving; }
    }

    bool _isShowDialog;
    public bool IsShowDialog
    {
        get => _isShowDialog;
    }

    Image Panel_TalkContent;
    DialogueAsset _curDialogueAsset;
    Coroutine _talkCoroutine;
    Coroutine _talkStopCoroutine;

    Text Text_TalkContent;
    Text Text_FullScreenTalkContent;
    Button Button_Talk;
    Image Image_BG;
    Image Image_LeftBody;
    Image Image_RightBody;
    Image Image_CenterBody;
    Image Image_BottomBody;
    Animator Animator_LeftBody;
    Animator Animator_RightBody;
    Animator Animator_CenterBody;
    Animator Animator_BottomBody;

    CanvasGroup Panel_LeftName;
    CanvasGroup Panel_RightName;
    Text Text_LeftName;
    Text Text_RightName;

    GameObject Panel_FullScreenDialog;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Image_BG = GetUI<Image>("Image_BG");
        Image_LeftBody = GetUI<Image>("Image_LeftBody");
        Image_RightBody = GetUI<Image>("Image_RightBody");
        Image_CenterBody = GetUI<Image>("Image_CenterBody");
        Image_BottomBody = GetUI<Image>("Image_BottomBody");

        Animator_LeftBody = GetUI<Animator>("Image_LeftBody");
        Animator_RightBody = GetUI<Animator>("Image_RightBody");
        Animator_CenterBody = GetUI<Animator>("Image_CenterBody");
        Animator_BottomBody = GetUI<Animator>("Image_BottomBody");
        Button_Talk = GetUI<Button>("Image_BG");

        Text_TalkContent = GetUI<Text>("Text_TalkContent");
        Text_FullScreenTalkContent = GetUI<Text>("Text_FullScreenTalkContent");
        Panel_TalkContent = GetUI<Image>("Panel_TalkContent");

        Panel_LeftName = GetUI<CanvasGroup>("Panel_LeftName");
        Panel_RightName = GetUI<CanvasGroup>("Panel_RightName");
        Text_LeftName = GetUI<Text>("Text_LeftName");
        Text_RightName = GetUI<Text>("Text_RightName");
        Panel_FullScreenDialog = GetUI<GameObject>("Panel_FullScreenDialog");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        ActionManager.Instance.AddListener(ActionType.SetWordSize, SetWordSize);
        ActionManager.Instance.AddListener(ActionType.SetTyperSpeed, SetTyperSpeed);
        ActionManager.Instance.AddListener(ActionType.SetDialogAlpha, SetDialogAlpha);
        ActionManager.Instance.AddListener(ActionType.SetShowShortcutKey, SetShowShortcutKey);
        AddButtonListen(Button_Talk, () => DialogManager.Instance.Talk());
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        SetWordSize();
        SetTyperSpeed();
        SetDialogAlpha();
        SetShowShortcutKey();

        //Image_CenterBody.rectTransform.position = Image_CenterBody.rectTransform.position + Vector3.down * Image_CenterBody.rectTransform.rect.height;
        Panel_LeftName.alpha = 0;
        Panel_RightName.alpha = 0;
        _isShowDialog = false;
        Panel_TalkContent.transform.localPosition = new Vector3(0, -Panel_TalkContent.rectTransform.rect.height);
        //Panel_TalkContent.rectTransform.anchoredPosition = new Vector2(Panel_TalkContent.rectTransform.anchoredPosition.x, Panel_TalkContent.rectTransform.anchoredPosition.y - Panel_TalkContent.rectTransform.rect.height);
    }


    /// <summary>
    /// 打开界面时调用
    /// </summary>
    /// <param name="objs"></param>
    public override void OnOpen(params object[] objs)
    {
        base.OnOpen(objs);
        Text_TalkContent.text = "";
        Image_LeftBody.color = Color.clear;
        Image_RightBody.color = Color.clear;
        Image_CenterBody.color = Color.clear;
        Image_BottomBody.color = Color.clear;
        Image_LeftBody.rectTransform.position = new Vector2(-Image_LeftBody.rectTransform.rect.width, 0);
        Image_RightBody.rectTransform.position = new Vector2(Image_RightBody.rectTransform.rect.width, 0);

        if (objs != null && objs.Length > 0)
        {
            var asset = objs[0] as DialogueAsset;

            //SetMessageByAsset(asset);
            //ShowBodyByPosType(asset.BodyPos);
            ShowDialog();
        }
    }

    /// <summary>
    /// 关闭界面时调用
    /// </summary>
    /// <param name="objs"></param>
    public override void OnClose(params object[] objs)
    {
        base.OnClose(objs);
    }


    /// <summary>
    /// 打开时的动画效果
    /// </summary>
    /// <param name="uiCallBack"></param>
    /// <param name="objs"></param>
    /// <returns></returns>
    public override IEnumerator OpenAnim(System.Action callback, params object[] args)
    {
        while (_isBodyMoving || !_isShowDialog)
        {
            yield return null;
        }

        yield return StartCoroutine(base.CloseAnim(callback));
    }

    /// <summary>
    /// 关闭时的动画效果,
    /// </summary>
    /// <param name="uiCallBack"></param>
    /// <param name="objs"></param>
    /// <returns></returns>
    public override IEnumerator CloseAnim(System.Action callback, params object[] args)
    {
        HideBodys();
        HideDialog(true);
        while (_isBodyMoving || _isShowDialog)
        {
            yield return null;
        }
        yield return StartCoroutine(base.CloseAnim(callback));
    }
    #endregion

    #region 成员方法
    public bool IsSameDialogType(E_DialogType type)
    {
        if (_curDialogueAsset == null)
        {
            return true;
        }
        return _curDialogueAsset.DialogType == type;
    }

    public void SetMessageByAsset(DialogueAsset asset)
    {
        _curDialogueAsset = asset;
        AudioManager.Instance.StopAudioByType(E_AudioType.Dub);
        AudioManager.Instance.StopAudioByType(E_AudioType.Audio);
        SetDelayEventList(asset.DelayEventList);
        PlayBgm(asset.Bgm);
        SetBackground(asset.Background);
        Text_FullScreenTalkContent.text = "";
        Panel_FullScreenDialog.SetActive(asset.DialogType == E_DialogType.FullScreen);

        SetBodySpriteByPosType(E_BodyPos.Left, asset.LeftBody);
        SetBodySpriteByPosType(E_BodyPos.Right, asset.RightBody);
        SetBodySpriteByPosType(E_BodyPos.Center, asset.CenterBody);
        SetBodySpriteByPosType(E_BodyPos.Bottom, asset.BottomBody);

        ShowBodyByBodyShowType(E_BodyPos.Left, asset.LeftBodyShowType);
        ShowBodyByBodyShowType(E_BodyPos.Right, asset.RightBodyShowType);
        ShowBodyByBodyShowType(E_BodyPos.Center, asset.CenterBodyShowType);
        ShowBodyByBodyShowType(E_BodyPos.Bottom, asset.BottomBodyShowType);
    }


    void SetTyperSpeed(params object[] objs)
    {
        switch (GameConfigData.Instance.TyperSpeedLevel)
        {
            case 0:
                _typerSpeed = GameConfig.Instance.TyperSpeedLevel1;
                break;
            case 1:
                _typerSpeed = GameConfig.Instance.TyperSpeedLevel2;
                break;
            case 2:
                _typerSpeed = GameConfig.Instance.TyperSpeedLevel3;
                break;
        }
    }

    void SetWordSize(params object[] objs)
    {
        switch (GameConfigData.Instance.WordSizeLevel)
        {
            case 0:
                Text_TalkContent.fontSize = GameConfig.Instance.WordSizeLevel1;
                Text_FullScreenTalkContent.fontSize = GameConfig.Instance.WordSizeLevel1;
                break;
            case 1:
                Text_TalkContent.fontSize = GameConfig.Instance.WordSizeLevel2;
                Text_FullScreenTalkContent.fontSize = GameConfig.Instance.WordSizeLevel2;
                break;
            case 2:
                Text_TalkContent.fontSize = GameConfig.Instance.WordSizeLevel3;
                Text_FullScreenTalkContent.fontSize = GameConfig.Instance.WordSizeLevel3;
                break;
        }
    }

    void SetDialogAlpha(params object[] objs)
    {
        Color tmpColor = Panel_TalkContent.color;
        switch (GameConfigData.Instance.DialogAlphaLevel)
        {
            case 0:
                tmpColor.a = GameConfig.Instance.DialogAlphaLevel1;
                break;
            case 1:
                tmpColor.a = GameConfig.Instance.DialogAlphaLevel2;
                break;
            case 2:
                tmpColor.a = GameConfig.Instance.DialogAlphaLevel3;
                break;
        }
        Panel_TalkContent.color = tmpColor;
    }

    void SetShowShortcutKey(params object[] objs)
    {
        Debug.LogError("未实装方法:是否显示快捷键");
    }

    #region TalkController

    public void ShowDialog(Action callback = null)
    {
        if (_isShowDialog) return;
        if (_curDialogueAsset != null)
            SetName(_curDialogueAsset.TalkerName, _curDialogueAsset.NamePos);
        Panel_TalkContent.rectTransform.DOAnchorPosY(0, 0.5f).OnComplete(() =>
        {
            _isShowDialog = true;
            callback?.Invoke();
        });
    }

    public void HideDialog(bool clearContent, Action callback = null)
    {
        if (!_isShowDialog) return;
        SetName("", E_NamePos.None);
        Panel_TalkContent.rectTransform.DOAnchorPosY(-Panel_TalkContent.rectTransform.rect.height, 0.5f).OnComplete(() =>
      {
          _isShowDialog = false;
          //SetName("", E_BodyPos.None);
          if (clearContent) Text_TalkContent.text = "";
          callback?.Invoke();
      });
    }

    public bool IsCanTalk()
    {
        if (_isTyping)
        {
            StopTyper();
            return false;
        }
        if (_isBodyMoving)
        {
            return false;
        }
        //if (_curDialogueAsset.DialogType == E_DialogType.Normal && !IsShowDialog)
        //{
        //    ShowDialog();
        //    //return false;
        //}

        return true;

    }

    public void Talk(DialogueAsset asset)
    {
        SetMessageByAsset(asset);
        switch (asset.DialogType)
        {
            case E_DialogType.Normal:
                StartCoroutine(PlayNormalTalk(asset));
                break;
            case E_DialogType.FullScreen:
                StartCoroutine(PlayFullScreenTalk(asset));
                break;
        }
    }

    public void StopTalk()
    {
        if (_talkStopCoroutine != null)
            StopCoroutine(_talkStopCoroutine);
        _talkStopCoroutine = StartCoroutine(StopTalkIE(_curDialogueAsset.DialogType));
    }

    IEnumerator PlayNormalTalk(DialogueAsset asset)
    {
        _curDialogueAsset = asset;


        yield return new WaitForEndOfFrame();
        if (!IsShowDialog)
        {
            ShowDialog();
        }
        while (_isBodyMoving)
        {
            yield return null;
        }
        SetName(asset.TalkerName, asset.NamePos);
        PlayDub(asset.Dub);
        StartTyper(asset.WordList);
    }

    IEnumerator PlayFullScreenTalk(DialogueAsset asset)
    {
        _curDialogueAsset = asset;
        SetBackground(asset.Background);
        PlayBgm(asset.Bgm);
        StartFullScreenTyper(asset.WordList, asset.IsNewTalk, asset.IsNewPage);
        yield return null;
    }

    IEnumerator StopTalkIE(E_DialogType type)
    {
        switch (type)
        {
            case E_DialogType.Normal:
                while (_isBodyMoving)
                {
                    yield return null;
                }
                break;
        }
        if (_isTyping)
            StopTyper();

    }

    #endregion

    #region Typer
    //string _targetContent;
    void StartTyper(List<TyperRhythm> wordList, string startStr = "")
    {
        string content = "";
        foreach (var word in wordList)
        {
            content += word.Word;
        }
        //_targetContent = startStr + content;
        Text_TalkContent.text = startStr;
        _talkCoroutine = StartCoroutine(Typer(Text_TalkContent, wordList));
    }

    void StartFullScreenTyper(List<TyperRhythm> wordList, bool isNewTalk, bool isClear)
    {
        if (isClear)
        {
            Text_FullScreenTalkContent.text = "";
        }
        if (isNewTalk)
        {
            Text_FullScreenTalkContent.text += "\n\u3000";
        }
        string content = "";
        foreach (var word in wordList)
        {
            content += word.Word;
        }
        //_targetContent = Text_FullScreenTalkContent.text + content;
        _talkCoroutine = StartCoroutine(Typer(Text_FullScreenTalkContent, wordList));
    }

    IEnumerator Typer(Text text, List<TyperRhythm> wordList)
    {
        _isTyping = true;
        int wordCount = 0;

        foreach (var word in wordList)
        {
            PlayTalkEventByIndex(wordCount);
            text.text += word.Word;
            if (word.WaitTime > 0 && !_isSkip && !_isSkiping&& !word.IsDrective)
                yield return new WaitForSeconds(word.WaitTime * _typerSpeed);
            wordCount++;
        }
        _isSkip = false;
        _isTyping = false;
    }

    void StopTyper()
    {
        _isSkip = true;
        //if (_talkCoroutine != null)
        //    StopCoroutine(_talkCoroutine);//Text_TalkContent.text = "";
        //foreach (var word in _curDialogueAsset.WordList)
        //{
        //    Text_TalkContent.text += word.Word;
        //}
        //switch (type)
        //{
        //    case E_DialogType.Normal:
        //        Text_TalkContent.text = _targetContent;
        //        break;
        //    case E_DialogType.FullScreen:

        //        Text_FullScreenTalkContent.text = _targetContent;
        //        break;
        //}
        //_isTyping = false;
    }
    #endregion

    #region Name
    void SetName(string roleName, E_NamePos pos)
    {
        if (string.IsNullOrEmpty(roleName))
        {
            Panel_LeftName.DOFade(0, TransitionTime);
            Panel_RightName.DOFade(0, TransitionTime);
            return;
        }
        switch (pos)
        {
            case E_NamePos.None:
                Panel_LeftName.DOFade(0, TransitionTime);
                Panel_RightName.DOFade(0, TransitionTime);
                break;
            case E_NamePos.Left:
                Text_LeftName.text = roleName;
                Panel_LeftName.DOFade(1, TransitionTime);
                Panel_RightName.DOFade(0, TransitionTime);
                break;
            case E_NamePos.Right:
                Text_RightName.text = roleName;
                Panel_LeftName.DOFade(0, TransitionTime);
                Panel_RightName.DOFade(1, TransitionTime);
                break;
        }
    }
    #endregion

    #region Body
    public void ChangeBody(E_BodyPos pos, GameObject body, E_BodyShowType showType)
    {
        SetBodySpriteByPosType(pos, body);
        ShowBodyByBodyShowType(pos, showType);
    }

    void SetBodySpriteByPosType(E_BodyPos pos, GameObject body)
    {
        if (body == null) return;
        switch (pos)
        {
            case E_BodyPos.Left:
                Image_LeftBody.sprite = body.GetComponent<Image>().sprite;
                Animator_LeftBody.runtimeAnimatorController = body.GetComponent<Animator>()?.runtimeAnimatorController;
                Image_LeftBody.SetNativeSize();
                break;
            case E_BodyPos.Right:
                Image_RightBody.sprite = body.GetComponent<Image>().sprite;
                Animator_RightBody.runtimeAnimatorController = body.GetComponent<Animator>()?.runtimeAnimatorController;
                Image_RightBody.SetNativeSize();
                break;
            case E_BodyPos.Center:
                Image_CenterBody.sprite = body.GetComponent<Image>().sprite;
                Animator_CenterBody.runtimeAnimatorController = body.GetComponent<Animator>()?.runtimeAnimatorController;
                Image_CenterBody.SetNativeSize();
                break;
            case E_BodyPos.Bottom:
                Image_BottomBody.sprite = body.GetComponent<Image>().sprite;
                Animator_BottomBody.runtimeAnimatorController = body.GetComponent<Animator>()?.runtimeAnimatorController;
                Image_BottomBody.SetNativeSize();
                break;
        }

    }

    void ShowBodyByBodyShowType(E_BodyPos bodyPosType, E_BodyShowType showType)
    {
        switch (bodyPosType)
        {
            case E_BodyPos.Left:
                switch (showType)
                {
                    case E_BodyShowType.Show:
                        if (Image_LeftBody.color != Color.clear)
                        {
                            ShowBody(false, bodyPosType, Image_LeftBody, () =>
                            {
                                Image_LeftBody.DOKill();
                                ShowBody(true, bodyPosType, Image_LeftBody);
                            });
                        }
                        else
                        {
                            Image_LeftBody.rectTransform.anchoredPosition = new Vector2(-Image_LeftBody.rectTransform.rect.width, 0);
                            ShowBody(true, bodyPosType, Image_LeftBody);
                        }
                        break;
                    case E_BodyShowType.Hide:
                        ShowBody(false, bodyPosType, Image_LeftBody, () =>
                        {
                            Image_LeftBody.DOKill();
                        });
                        break;
                    case E_BodyShowType.Highlight:
                        Image_LeftBody.color = Color.white;
                        break;
                    case E_BodyShowType.Gray:
                        Image_LeftBody.color = Color.gray;
                        break;
                }
                break;
            case E_BodyPos.Right:
                switch (showType)
                {
                    case E_BodyShowType.Show:
                        if (Image_RightBody.color != Color.clear)
                        {
                            ShowBody(false, bodyPosType, Image_RightBody, () =>
                            {
                                Image_RightBody.DOKill();
                                ShowBody(true, bodyPosType, Image_RightBody);
                            });
                        }
                        else
                        {
                            Image_RightBody.rectTransform.anchoredPosition = new Vector2(Image_RightBody.rectTransform.rect.width, 0);
                            ShowBody(true, bodyPosType, Image_RightBody);
                        }
                        break;
                    case E_BodyShowType.Hide:
                        ShowBody(false, bodyPosType, Image_RightBody, () =>
                        {
                            Image_RightBody.DOKill();
                        });
                        break;
                    case E_BodyShowType.Highlight:
                        Image_RightBody.color = Color.white;
                        break;
                    case E_BodyShowType.Gray:
                        Image_RightBody.color = Color.gray;
                        break;
                }
                break;
            case E_BodyPos.Center:
                switch (showType)
                {
                    case E_BodyShowType.Show:
                        if (Image_CenterBody.color != Color.clear)
                        {
                            ShowBody(false, bodyPosType, Image_CenterBody, () =>
                            {
                                Image_CenterBody.DOKill();
                                ShowBody(true, bodyPosType, Image_CenterBody);
                            });
                        }
                        else
                        {
                            ShowBody(true, bodyPosType, Image_CenterBody);
                        }
                        break;
                    case E_BodyShowType.Hide:
                        ShowBody(false, bodyPosType, Image_CenterBody, () =>
                        {
                            Image_CenterBody.DOKill();
                        });
                        break;
                    case E_BodyShowType.Highlight:
                        Image_CenterBody.color = Color.white;
                        break;
                    case E_BodyShowType.Gray:
                        Image_CenterBody.color = Color.gray;
                        break;
                }
                break;
            case E_BodyPos.Bottom:
                switch (showType)
                {
                    case E_BodyShowType.Show:
                        ShowBody(true, bodyPosType, Image_BottomBody);
                        break;
                    case E_BodyShowType.Hide:
                        ShowBody(false, bodyPosType, Image_BottomBody);
                        break;
                    case E_BodyShowType.Highlight:
                        Image_BottomBody.color = Color.white;
                        break;
                    case E_BodyShowType.Gray:
                        Image_BottomBody.color = Color.gray;
                        break;
                }
                break;
        }
    }


    void ShowBody(bool value, E_BodyPos bodyPos, Image bodyImage, Action callback = null)
    {
        _isBodyMoving = true;
        if (value) bodyImage.color = Color.clear;
        Color targetColor = value ? Color.white : Color.clear;
        float colorTransitionTime = TransitionTime * (value ? 1 : 2);
        bodyImage.transform.SetAsLastSibling();
        bodyImage.DOKill();

        float endValue = 0;
        switch (bodyPos)
        {
            case E_BodyPos.Left:
                endValue = value ? 0 : -bodyImage.rectTransform.rect.width;
                bodyImage.rectTransform.DOAnchorPosX(endValue, TransitionTime).OnComplete(() =>
                {
                    _isBodyMoving = false;
                    bodyImage.DOKill();
                    bodyImage.color = targetColor;
                    callback?.Invoke();
                });
                bodyImage.DOColor(targetColor, colorTransitionTime);
                break;
            case E_BodyPos.Right:
                endValue = value ? 0 : bodyImage.rectTransform.rect.width;
                bodyImage.rectTransform.DOAnchorPosX(endValue, TransitionTime).OnComplete(() =>
                {
                    _isBodyMoving = false;
                    bodyImage.DOKill();
                    bodyImage.color = targetColor;
                    callback?.Invoke();
                });
                bodyImage.DOColor(targetColor, colorTransitionTime);
                break;
            case E_BodyPos.Center:
                bodyImage.DOColor(targetColor, colorTransitionTime).OnComplete(() =>
                {
                    _isBodyMoving = false;
                    bodyImage.DOKill();
                    bodyImage.color = targetColor;
                    callback?.Invoke();
                });
                break;
            case E_BodyPos.Bottom:
                bodyImage.color = targetColor;
                _isBodyMoving = false;
                callback?.Invoke();
                break;
        }
    }

    public void HideBodys()
    {
        if (Image_LeftBody.color != Color.clear)
            ShowBody(false, E_BodyPos.Left, Image_LeftBody);
        if (Image_RightBody.color != Color.clear)
            ShowBody(false, E_BodyPos.Right, Image_RightBody);
        if (Image_CenterBody.color != Color.clear)
            ShowBody(false, E_BodyPos.Center, Image_CenterBody);
        if (Image_BottomBody.color != Color.clear)
            ShowBody(false, E_BodyPos.Bottom, Image_BottomBody);
    }

    #endregion

    #region Background
    GameObject _curBg;
    void SetBackground(GameObject bg)
    {
        if (bg != null)
        {
            if (_curBg != null)
            {
                if (_curBg.name == bg.gameObject.name)
                {
                    return;
                }
                GameObject.Destroy(bg);
            }
            _curBg = GameObject.Instantiate(bg, Image_BG.transform, false);
            _curBg.name = bg.name;
            //Image_BG.sprite = sprite;
        }
    }
    #endregion

    #region Aduio
    AudioClip _currentAudio;
    void PlayBgm(AudioClip audio)
    {
        if (_currentAudio == audio) return;
        if (audio != null)
        {
            _currentAudio = audio;
            AudioManager.Instance.Play(audio, E_AudioType.Bgm, true, true);
        }
    }

    void PlayDub(AudioClip audio)
    {
        if (audio != null)
            AudioManager.Instance.Play(audio, E_AudioType.Dub, false, true);
    }

    Dictionary<int, List<TalkEvent>> _talkEventDelayDict = new Dictionary<int, List<TalkEvent>>();
    void SetDelayEventList(List<DelayEvent> eventList)
    {
        _talkEventDelayDict.Clear();
        foreach (var tempEvent in eventList)
        {
            TalkEvent tempTalkEvent = null;
            switch (tempEvent.EventType)
            {
                case E_EventType.PlayAudio:
                    tempTalkEvent = new PlayAudioEvent(tempEvent);
                    break;
                case E_EventType.ChangeBody:
                    tempTalkEvent = new ChangeBodyEvent(tempEvent);
                    break;
            }
            if (!_talkEventDelayDict.TryGetValue(tempEvent.Delay, out List<TalkEvent> tempList))
            {
                tempList = new List<TalkEvent>();
                _talkEventDelayDict.Add(tempEvent.Delay, tempList);
            }
            tempList.Add(tempTalkEvent);
        }
    }

    void PlayTalkEventByIndex(int delayIndex)
    {
        if (_talkEventDelayDict.TryGetValue(delayIndex, out List<TalkEvent> tempEventList))
        {
            foreach (var tempEvent in tempEventList)
            {
                tempEvent.Play();
            }
        }
    }
    #endregion

    #endregion
}
