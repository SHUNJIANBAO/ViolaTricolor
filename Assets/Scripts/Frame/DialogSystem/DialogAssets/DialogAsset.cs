using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogAsset : ScriptableObject
{
    public string OptionName;  //标题名
    public E_UnLockType UnLockType;  //锁定类型
    public DialogAsset NeedDialogAsset;

    public E_TalkEndEventType TalkEndEventType;
    public E_MaskType MaskType;
    public DialogAsset LinkedDialogAsset; //连接的段落
    public List<DialogAsset> SelectDialogAssetList = new List<DialogAsset>();

    public List<DialogueAsset> DialogueAssetList = new List<DialogueAsset>();

    public DialogAsset Copy()
    {
        DialogAsset asset = ScriptableObject.CreateInstance<DialogAsset>();
        asset.name = this.name;


        asset.OptionName = OptionName;
        asset.UnLockType = UnLockType;
        asset.MaskType = MaskType;
        asset.NeedDialogAsset = NeedDialogAsset;
        asset.LinkedDialogAsset = LinkedDialogAsset;

        asset.TalkEndEventType = TalkEndEventType;

        asset.SelectDialogAssetList = new List<DialogAsset>(SelectDialogAssetList);

        asset.DialogueAssetList = new List<DialogueAsset>();
        foreach (var dialogueAsset in DialogueAssetList)
        {
            asset.DialogueAssetList.Add(dialogueAsset.Copy());
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
    Transition,
    Night,
    Select,
    GameOver,
}