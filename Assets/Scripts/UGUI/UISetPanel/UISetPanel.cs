﻿using System.Collections;
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
    string[] _screenModeTitleArray = new string[2] { "全屏", "窗口化" };
    Text Text_ScreenMode;
    Button Button_LeftScreenMode;
    Button Button_RightScreenMode;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Text_ScreenMode = GetUI<Text>("Text_ScreenMode");
        Button_LeftScreenMode = GetUI<Button>("Button_LeftScreenMode");
        Button_RightScreenMode = GetUI<Button>("Button_RightScreenMode");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddButtonListen(Button_LeftScreenMode, OnClickButtonLeftScreenMode);
        AddButtonListen(Button_RightScreenMode, OnClickButtonRightScreenMode);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        _screenModeIndex = GameConfigData.Instance.IsFullScreen ? 0 : 1;
        Text_ScreenMode.text = _screenModeTitleArray[_screenModeIndex];
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

    void OnClickButtonLeftScreenMode()
    {
        _screenModeIndex = (_screenModeIndex + 1) % _screenModeTitleArray.Length;
        SetIsFullScreen(_screenModeIndex == 0);
        Text_ScreenMode.text = _screenModeTitleArray[_screenModeIndex];
    }
    void OnClickButtonRightScreenMode()
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

    void SetVolume(E_VolumeType volumeType, int level)
    {
        switch (volumeType)
        {
            case E_VolumeType.Master:
                GameConfigData.Instance.MasterVolumeLevel = level;
                break;
            case E_VolumeType.Bgm:
                GameConfigData.Instance.BgmVolumeLevel = level;
                break;
            case E_VolumeType.Audio:
                GameConfigData.Instance.AudioVolumeLevel = level;
                break;
            case E_VolumeType.Dub:
                GameConfigData.Instance.DubVoumeLevel = level;
                break;
        }
        AudioManager.Instance.SetVolume(volumeType, level / 10f);
    }

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

    void ResetGameConfig()
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
