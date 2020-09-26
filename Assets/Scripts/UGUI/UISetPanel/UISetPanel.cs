using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbUISystem;
using PbAudioSystem;

[RequireComponent(typeof(CanvasGroup))]
public class UISetPanel : UIPanelBase
{
    #region 参数
    int _screenModeIndex;
    //int _wordSizeIndex;
    //int _typerSpeedIndex;
    //int _dialogAlphaIndex;
    string[] _screenModeTitleArray = new string[2] { "全屏", "窗口化" };
    string[] _wordSizeTitleArray = new string[3] { "小", "中", "大" };
    string[] _typerSpeedTitleArray = new string[3] { "慢", "中", "快" };
    string[] _dialogAlphaTitleArray = new string[3] { "低", "中", "高" };

    Text Text_ScreenMode;
    Text Text_WordSize;
    Text Text_TyperSpeed;
    Text Text_DialogAlpha;
    Image Image_MasterVolume;
    Image Image_BgmVolume;
    Image Image_DubVolume;
    Image Image_AudioVolume;

    Toggle Toggle_SkipUnRead;
    Toggle Toggle_ShowShortcutKey;

    Button Button_Reset;
    Button Button_LeftScreenMode;
    Button Button_RightScreenMode;
    Button Button_LeftWordSize;
    Button Button_RightWordSize;
    Button Button_LeftTyperSpeed;
    Button Button_RightTyperSpeed;
    Button Button_LeftDialogAlpha;
    Button Button_RightDialogAlpha;

    Button Button_AddMasterVolume;
    Button Button_ReduceMasterVolume;
    Button Button_AddBgmVolume;
    Button Button_ReduceBgmVolume;
    Button Button_AddDubVolume;
    Button Button_ReduceDubVolume;
    Button Button_AddAudioVolume;
    Button Button_ReduceAudioVolume;

    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();

        Text_ScreenMode = GetUI<Text>("Text_ScreenMode");
        Text_WordSize = GetUI<Text>("Text_WordSize");
        Text_TyperSpeed = GetUI<Text>("Text_TyperSpeed");
        Text_DialogAlpha = GetUI<Text>("Text_DialogAlpha");

        Image_MasterVolume = GetUI<Image>("Image_MasterVolume");
        Image_BgmVolume = GetUI<Image>("Image_BgmVolume");
        Image_DubVolume = GetUI<Image>("Image_DubVolume");
        Image_AudioVolume = GetUI<Image>("Image_AudioVolume");

        Toggle_SkipUnRead = GetUI<Toggle>("Toggle_SkipUnRead");
        Toggle_ShowShortcutKey = GetUI<Toggle>("Toggle_ShowShortcutKey");

        Button_Reset = GetUI<Button>("Button_Reset");
        Button_LeftScreenMode = GetUI<Button>("Button_LeftScreenMode");
        Button_RightScreenMode = GetUI<Button>("Button_RightScreenMode");
        Button_LeftWordSize = GetUI<Button>("Button_LeftWordSize");
        Button_RightWordSize = GetUI<Button>("Button_RightWordSize");
        Button_LeftTyperSpeed = GetUI<Button>("Button_LeftTyperSpeed");
        Button_RightTyperSpeed = GetUI<Button>("Button_RightTyperSpeed");
        Button_LeftDialogAlpha = GetUI<Button>("Button_LeftDialogAlpha");
        Button_RightDialogAlpha = GetUI<Button>("Button_RightDialogAlpha");
        Button_AddMasterVolume = GetUI<Button>("Button_AddMasterVolume");
        Button_ReduceMasterVolume = GetUI<Button>("Button_ReduceMasterVolume");
        Button_AddBgmVolume = GetUI<Button>("Button_AddBgmVolume");
        Button_ReduceBgmVolume = GetUI<Button>("Button_ReduceBgmVolume");
        Button_AddDubVolume = GetUI<Button>("Button_AddDubVolume");
        Button_ReduceDubVolume = GetUI<Button>("Button_ReduceDubVolume");
        Button_AddAudioVolume = GetUI<Button>("Button_AddAudioVolume");
        Button_ReduceAudioVolume = GetUI<Button>("Button_ReduceAudioVolume");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddToggleListen(Toggle_SkipUnRead, SetSkipUnRead);
        AddToggleListen(Toggle_ShowShortcutKey, SetShowShortcutKey);

