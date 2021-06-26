using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using PbFramework;

public class HintHandle : MonoBehaviour
{
    CanvasGroup _group;
    Text _text;
    Canvas _canvas;
    void Awake()
    {
        _group = GetComponent<CanvasGroup>();
        _text = GetComponentInChildren<Text>();
        _canvas=GetComponent<Canvas>();
    }
    private void OnEnable()
    {

        _group.alpha = 0;

    }

    public void Show(string content)
    {
        var tmpCanvas= UIManager.Instance.PanelRoot.GetComponentInParent<Canvas>();
        _canvas.GetComponent<RectTransform>().sizeDelta = tmpCanvas.GetComponent<RectTransform>().sizeDelta;
        _canvas.transform.localScale = tmpCanvas.transform.localScale;
        Show();
        _text.text = content;
    }

    public void Show()
    {
        _group.DOFade(1, 0.1f);
    }

    public void Hide()
    {
        _group.DOFade(0, 0.05f);

    }
}
