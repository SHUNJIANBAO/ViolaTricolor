using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbUISystem;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UINoteBookPanel : UIPanelBase
{
    #region 参数
    BookPro BookPro;
    AutoFlip AutoFlip;
    Button Button_LeftPage;
    Button Button_RigthPage;

    Button Button_Catalog;
    Button Button_Close;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Button_Close = GetUI<Button>("Button_Close");
        Button_Catalog = GetUI<Button>("Button_Catalog");
        BookPro = GetUI<BookPro>("BookPro");
        AutoFlip = GetUI<AutoFlip>("BookPro");

        Button_LeftPage = GetUI<Button>("Button_LeftPage");
        Button_RigthPage = GetUI<Button>("Button_RigthPage");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddButtonListen(Button_Close, OnButtonClickCloseNoteBook);
        AddButtonListen(Button_Catalog, BackToCatalog);
        AddButtonListen(Button_LeftPage, FlipToLeftPage);
        AddButtonListen(Button_RigthPage, FlipToRightPage);
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
        BookPro.papers.Clear();
        CreatePage();
        CreatePage();
        CreatePage();
        CreatePage();
        CreatePage();
        BookPro.UpdatePages();
        BookPro.CurrentPaper = 1;

        ShowCatalogByType(E_CatalogType.People);
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
        //m_CanvasGroup.alpha = 0;
        //m_CanvasGroup.DOKill();
        //m_CanvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        //{
        yield return StartCoroutine(base.OpenAnim(callback));
        //});
        //yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// 关闭时的动画效果,
    /// </summary>
    /// <param name="uiCallBack"></param>
    /// <param name="objs"></param>
    /// <returns></returns>
    public override IEnumerator CloseAnim(System.Action callback, params object[] args)
    {
        //m_CanvasGroup.blocksRaycasts = false;
        //m_CanvasGroup.DOKill();
        //m_CanvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        //{
        yield return StartCoroutine(base.CloseAnim(callback));
        //});
        //yield return new WaitForEndOfFrame();
    }
    #endregion

    #region 成员方法

    void OnButtonClickCloseNoteBook()
    {
        UIManager.Instance.OpenPanel<UIMaskPanel>(true, () =>
        {
            UIManager.Instance.ClosePanel(this);
            DialogManager.Instance.Talk();
        }, E_MaskType.GameStateChange);
    }

    void OnButtonClickCatalog()
    {

    }

    int _index;
    void CreatePage()
    {
        _index++;
        var rightPage = UIManager.Instance.CreateItem<UIPageItem>(BookPro.transform);
        rightPage.SetPos(E_PagePos.Right);
        var rightPageTrans = rightPage.GetComponent<RectTransform>();
        rightPageTrans.sizeDelta = BookPro.RightPageTransform.sizeDelta;
        rightPageTrans.pivot = BookPro.RightPageTransform.pivot;
        rightPageTrans.anchoredPosition = BookPro.RightPageTransform.anchoredPosition;
        rightPageTrans.localScale = BookPro.RightPageTransform.localScale;
        rightPage.name = "Page";
        rightPage.SetText("Test Right" + _index.ToString());
        //lastElement.FindPropertyRelative("Front").objectReferenceInstanceIDValue = rightPage.GetInstanceID();

        _index++;
        var leftPage = UIManager.Instance.CreateItem<UIPageItem>(BookPro.transform);
        leftPage.SetPos(E_PagePos.Left);
        var leftPageTrans = leftPage.GetComponent<RectTransform>();
        leftPageTrans.transform.SetParent(BookPro.transform, true);
        leftPageTrans.sizeDelta = BookPro.LeftPageTransform.sizeDelta;
        leftPageTrans.pivot = BookPro.LeftPageTransform.pivot;
        leftPageTrans.anchoredPosition = BookPro.LeftPageTransform.anchoredPosition;
        leftPageTrans.localScale = BookPro.LeftPageTransform.localScale;
        leftPageTrans.name = "Page";
        leftPage.SetText("Test Left" + _index.ToString());
        //lastElement.FindPropertyRelative("Back").objectReferenceInstanceIDValue = leftPage.GetInstanceID();

        Paper paper = new Paper();
        paper.Back = leftPage.gameObject;
        paper.Front = rightPage.gameObject;
        BookPro.papers.Add(paper);

        BookPro.EndFlippingPaper = BookPro.papers.Count - 1;
    }

    void ShowCatalogByType(E_CatalogType type)
    {

    }

    void BackToCatalog()
    {
        float beforeTime = AutoFlip.PageFlipTime;

        float flipTime =beforeTime/ (BookPro.CurrentPaper - 1);
        AutoFlip.PageFlipTime = flipTime;

        WhileBack(() => AutoFlip.PageFlipTime = beforeTime); ;
    }

    void WhileBack(Action callback)
    {
        if (BookPro.CurrentPaper > 1)
        {
            AutoFlip.FlipLeftPage(()=>WhileBack(callback));
        }
        else
        {
            callback?.Invoke();
        }
    }

    void FlipToLeftPage()
    {
        if (BookPro.CurrentPaper <= 2) return;
        AutoFlip.FlipLeftPage();
    }

    void FlipToRightPage()
    {
        if (BookPro.CurrentPaper >= BookPro.papers.Count - 1) return;
        AutoFlip.FlipRightPage();
    }
    #endregion
}
