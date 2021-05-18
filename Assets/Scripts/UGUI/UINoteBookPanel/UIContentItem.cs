using PbUISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContentItem : UIItemBase
{
    Image _image;
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        _image = GetComponent<Image>();
    }
    public void Init(string spriteName)
    {
        var sprite = Resources.Load<Sprite>("NoteContents/" + spriteName);
        _image.sprite = sprite;
        _image.SetNativeSize();
    }
}
