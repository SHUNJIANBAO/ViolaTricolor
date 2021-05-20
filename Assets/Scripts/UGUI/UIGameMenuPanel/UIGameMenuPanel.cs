using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbFramework;//using PbUISystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIGameMenuPanel : UIPanelBase
{
    #region 参数
    Button Button_Set;
    Button Button_Note;
    Button Button_Load;
    Button Button_Save;
    Button Button_MainMenu;

    GameObject Image_Selected;
    //Button Button_ExitGame;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Button_Set = GetUI<Button>("Button_Set");
        Button_Note = GetUI<Button>("Button_Note");
        Button_Load = GetUI<Button>("Button_Load");
        Button_Save = GetUI<Button>("Button_Save");
        Button_MainMenu = GetUI<Button>("Button_MainMenu");
        //Button_ExitGame = GetUI<Button>("Button_ExitGame");

        Image_Selected = GetUI<GameObject>("Image_Selected");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddButtonListen(Button_Set, OnClickButtonSet);
        AddButtonListen(Button_MainMenu, OnClickButtonOpenMainMenu);
        //AddButtonListen(Button_ExitGame, OnClickButtonExitGame);

        AddOnPointerEnterListen("Button_Set",(data)=> OnPointerEnterShowSelected(Button_Set.transform,data));
        AddOnPointerEnterListen("Button_Note", (data) => OnPointerEnterShowSelected(Button_Note.transform, data));
        AddOnPointerEnterListen("Button_Load", (data) => OnPointerEnterShowSelected(Button_Load.transform, data));
        AddOnPointerEnterListen("Button_Save", (data) => OnPointerEnterShowSelected(Button_Save.transform, data));
        AddOnPointerEnterListen("Button_MainMenu", (data) => OnPointerEnterShowSelected(Button_MainMenu.transform, data));

        AddOnPointerExitListen("Button_Set", OnPointerExitHideSelected);
        AddOnPointerExitListen("Button_Note", OnPointerExitHideSelected);
        AddOnPointerExitListen("Button_Load", OnPointerExitHideSelected);
        AddOnPointerExitListen("Button_Save", OnPointerExitHideSelected);
        AddOnPointerExitListen("Button_MainMenu", OnPointerExitHideSelected);
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
        Image_Selected.SetActive(false);
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


    void OnClickButtonSet()
    {
        UIManager.Instance.OpenPanel<UISetPanel>();
    }

    void OnClickButtonOpenMainMenu()
    {
        UIManager.Instance.OpenPanel<UIGameExitPanel>();
        //UIManager.Instance.CloseAllNormalPanel();
        //UIManager.Instance.OpenPanel<UIMainMenuPanel>();
    }

    void OnClickButtonExitGame()
    {
        UIManager.Instance.OpenPanel<UIGameExitPanel>();
    }


    void OnPointerEnterShowSelected(Transform parent, BaseEventData data)
    {
        Image_Selected.SetActive(true);
        Image_Selected.transform.SetParent(parent, false);
    }

    void OnPointerExitHideSelected(BaseEventData data)
    {
        Image_Selected.SetActive(false);
    }
    #endregion
}
