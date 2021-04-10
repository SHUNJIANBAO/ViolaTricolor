using PbUISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPageItem : UIItemBase
{
    //Image Image_Left;
    //Image Image_Right;
    Image _image;
    Text Text_Content;
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        _image = GetComponent<Image>();
    }
    protected override void AddUIListener()
    {
        base.AddUIListener();

    }

    public void SetPos(E_PagePos posType)
    {
        switch (posType)
        {
            case E_PagePos.Left:
                _image.sprite = GetUI<Image>("Image_Left").sprite;
                break;
            case E_PagePos.Right:
                _image.sprite = GetUI<Image>("Image_Right").sprite;
                break;
        }
    }

    public void SetText(string text)
    {
        if(Text_Content==null) Text_Content = GetUI<Text>("Text_Content");

        Text_Content.text = text;
    }
}

public enum E_PagePos
{
    Left,
    Right
}
