using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbUISystem;
using System;
using System.Linq;
using PbFramework;

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

    Toggle Toggle_CatalogPeople;
    Toggle Toggle_CatalogThing;
    Toggle Toggle_CatalogRecord;

    UIPageItem FirstPage;

    CanvasGroup Panel_Catalog;

    E_CatalogType _currentCatalogType = E_CatalogType.People;

    public Sprite Test;
    public Sprite Test2;
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

        FirstPage = GetUI<UIPageItem>("FirstPage");

        Button_LeftPage = GetUI<Button>("Button_LeftPage");
        Button_RigthPage = GetUI<Button>("Button_RigthPage");

        Toggle_CatalogPeople = GetUI<Toggle>("Toggle_CatalogPeople");
        Toggle_CatalogThing = GetUI<Toggle>("Toggle_CatalogThing");
        Toggle_CatalogRecord = GetUI<Toggle>("Toggle_CatalogRecord");

        Panel_Catalog = GetUI<CanvasGroup>("Panel_Catalog");
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

        AddToggleListen(Toggle_CatalogPeople, (value) => OnToggleChange(E_CatalogType.People, value));
        AddToggleListen(Toggle_CatalogThing, (value) => OnToggleChange(E_CatalogType.Thing, value));
        AddToggleListen(Toggle_CatalogRecord, (value) => OnToggleChange(E_CatalogType.Record, value));
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();

    }

    bool _isClear;
    Dictionary<E_CatalogType, Dictionary<string, Dictionary<int, List<string>>>> _note;

    /// <summary>
    /// 打开界面时调用
    /// </summary>
    /// <param name="objs"></param>
    public override void OnOpen(params object[] objs)
    {
        base.OnOpen(objs);

        _isClear = bool.Parse(objs[0].ToString());

        Panel_Catalog.alpha = 1;
        Panel_Catalog.blocksRaycasts = true;

        if (_isClear)
        {
            _note = RecordData.Instance.Note;
        }
        else
        {
            _note = PlayerManager.Instance.CurrentRecord.Note;
        }

        if (_note.TryGetValue(E_CatalogType.People, out Dictionary<string, Dictionary<int, List<string>>> peopleCatalogDict))
        {
            ShowCatalog(peopleCatalogDict.Keys);
        }

        if (_note.TryGetValue(E_CatalogType.Thing, out Dictionary<string, Dictionary<int, List<string>>> thingCatalogDict))
        {
            ShowCatalog(thingCatalogDict.Keys);
        }
        if (_note.TryGetValue(E_CatalogType.Record, out Dictionary<string, Dictionary<int, List<string>>> recordCatalogDict))
        {
            ShowCatalog(recordCatalogDict.Keys);
        }

        Toggle_CatalogPeople.isOn = true;
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
        if (!_isClear)
        {
            DialogManager.Instance.Talk();
        }
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


    void OnToggleChange(E_CatalogType type, bool value)
    {
        if (value)
        {
            SetCatalogType(type);
        }
    }

    public void SetCatalogType(E_CatalogType type)
    {
        _currentCatalogType = type;
    }

    void ShowCatalog(IEnumerable<string> catalogList)
    {
        foreach (var catalog in catalogList)
        {
            var btn = GetUI<Button>(catalog);
            if (btn == null) Debug.LogError(catalog);
            btn.gameObject.SetActive(true);
        }
    }

    void OnButtonClickCloseNoteBook()
    {
        UIManager.Instance.OpenPanel<UIMaskPanel>(true, () =>
        {
            UIManager.Instance.ClosePanel(this);
            DialogManager.Instance.Talk();
        }, E_MaskType.GameStateChange);
    }

    public void ReadContent(string catalog)
    {
        Panel_Catalog.DOFade(0, 1).OnComplete(() => Panel_Catalog.blocksRaycasts = false);

        var contentDict = PlayerManager.Instance.GetRecordContent(_isClear, _currentCatalogType, catalog);

        int maxPage = Mathf.Max(contentDict.Keys.Max(), 1);

        if ((maxPage % 2) != 2)
        {
            maxPage++;
        }

        FirstPage.SetText(GetContent(contentDict, 1));

        for (int i = 2; i <= maxPage;)
        {
            var frontContent = GetContent(contentDict, i);
            var backContent = GetContent(contentDict, i + 1);
            CreatePage(frontContent, backContent);
            i += 2;
        }
        BookPro.CurrentPaper = 1;
        BookPro.UpdatePages();
    }

    public void ShowPanel(CanvasGroup canvasGroup)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
        {
            canvasGroup.DOFade(1, 0.5f).OnComplete(() => canvasGroup.blocksRaycasts = true);
        });
    }

    public void HidePanel(CanvasGroup canvasGroup)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.5f);
    }

    List<string> GetContent(Dictionary<int, List<string>> content, int page)
    {
        if (content.TryGetValue(page, out List<string> value))
        {
            return value;
        }
        return null;
    }

    void CreatePage(List<string> frontPageContent, List<string> backPageContent)
    {
        var rightPage = PoolManager.Spawn("UIPageItem").GetComponent<UIPageItem>();
        rightPage.transform.SetParent(BookPro.transform);

        rightPage.SetPos(E_PagePos.Right);
        var rightPageTrans = rightPage.GetComponent<RectTransform>();
        rightPageTrans.sizeDelta = BookPro.RightPageTransform.sizeDelta;
        rightPageTrans.pivot = BookPro.RightPageTransform.pivot;
        rightPageTrans.anchoredPosition = BookPro.RightPageTransform.anchoredPosition;
        rightPageTrans.localScale = BookPro.RightPageTransform.localScale;
        rightPage.SetText(frontPageContent);


        var leftPage = PoolManager.Spawn("UIPageItem").GetComponent<UIPageItem>();
        leftPage.transform.SetParent(BookPro.transform);
        leftPage.SetPos(E_PagePos.Left);
        var leftPageTrans = leftPage.GetComponent<RectTransform>();
        leftPageTrans.transform.SetParent(BookPro.transform, true);
        leftPageTrans.sizeDelta = BookPro.LeftPageTransform.sizeDelta;
        leftPageTrans.pivot = BookPro.LeftPageTransform.pivot;
        leftPageTrans.anchoredPosition = BookPro.LeftPageTransform.anchoredPosition;
        leftPageTrans.localScale = BookPro.LeftPageTransform.localScale;
        leftPage.SetText(backPageContent);


        Paper paper = new Paper();
        paper.Back = leftPage.gameObject;
        paper.Front = rightPage.gameObject;
        BookPro.papers.Add(paper);

        BookPro.EndFlippingPaper = BookPro.papers.Count - 1;
    }

    void ClearPages()
    {
        for (int i = BookPro.papers.Count - 1; i > 0; i--)
        {
            PoolManager.DeSpawn(BookPro.papers[i].Front);
            PoolManager.DeSpawn(BookPro.papers[i].Back);
            BookPro.papers.RemoveAt(i);
        }
        BookPro.CurrentPaper = 1;
        BookPro.UpdatePages();
    }

    void BackToCatalog()
    {
        Panel_Catalog.DOFade(1, 1).OnComplete(() =>
        {
            Panel_Catalog.blocksRaycasts = true;
            ClearPages();
        });

        //float beforeTime = AutoFlip.PageFlipTime;

        //float flipTime = beforeTime / (BookPro.CurrentPaper - 1);
        //AutoFlip.PageFlipTime = flipTime;

        //WhileBack(() => { AutoFlip.PageFlipTime = beforeTime; ClearPages(); }); ;
    }

    //void WhileBack(Action callback)
    //{
    //    if (BookPro.CurrentPaper > 1)
    //    {
    //        AutoFlip.FlipLeftPage(() => WhileBack(callback));
    //    }
    //    else
    //    {
    //        callback?.Invoke();
    //    }
    //}

    void FlipToLeftPage()
    {
        if (BookPro.CurrentPaper <= 1)
        {
            Button_LeftPage.interactable = true;
            return;
        }
        AutoFlip.FlipLeftPage();
    }

    void FlipToRightPage()
    {
        if (BookPro.CurrentPaper >= BookPro.papers.Count - 1)
        {
            Button_RigthPage.interactable = true;
            return;
        }
        AutoFlip.FlipRightPage();
    }
    #endregion
}
