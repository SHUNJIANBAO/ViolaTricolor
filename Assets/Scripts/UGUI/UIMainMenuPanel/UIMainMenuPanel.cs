using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbUISystem;

[RequireComponent(typeof(CanvasGroup))]
public class UIMainMenuPanel : UIPanelBase
{
    #region 参数
    Button Button_NewGame;
    Button Button_Load;
    Button Button_Set;
    Button Button_Note;

    Button Button_ExitGame;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Button_NewGame = GetUI<Button>("Button_NewGame");
        Button_Load = GetUI<Button>("Button_Load");
        Button_Set = GetUI<Button>("Button_Set");
        Button_Note = GetUI<Button>("Button_Note");

        Button_ExitGame = GetUI<Button>("Button_ExitGame");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddButtonListen(Button_NewGame, OnClickButtonNewGame);
        AddButtonListen(Button_Load, OnClickButtonLoad);
        AddButtonListen(Button_Set, OnClickButtonSet);
        AddButtonListen(Button_Note, OnClickButtonNote);
        AddButtonListen(Button_ExitGame, OnClickButtonExitGame);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        //GameManager.Instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
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
    void OnClickButtonNewGame()
    {
        PlayerManager.Instance.NewGame();
    }

    void OnClickButtonLoad()
    {
        UIManager.Instance.OpenPanel<UIRecordPanel>();
    }

    void OnClickButtonSet()
    {
        UIManager.Instance.OpenPanel<UISetPanel>();
    }

    void OnClickButtonNote()
    {
        UIManager.Instance.OpenPanel<UINoteBookPanel>(true,null,true);
    }

    void OnClickButtonExitGame()
    {
        Application.Quit();
    }
    #endregion
}
