using PbFramework;//using PbUISystem;
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
    public void Init(string languageImageKey)
    {
        var cfg= LanguageImageConfig.GetData(languageImageKey);
        string spriteName = "";
        switch (GameConfigData.Instance.Language)
        {
            case E_LanguageType.CN:
                spriteName = cfg.CN;
                break;
            case E_LanguageType.JP:
                spriteName = cfg.JP;
                break;
            case E_LanguageType.EN:
                spriteName = cfg.EN;
                break;
            default:
                break;
        }
        var sprite = Resources.Load<Sprite>("NoteContents/" + spriteName);
        _image.sprite = sprite;
        _image.SetNativeSize();
    }
}
