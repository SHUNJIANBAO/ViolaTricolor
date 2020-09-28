using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbUISystem;

public enum E_MaskType
{
    Black,
    White,
    GameStateChange,
}

[RequireComponent(typeof(CanvasGroup))]
public class UIMaskPanel : UIPanelBase
{
    #region 参数
    Image Image_White;
    Image Image_Black;

    float _duration;
    #endregion

    #region 继承方法

    /// <summary>
    /// 得到UI组件
    /// </summary>
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Image_White = GetUI<Image>("Image_White");
        Image_Black = GetUI<Image>("Image_Black");
    }

    /// <summary>
    /// 给UI添加方法
    /// </summary>
    protected override void AddUIListener()
    {
        base.AddUIListener();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
        Image_Black.gameObject.SetActive(false);
        Image_White.gameObject.SetActive(false);
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
        E_MaskType maskType = (E_MaskType)args[0];

        switch (maskType)
        {
            case E_MaskType.Black:
                _duration = GameConfig.Instance.BlackTransitionTime;
                Image_Black.gameObject.SetActive(true);
                Image_White.gameObject.SetActive(false);
                break;
            case E_MaskType.White:
                _duration = GameConfig.Instance.WhiteTransitionTime;
                Image_Black.gameObject.SetActive(false);
                Image_White.gameObject.SetActive(true);
                break;
            case E_MaskType.GameStateChange:
                _duration = GameConfig.Instance.GameStateChangeTransitionTime;
                Image_Black.gameObject.SetActive(true);
                Image_White.gameObject.SetActive(false);
                break;
        }

        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.DOKill();
        m_CanvasGroup.DOFade(1, _duration).OnComplete(() =>
        {
            StartCoroutine(base.OpenAnim(callback));
            UIManager.Instance.ClosePanel(this);
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
        m_CanvasGroup.DOFade(0, _duration).OnComplete(() =>
        {
            StartCoroutine(base.CloseAnim(callback));
        });
        yield return new WaitForEndOfFrame();
    }
    #endregion

    #region 成员方法



    #endregion
}
