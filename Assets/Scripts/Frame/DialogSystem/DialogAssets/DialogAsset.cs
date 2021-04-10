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
    public List<NoteRecordData> NodeRecordDataList = new List<NoteRecordData>();

    public DialogAsset LockConditionAsset;
    public DialogAsset LockLinkedAsset;

    public List<DialogAsset> SelectDialogAssetList = new List<DialogAsset>();

    public List<DialogueAsset> DialogueAssetList = new List<DialogueAsset>();

}

[System.Serializable]
public class NoteRecordData
{
    public E_CatalogType CatalogType;
    public string Title;
    public int Page;
    public string Text;
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

public enum E_CatalogType
{
    People,
    Thing,
    Record,
}