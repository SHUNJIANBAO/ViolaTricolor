﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbFramework;//using PbUISystem;

[RequireComponent(typeof(CanvasGroup))]
public class UIGameExitPanel : UIPanelBase
{
    #region 参数
    Button Button_Enter;
    Button Button_Cancel;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Button_Enter = GetUI<Button>("Button_Enter");
        Button_Cancel = GetUI<Button>("Button_Cancel");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddButtonListen(Button_Enter, OnClickButtonEnter);
        AddButtonListen(Button_Cancel, OnClickButtonCancel);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
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
    void OnClickButtonEnter()
    {
        DialogManager.IsTalking = false;
        UIManager.Instance.CloseAllNormalPanel();
        UIManager.Instance.OpenPanel<UIMainMenuPanel>();
       // Application.Quit();
    }

    void OnClickButtonCancel()
    {
        UIManager.Instance.ClosePanel(this);
    }

    #endregion
}
