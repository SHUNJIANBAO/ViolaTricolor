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
    enum E_ImagePosType
    {
        Left,
        Right,
        Center,
        Bottom,
    }

    #region 参数
    public float TransitionTime = 0.5f;
    float _typerSpeed;

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

    string _curName;
    Color _grayColor;

    Image Panel_TalkContent;
    TalkAsset _curTalkAsset;
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
    Image Image_LeftFace;
    Image Image_RightFace;
    Image Image_CenterFace;
    Animation Animation_LeftFace;
    Animation Animation_RightFace;
    Animation Animation_CenterFace;

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
        Image_LeftFace = GetUI<Image>("Image_LeftFace");
        Image_RightBody = GetUI<Image>("Image_RightBody");
        Image_RightFace = GetUI<Image>("Image_RightFace");
        Image_CenterBody = GetUI<Image>("Image_CenterBody");
        Image_CenterFace = GetUI<Image>("Image_CenterFace");
        Image_BottomBody = GetUI<Image>("Image_BottomBody");
        Button_Talk = GetUI<Button>("Image_BG");

        Animation_LeftFace = Image_LeftFace.GetComponent<Animation>();
        Animation_RightFace = Image_RightFace.GetComponent<Animation>();
        Animation_CenterFace = Image_CenterFace.GetComponent<Animation>();

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

        _grayColor = Color.gray;// = new Color(100 / 255f, 100 / 255f, 100 / 255f, 1);

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
        Image_LeftFace.color = Color.clear;
        Image_RightFace.color = Color.clear;
        Image_CenterFace.color = Color.clear;
        Image_LeftBody.rectTransform.position = new Vector2(-Image_LeftBody.rectTransform.rect.width, 0);
        Image_RightBody.rectTransform.position = new Vector2(Image_RightBody.rectTransform.rect.width, 0);
        //ShowDialog();
        if (objs != null && objs.Length > 0)
        {
            var asset = objs[0] as TalkAsset;

            _curName = asset.TalkerName;
            InitByAsset(asset);
            SetBodySpriteByPosType(asset.BodyPos, asset.Body, asset.FaceSprite, asset.FaceAnimation);
            ShowBodyByPosType(asset.BodyPos);
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
        if (_curTalkAsset == null)
        {
            return true;
        }
        return _curTalkAsset.DialogType == type;
    }

    public void InitByAsset(TalkAsset asset)
    {
        _curTalkAsset = asset;
        AudioManager.Instance.StopAudioByType(E_AudioType.Dub);
        AudioManager.Instance.StopAudioByType(E_AudioType.Audio);
        PlayBgm(asset.Bgm);
        SetBackground(asset.Background);
        Panel_FullScreenDialog.SetActive(asset.DialogType == E_DialogType.FullScreen);
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
        if (_curTalkAsset != null)
            SetName(_curTalkAsset.TalkerName, _curTalkAsset.BodyPos);
        Panel_TalkContent.rectTransform.DOAnchorPosY(0, 0.5f).OnComplete(() =>
        {
            _isShowDialog = true;
            callback?.Invoke();
        });
    }

    public void HideDialog(bool clearContent, Action callback = null)
    {
        if (!_isShowDialog) return;
        SetName("", E_BodyPos.None);
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
            StopTyper(_curTalkAsset.DialogType);
            return false;
        }
        if (_isBodyMoving)
        {
            return false;
        }
        //if (_curTalkAsset.DialogType == E_DialogType.Normal && !IsShowDialog)
        //{
        //    ShowDialog();
        //    //return false;
        //}

        return true;

    }

    public void Talk(TalkAsset asset)
    {
        AudioManager.Instance.StopAudioByType(E_AudioType.Dub);
        AudioManager.Instance.StopAudioByType(E_AudioType.Audio);
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
        _talkStopCoroutine = StartCoroutine(StopTalkIE(_curTalkAsset.DialogType));
    }

    IEnumerator PlayNormalTalk(TalkAsset asset)
    {
        _curTalkAsset = asset;
        PlayBgm(asset.Bgm);
        SetBackground(asset.Background);
        SetBodySpriteByPosType(asset.BodyPos, asset.Body, asset.FaceSprite, asset.FaceAnimation);
        SetAudioEventList(asset.AudioEventList);
        if (_curName != asset.TalkerName)
            ShowBodyByPosType(asset.BodyPos);
        yield return new WaitForEndOfFrame();
        if (!IsShowDialog)
        {
            ShowDialog();
        }
        while (_isBodyMoving)
        {
            yield return null;
        }
        SetName(asset.TalkerName, asset.BodyPos);
        PlayDub(asset.Dub);
        StartTyper(asset.Content);
    }

    IEnumerator PlayFullScreenTalk(TalkAsset asset)
    {
        _curTalkAsset = asset;
        SetBackground(asset.Background);
        PlayBgm(asset.Bgm);
        StartFullScreenTyper(asset.Content, asset.IsNewTalk, asset.IsNewPage);
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
            StopTyper(type);

    }

    #endregion

    #region Typer
    string _targetContent;
    void StartTyper(string content, string startStr = "")
    {
        _targetContent = startStr + content;
        Text_TalkContent.text = startStr;
        _talkCoroutine = StartCoroutine(Typer(Text_TalkContent, content));
    }

    void StartFullScreenTyper(string content, bool isNewTalk, bool isClear)
    {
        if (isClear)
        {
            Text_FullScreenTalkContent.text = "";
        }
        if (isNewTalk)
        {
            Text_FullScreenTalkContent.text += "\n\u3000";
        }
        _targetContent = Text_FullScreenTalkContent.text + content;
        _talkCoroutine = StartCoroutine(Typer(Text_FullScreenTalkContent, content));
    }

    IEnumerator Typer(Text text, string content)
    {
        _isTyping = true;
        int wordCount = 0;

        foreach (var word in content)
        {
            PlayAudioByIndex(wordCount);
            text.text += word;
            yield return new WaitForSeconds(_typerSpeed);
            wordCount++;
        }

        _isTyping = false;
    }


    //IEnumerator Typer(List<TyperRhythm> wordList)
    //{
    //    _isTyping = true;
    //    Text_TalkContent.text = "";

    //    foreach (var word in wordList)
    //    {
    //        Text_TalkContent.text += word.Word;
    //        if (word.WaitTime > 0).
    //            yield return new WaitForSeconds(word.WaitTime);
    //    }

    //    _isTyping = false;
    //}

    void StopTyper(E_DialogType type)
    {
        if (_talkCoroutine != null)
            StopCoroutine(_talkCoroutine);
        //Text_TalkContent.text = "";
        //foreach (var word in _curTalkAsset.WordList)
        //{
        //    Text_TalkContent.text += word.Word;
        //}
        switch (type)
        {
            case E_DialogType.Normal:
                Text_TalkContent.text = _targetContent;
                break;
            case E_DialogType.FullScreen:
                Text_FullScreenTalkContent.text = _targetContent;
                break;
        }
        _isTyping = false;
    }
    #endregion

    #region Name
    void SetName(string roleName, E_BodyPos pos)
    {
        _curName = roleName;
        if (string.IsNullOrEmpty(roleName))
        {
            Panel_LeftName.DOFade(0, TransitionTime);
            Panel_RightName.DOFade(0, TransitionTime);
            return;
        }
        switch (pos)
        {
            case E_BodyPos.None:
                Panel_LeftName.DOFade(0, TransitionTime);
                Panel_RightName.DOFade(0, TransitionTime);
                break;
            case E_BodyPos.Left:
            case E_BodyPos.OnlyLeft:
                Text_LeftName.text = roleName;
                Panel_LeftName.DOFade(1, TransitionTime);
                Panel_RightName.DOFade(0, TransitionTime);
                break;
            case E_BodyPos.Right:
            case E_BodyPos.OnlyRight:
                Text_RightName.text = roleName;
                Panel_LeftName.DOFade(0, TransitionTime);
                Panel_RightName.DOFade(1, TransitionTime);
                break;
            case E_BodyPos.Center:
            case E_BodyPos.OnlyCenter:
                Text_RightName.text = roleName;
                Panel_LeftName.DOFade(0, TransitionTime);
                Panel_RightName.DOFade(1, TransitionTime);
                break;
            case E_BodyPos.Bottom:
            case E_BodyPos.OnlyBottom:
                Text_LeftName.text = roleName;
                Panel_LeftName.DOFade(1, TransitionTime);
                Panel_RightName.DOFade(0, TransitionTime);
                break;
        }
    }
    #endregion

    #region Body
    void SetBodySpriteByPosType(E_BodyPos pos, Body body, Sprite face, AnimationClip faceAnimation)
    {
        switch (pos)
        {
            case E_BodyPos.Left:
            case E_BodyPos.OnlyLeft:
                Image_LeftBody.sprite = body.BodyImage.sprite;
                Image_LeftBody.SetNativeSize();
                Image_LeftFace.sprite = face;
                Image_LeftFace.SetNativeSize();
                Image_LeftFace.transform.localPosition = body.FaceImage.transform.localPosition;
                if (Animation_LeftFace.clip != faceAnimation)
                    Animation_LeftFace.clip = faceAnimation;
                Image_LeftFace.gameObject.SetActive(Image_LeftFace.sprite != null);
                break;
            case E_BodyPos.Right:
            case E_BodyPos.OnlyRight:
                Image_RightBody.sprite = body.BodyImage.sprite;
                Image_RightBody.SetNativeSize();
                Image_RightFace.sprite = face;
                Image_RightFace.SetNativeSize();
                Image_RightFace.transform.localPosition = body.FaceImage.transform.localPosition;
                if (Animation_RightFace.clip != faceAnimation)
                    Animation_RightFace.clip = faceAnimation;
                Image_RightFace.gameObject.SetActive(Image_RightFace.sprite != null);
                break;
            case E_BodyPos.Center:
            case E_BodyPos.OnlyCenter:
                Image_CenterBody.sprite = body.BodyImage.sprite;
                Image_CenterBody.SetNativeSize();
                Image_CenterFace.sprite = face;
                Image_CenterFace.SetNativeSize();
                Image_CenterFace.transform.localPosition = body.FaceImage.transform.localPosition;
                if (Animation_CenterFace.clip != faceAnimation)
                    Animation_CenterFace.clip = faceAnimation;
                Image_CenterFace.gameObject.SetActive(Image_CenterFace.sprite != null);
                break;
            case E_BodyPos.Bottom:
            case E_BodyPos.OnlyBottom:
                Image_BottomBody.sprite = body.BodyImage.sprite;
                Image_BottomBody.SetNativeSize();
                break;
        }

    }

    void ShowBodyByPosType(E_BodyPos bodyPosType)
    {
        switch (bodyPosType)
        {
            case E_BodyPos.None:
                HideBodys();
                return;
            case E_BodyPos.Left:
                Image_LeftBody.rectTransform.position = new Vector2(-Image_LeftBody.rectTransform.rect.width, 0);
                Image_RightBody.color = Image_RightBody.color == Color.clear ? Color.clear : _grayColor;
                Image_CenterBody.color = Image_CenterBody.color == Color.clear ? Color.clear : _grayColor;
                ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
                if (Image_LeftBody.color != Color.clear)
                {
                    ShowBody(false, E_ImagePosType.Left, Image_LeftBody, () =>
                     {
                         Image_LeftBody.DOKill();
                         ShowBody(true, E_ImagePosType.Left, Image_LeftBody);
                     });
                }
                else
                {
                    ShowBody(true, E_ImagePosType.Left, Image_LeftBody);
                }
                break;
            case E_BodyPos.Right:
                Image_RightBody.rectTransform.position = new Vector2(Image_RightBody.rectTransform.rect.width, 0);
                Image_LeftBody.color = Image_LeftBody.color == Color.clear ? Color.clear : _grayColor;
                Image_CenterBody.color = Image_CenterBody.color == Color.clear ? Color.clear : _grayColor;
                ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
                if (Image_RightBody.color != Color.clear)
                {
                    ShowBody(false, E_ImagePosType.Right, Image_RightBody, () =>
                    {
                        Image_RightBody.DOKill();
                        ShowBody(true, E_ImagePosType.Right, Image_RightBody);
                    });
                }
                else
                {
                    ShowBody(true, E_ImagePosType.Right, Image_RightBody);
                }
                break;
            case E_BodyPos.Center:
                Image_LeftBody.color = Image_LeftBody.color == Color.clear ? Color.clear : _grayColor;
                Image_RightBody.color = Image_RightBody.color == Color.clear ? Color.clear : _grayColor;
                ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
                if (Image_CenterBody.color != Color.clear)
                {
                    ShowBody(false, E_ImagePosType.Center, Image_CenterBody, () =>
                    {
                        Image_CenterBody.DOKill();
                        ShowBody(true, E_ImagePosType.Center, Image_CenterBody);
                    });
                }
                else
                {
                    ShowBody(true, E_ImagePosType.Center, Image_CenterBody);
                }
                break;
            case E_BodyPos.Bottom:
                ShowBody(true, E_ImagePosType.Bottom, Image_BottomBody);
                break;
            case E_BodyPos.OnlyLeft:
                Image_LeftBody.rectTransform.position = new Vector2(-Image_LeftBody.rectTransform.rect.width, 0);
                if (Image_LeftBody.color != Color.clear)
                {
                    ShowBody(false, E_ImagePosType.Left, Image_LeftBody, () =>
                    {
                        Image_LeftBody.DOKill();
                        ShowBody(true, E_ImagePosType.Left, Image_LeftBody);
                    });
                }
                else
                {
                    ShowBody(true, E_ImagePosType.Left, Image_LeftBody);
                }
                ShowBody(false, E_ImagePosType.Right, Image_RightBody);
                ShowBody(false, E_ImagePosType.Center, Image_CenterBody);
                ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
                break;
            case E_BodyPos.OnlyRight:
                Image_RightBody.rectTransform.position = new Vector2(Image_RightBody.rectTransform.rect.width, 0);
                if (Image_RightBody.color != Color.clear)
                {
                    ShowBody(false, E_ImagePosType.Right, Image_RightBody, () =>
                    {
                        Image_RightBody.DOKill();
                        ShowBody(true, E_ImagePosType.Right, Image_RightBody);
                    });
                }
                else
                {
                    ShowBody(true, E_ImagePosType.Right, Image_RightBody);
                }
                ShowBody(false, E_ImagePosType.Left, Image_LeftBody);
                ShowBody(false, E_ImagePosType.Center, Image_CenterBody);
                ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
                break;
            case E_BodyPos.OnlyCenter:
                if (Image_CenterBody.color != Color.clear)
                {
                    ShowBody(false, E_ImagePosType.Center, Image_CenterBody, () =>
                    {
                        Image_CenterBody.DOKill();
                        ShowBody(true, E_ImagePosType.Center, Image_CenterBody);
                    });
                }
                else
                {
                    ShowBody(true, E_ImagePosType.Center, Image_CenterBody);
                }
                ShowBody(false, E_ImagePosType.Left, Image_LeftBody);
                ShowBody(false, E_ImagePosType.Right, Image_RightBody);
                ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
                break;
            case E_BodyPos.OnlyBottom:
                ShowBody(false, E_ImagePosType.Left, Image_LeftBody);
                ShowBody(false, E_ImagePosType.Right, Image_RightBody);
                ShowBody(false, E_ImagePosType.Center, Image_CenterBody);
                ShowBody(true, E_ImagePosType.Bottom, Image_BottomBody);
                break;
        }

    }


    void ShowBody(bool value, E_ImagePosType bodyPos, Image bodyImage, Action callback = null)
    {
        _isBodyMoving = true;
        if (value) bodyImage.color = Color.clear;
        bodyImage.transform.SetAsLastSibling();
        bodyImage.DOKill();

        float endValue = 0;
        switch (bodyPos)
        {
            case E_ImagePosType.Left:
                endValue = value ? 0 : -bodyImage.rectTransform.rect.width;
                bodyImage.rectTransform.DOAnchorPosX(endValue, TransitionTime).OnComplete(() =>
                {
                    _isBodyMoving = false;
                    callback?.Invoke();
                });
                bodyImage.DOColor(value ? Color.white : Color.clear, TransitionTime * (value ? 1 : 2));
                Image_LeftFace.DOColor(value ? Color.white : Color.clear, TransitionTime * (value ? 1 : 2));
                break;
            case E_ImagePosType.Right:
                endValue = value ? 0 : bodyImage.rectTransform.rect.width;
                bodyImage.rectTransform.DOAnchorPosX(endValue, TransitionTime).OnComplete(() =>
                {
                    _isBodyMoving = false;
                    callback?.Invoke();
                });
                bodyImage.DOColor(value ? Color.white : Color.clear, TransitionTime * (value ? 1 : 2));
                Image_RightFace.DOColor(value ? Color.white : Color.clear, TransitionTime * (value ? 1 : 2));
                break;
            case E_ImagePosType.Center:
                bodyImage.DOColor(value ? Color.white : Color.clear, TransitionTime).OnComplete(() =>
                {
                    _isBodyMoving = false;
                    callback?.Invoke();
                });
                Image_CenterFace.DOColor(value ? Color.white : Color.clear, TransitionTime * (value ? 1 : 2));
                break;
            case E_ImagePosType.Bottom:
                bodyImage.color = value ? Color.white : Color.clear;
                _isBodyMoving = false;
                callback?.Invoke();
                break;
        }
    }

    public void HideBodys()
    {
        if (Image_LeftBody.color != Color.clear)
            ShowBody(false, E_ImagePosType.Left, Image_LeftBody);
        if (Image_RightBody.color != Color.clear)
            ShowBody(false, E_ImagePosType.Right, Image_RightBody);
        if (Image_CenterBody.color != Color.clear)
            ShowBody(false, E_ImagePosType.Center, Image_CenterBody);
        if (Image_BottomBody.color != Color.clear)
            ShowBody(false, E_ImagePosType.Bottom, Image_BottomBody);
    }

    #endregion

    #region Background
    void SetBackground(Sprite sprite)
    {
        if (sprite != null)
            Image_BG.sprite = sprite;
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

    Dictionary<int, AudioClip> _audioEventDict = new Dictionary<int, AudioClip>();
    void SetAudioEventList(List<AudioEvent> audioEventList)
    {
        _audioEventDict.Clear();
        foreach (var audioEvent in audioEventList)
        {
            _audioEventDict.Add(audioEvent.AudioDelay, audioEvent.Audio);
        }
    }

    void PlayAudioByIndex(int index)
    {
        if (_audioEventDict.TryGetValue(index, out AudioClip audio))
        {
            AudioManager.Instance.Play(audio, E_AudioType.Audio);
        }
    }
    #endregion

    #endregion
}
