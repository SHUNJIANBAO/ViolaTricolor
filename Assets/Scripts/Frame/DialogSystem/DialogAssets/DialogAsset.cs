using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogAsset : ScriptableObject
{
    public string OptionName;  //标题名
    public E_UnLockType UnLockType;  //锁定类型
    public DialogAsset NeedDialogAsset;

    public E_TalkEndEventType TalkEndEventType;
    public DialogAsset LinkedDialogAsset; //连接的段落
    public List<DialogAsset> SelectDialogAssetList = new List<DialogAsset>();

    public List<TalkAsset> TalkAssetsList = new List<TalkAsset>();

    public DialogAsset Copy()
    {
        DialogAsset asset = ScriptableObject.CreateInstance<DialogAsset>();
        asset.name = this.name;


        asset.OptionName = OptionName;
        asset.UnLockType = UnLockType;
        asset.NeedDialogAsset = NeedDialogAsset;
        asset.LinkedDialogAsset = LinkedDialogAsset;

        asset.TalkEndEventType = TalkEndEventType;

        asset.SelectDialogAssetList = new List<DialogAsset>(SelectDialogAssetList);

        asset.TalkAssetsList = new List<TalkAsset>();
        foreach (var talkAsset in TalkAssetsList)
        {
            asset.TalkAssetsList.Add(talkAsset.Copy());
        }
        return asset;
    }
}


public enum E_UnLockType
{
    None,
    Talked,
}

public enum E_TalkEndEventType
{
    Night,
    Select,
    GameOver,
}