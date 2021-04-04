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

    Slider Slider_MasterVolume;
    Slider Slider_DubVolume;
    Slider Slider_BgmVolume;
    Slider Slider_AudioVolume;

    Slider Slider_WordSize;
    Slider Slider_DialogAlpha;
    Slider Slider_TyperSpeed;

    Toggle Toggle_ScreenMode;
    Toggle Toggle_SkipUnRead;
    Toggle Toggle_ShowShortcutKey;

    Button Button_Reset;


    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();

        Slider_MasterVolume = GetUI<Slider>("Slider_MasterVolume");
        Slider_DubVolume = GetUI<Slider>("Slider_DubVolume");
        Slider_BgmVolume = GetUI<Slider>("Slider_BgmVolume");
        Slider_AudioVolume = GetUI<Slider>("Slider_AudioVolume");

        Slider_WordSize = GetUI<Slider>("Slider_WordSize");
        Slider_DialogAlpha = GetUI<Slider>("Slider_DialogAlpha");
        Slider_TyperSpeed = GetUI<Slider>("Slider_TyperSpeed");

        Toggle_ScreenMode = GetUI<Toggle>("Toggle_ScreenMode");
        Toggle_SkipUnRead = GetUI<Toggle>("Toggle_SkipUnRead");
        Toggle_ShowShortcutKey = GetUI<Toggle>("Toggle_ShowShortcutKey");

        Button_Reset = GetUI<Button>("Button_Reset");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddToggleListen(Toggle_ScreenMode, SetIsFullScreen);
        AddToggleListen(Toggle_SkipUnRead, SetSkipUnRead);
        AddToggleListen(Toggle_ShowShortcutKey, SetShowShortcutKey);

        AddButtonListen(Button_Reset, OnButtonClickResetGameConfig);

        AddSliderListen(Slider_MasterVolume, (value) => SetVolume(E_VolumeType.Master, (int)value));
        AddSliderListen(Slider_DubVolume, (value) => SetVolume(E_VolumeType.Dub, (int)value));
        AddSliderListen(Slider_BgmVolume, (value) => SetVolume(E_VolumeType.Bgm, (int)value));
        AddSliderListen(Slider_AudioVolume, (value) => SetVolume(E_VolumeType.Audio, (int)value));

        AddSliderListen(Slider_WordSize, (value) => SetWordSizeLevel((int)value));
        AddSliderListen(Slider_DialogAlpha, (value) => SetDialogAlphaLevel((int)value));
        AddSliderListen(Slider_TyperSpeed, (value) => SetTyperSpeedLevel((int)value));
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        Slider_MasterVolume.value = GameConfigData.Instance.MasterVolumeLevel;
        Slider_DubVolume.value = GameConfigData.Instance.DubVoumeLevel ;
        Slider_BgmVolume.value = GameConfigData.Instance.BgmVolumeLevel;
        Slider_AudioVolume.value = GameConfigData.Instance.AudioVolumeLevel;

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
    void SetVolume(E_VolumeType volumeType, int level)
    {
        switch (volumeType)
        {
            case E_VolumeType.Master:
                GameConfigData.Instance.MasterVolumeLevel = level;
                Slider_MasterVolume.value = level;
                break;
            case E_VolumeType.Bgm:
                GameConfigData.Instance.BgmVolumeLevel = level;
                Slider_BgmVolume.value = level;
                break;
            case E_VolumeType.Audio:
                GameConfigData.Instance.AudioVolumeLevel = level;
                Slider_AudioVolume.value = level;
                break;
            case E_VolumeType.Dub:
                GameConfigData.Instance.DubVoumeLevel = level;
                Slider_DubVolume.value = level;
                break;
        }
        AudioManager.Instance.SetVolume(volumeType, level / 10f);
    }
    #endregion


    void SetWordSizeLevel(int level)
    {
        GameConfigData.Instance.WordSizeLevel = level;
        ActionManager.Instance.Invoke(ActionType.SetWordSize);
    }


    void SetDialogAlphaLevel(int level)
    {
        GameConfigData.Instance.DialogAlphaLevel = level;
        ActionManager.Instance.Invoke(ActionType.SetDialogAlpha);
    }

    void SetTyperSpeedLevel(int level)
    {
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