        AddButtonListen(Button_Reset, OnButtonClickResetGameConfig);
        AddButtonListen(Button_LeftScreenMode, OnButtonClickLeftScreenMode);
        AddButtonListen(Button_RightScreenMode, OnButtonClickRightScreenMode);
        AddButtonListen(Button_LeftWordSize, OnButtonClickLeftWordSize);
        AddButtonListen(Button_RightWordSize, OnButtonClickRightWordSize);
        AddButtonListen(Button_LeftTyperSpeed, OnButtonClickLeftTyperSpeed);
        AddButtonListen(Button_RightTyperSpeed, OnButtonClickRightTyperSpeed);
        AddButtonListen(Button_LeftDialogAlpha, OnButtonClickLeftDialogAlpha);
        AddButtonListen(Button_RightDialogAlpha, OnButtonClickRightDialogAlpha);
        AddButtonListen(Button_AddMasterVolume, OnButtonClickAddMasterVolume);
        AddButtonListen(Button_ReduceMasterVolume, OnButtonClickReduceMasterVolume);
        AddButtonListen(Button_AddBgmVolume, OnButtonClickAddBgmVolume);
        AddButtonListen(Button_ReduceBgmVolume, OnButtonClickReduceBgmVolume);
        AddButtonListen(Button_AddDubVolume, OnButtonClickAddDubVolume);
        AddButtonListen(Button_ReduceDubVolume, OnButtonClickReduceDubVolume);
        AddButtonListen(Button_AddAudioVolume, OnButtonClickAddAudioVolume);
        AddButtonListen(Button_ReduceAudioVolume, OnButtonClickReduceAudioVolume);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        _screenModeIndex = GameConfigData.Instance.IsFullScreen ? 0 : 1;
        Text_ScreenMode.text = _screenModeTitleArray[_screenModeIndex];
        Text_WordSize.text = _wordSizeTitleArray[GameConfigData.Instance.WordSizeLevel];
        Text_TyperSpeed.text = _typerSpeedTitleArray[GameConfigData.Instance.TyperSpeedLevel];
        Text_DialogAlpha.text = _dialogAlphaTitleArray[GameConfigData.Instance.DialogAlphaLevel];
        Image_MasterVolume.fillAmount = GameConfigData.Instance.MasterVolumeLevel / 10f;
        Image_BgmVolume.fillAmount = GameConfigData.Instance.BgmVolumeLevel / 10f;
        Image_DubVolume.fillAmount = GameConfigData.Instance.DubVoumeLevel / 10f;
        Image_AudioVolume.fillAmount = GameConfigData.Instance.AudioVolumeLevel / 10f;
        Toggle_SkipUnRead.isOn = GameConfigData.Instance.IsSkipUnRead;
        Toggle_ShowShortcutKey.isOn = GameConfigData.Instance.IsShowShortcutKey;
    }


    /// <summary>
    /// 打开界面时调用
    /// </summary>
    /// <param name="objs"></param>
    public override void OnOpen(params object[] objs)
    {
        base.OnOpen(objs);
    }

    /// <summary>
    /// 获得焦点时
    /// </summary>
    public override void OnFocus()
    {
        base.OnFocus();
    }

    /// <summary>
    /// 失去焦点时
    /// </summary>
    public override void OnLostFocus()
    {
        base.OnLostFocus();
    }

    /// <summary>
    /// 刷新界面
    /// </summary>
    /// <param name="args"></param>
    protected override void OnRefresh(params object[] args)
    {
        base.OnRefresh(args);
    }

    /// <summary>
    /// 关闭界面时调用
    /// </summary>
    /// <param name="objs"></param>
    public override void OnClose(params object[] objs)
    {
        base.OnClose(objs);
        GameConfigData.Save();
    }


    /// <summary>
    /// 打开时的动画效果
    /// </summary>
    /// <param name="uiCallBack"></param>
    /// <param name="objs"></param>
    /// <returns></returns>
    public override IEnumerator OpenAnim(System.Action callback, params object[] args)
    {
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.DOKill();
        m_CanvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            StartCoroutine(base.OpenAnim(callback));
        });
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// 关闭时的动画效果,
    /// </summary>
    /// <param name="uiCallBack"></param>
    /// <param name="objs"></param>
    /// <returns></returns>
    public override IEnumerator CloseAnim(System.Action callback, params object[] args)
    {
        m_CanvasGroup.blocksRaycasts = false;
        m_CanvasGroup.DOKill();
        m_CanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            StartCoroutine(base.CloseAnim(callback));
        });
        yield return new WaitForEndOfFrame();
    }
    #endregion

    #region 成员方法

    void OnButtonClickLeftScreenMode()
    {
        _screenModeIndex = (_screenModeIndex + 1) % _screenModeTitleArray.Length;
        SetIsFullScreen(_screenModeIndex == 0);
        Text_ScreenMode.text = _screenModeTitleArray[_screenModeIndex];
    }
    void OnButtonClickRightScreenMode()
    {
        _screenModeIndex = (_screenModeIndex + 1) % _screenModeTitleArray.Length;
        SetIsFullScreen(_screenModeIndex == 0);
        Text_ScreenMode.text = _screenModeTitleArray[_screenModeIndex];
    }

    void SetIsFullScreen(bool value)
    {
        GameConfigData.Instance.IsFullScreen = value;
        if (GameConfigData.Instance.IsFullScreen)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            //Screen.fullScreen = true;
        }
        else
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
            //Screen.fullScreen = false;
        }
    }

    #region Volume
    void OnButtonClickAddMasterVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.MasterVolumeLevel + 1, 0, 10);
        SetVolume(E_VolumeType.Master, level);
    }
    void OnButtonClickReduceMasterVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.MasterVolumeLevel - 1, 0, 10);
        SetVolume(E_VolumeType.Master, level);
    }
    void OnButtonClickAddBgmVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.BgmVolumeLevel + 1, 0, 10);
        SetVolume(E_VolumeType.Bgm, level);
    }
    void OnButtonClickReduceBgmVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.BgmVolumeLevel - 1, 0, 10);
        SetVolume(E_VolumeType.Bgm, level);
    }
    void OnButtonClickAddDubVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.DubVoumeLevel + 1, 0, 10);
        SetVolume(E_VolumeType.Dub, level);
    }
    void OnButtonClickReduceDubVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.DubVoumeLevel - 1, 0, 10);
        SetVolume(E_VolumeType.Dub, level);
    }
    void OnButtonClickAddAudioVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.AudioVolumeLevel + 1, 0, 10);
        SetVolume(E_VolumeType.Audio, level);
    }
    void OnButtonClickReduceAudioVolume()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.AudioVolumeLevel - 1, 0, 10);
        SetVolume(E_VolumeType.Audio, level);
    }

    void SetVolume(E_VolumeType volumeType, int level)
    {
        switch (volumeType)
        {
            case E_VolumeType.Master:
                GameConfigData.Instance.MasterVolumeLevel = level;
                Image_MasterVolume.fillAmount = level / 10f;
                break;
            case E_VolumeType.Bgm:
                GameConfigData.Instance.BgmVolumeLevel = level;
                Image_BgmVolume.fillAmount = level / 10f;
                break;
            case E_VolumeType.Audio:
                GameConfigData.Instance.AudioVolumeLevel = level;
                Image_AudioVolume.fillAmount = level / 10f;
                break;
            case E_VolumeType.Dub:
                GameConfigData.Instance.DubVoumeLevel = level;
                Image_DubVolume.fillAmount = level / 10f;
                break;
        }
        AudioManager.Instance.SetVolume(volumeType, level / 10f);
    }
    #endregion

    void OnButtonClickLeftWordSize()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.WordSizeLevel - 1, 0, 2);
        SetWordSizeLevel(level);
    }

    void OnButtonClickRightWordSize()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.WordSizeLevel + 1, 0, 2);
        SetWordSizeLevel(level);
    }

    void SetWordSizeLevel(int level)
    {
        Text_WordSize.text = _wordSizeTitleArray[level];
        GameConfigData.Instance.WordSizeLevel = level;
        ActionManager.Instance.Invoke(ActionType.SetWordSize);
    }

    void OnButtonClickLeftDialogAlpha()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.DialogAlphaLevel - 1, 0, 2);
        SetDialogAlphaLevel(level);
    }

    void OnButtonClickRightDialogAlpha()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.DialogAlphaLevel + 1, 0, 2);
        SetDialogAlphaLevel(level);
    }

    void SetDialogAlphaLevel(int level)
    {
        Text_DialogAlpha.text = _dialogAlphaTitleArray[level];
        GameConfigData.Instance.DialogAlphaLevel = level;
        ActionManager.Instance.Invoke(ActionType.SetDialogAlpha);
    }

    void OnButtonClickLeftTyperSpeed()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.TyperSpeedLevel - 1, 0, 2);
        SetTyperSpeedLevel(level);
    }

    void OnButtonClickRightTyperSpeed()
    {
        int level = Mathf.Clamp(GameConfigData.Instance.TyperSpeedLevel + 1, 0, 2);
        SetTyperSpeedLevel(level);
    }

    void SetTyperSpeedLevel(int level)
    {
        Text_TyperSpeed.text = _typerSpeedTitleArray[level];
        GameConfigData.Instance.TyperSpeedLevel = level;
        ActionManager.Instance.Invoke(ActionType.SetTyperSpeed);
    }

    void SetShowShortcutKey(bool value)
    {
        GameConfigData.Instance.IsShowShortcutKey = value;
        ActionManager.Instance.Invoke(ActionType.SetShowShortcutKey);
    }

    void SetSkipUnRead(bool value)
    {
        GameConfigData.Instance.IsSkipUnRead = value;
        ActionManager.Instance.Invoke(ActionType.SetSkipUnRead);
    }

    void OnButtonClickResetGameConfig()
    {
        SetIsFullScreen(true);
        SetVolume(E_VolumeType.Bgm, 10);
        SetVolume(E_VolumeType.Dub, 10);
        SetVolume(E_VolumeType.Audio, 10);
        SetVolume(E_VolumeType.Master, 10);
        SetWordSizeLevel(1);
        SetDialogAlphaLevel(1);
        SetTyperSpeedLevel(1);
        SetShowShortcutKey(false);
        SetSkipUnRead(false);
    }
    #endregion
}
