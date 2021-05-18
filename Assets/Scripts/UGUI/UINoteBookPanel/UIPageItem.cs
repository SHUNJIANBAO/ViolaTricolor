using PbUISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPageItem : UIItemBase, PbFramework.IPoolItem
{
    //Image Image_Left;
    //Image Image_Right;
    Image _image;
    Text Text_Content;
    List<UIContentItem> _contentItemList = new List<UIContentItem>();

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

    public void SetText(List<string> contentList)
    {
        if (contentList == null)
        {
            return;
        }


        for (int i = 0; i < Mathf.Max(contentList.Count, _contentItemList.Count); i++)
        {
            if (_contentItemList.Count <= i)
            {
                var item = UIManager.Instance.CreateItem<UIContentItem>(transform);
                _contentItemList.Add(item);
                item.Init(contentList[i]);
            }
            else
            {
                if (contentList.Count > i)
                {
                    _contentItemList[i].gameObject.SetActive(true);
                    _contentItemList[i].Init(contentList[i]);
                }
                else
                {
                    _contentItemList[i].gameObject.SetActive(false);
                }
            }
        }

        //if(Text_Content==null) Text_Content = GetUI<Text>("Text_Content");

        //Text_Content.text = text;
    }

    public void OnSpawn()
    {

    }

    public void OnDeSpawn()
    {

    }
}

public enum E_PagePos
{
    Left,
    Right
}
