using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbUISystem;

public class PlayerManager : Singleton<PlayerManager>
{
    public RecordInfo CurrentRecord { get; private set; }

    public void LoadRecord(int recordIndex)
    {
        CurrentRecord = RecordData.Instance.GetRecordByIndex(recordIndex);
        UIManager.Instance.CloseAllNormalPanel();
        DialogManager.Instance.SetTalkAsset(CurrentRecord.RecordDiaologAsset);
        DialogManager.Instance.Talk();
    }

    public void NewGame()
    {
        CurrentRecord = RecordData.Instance.CreateNewRecord();
        UIManager.Instance.CloseAllNormalPanel();
        DialogManager.Instance.SetTalkAsset(DialogManager.Instance.DefaultDialogAsset);
        DialogManager.Instance.Talk();
    }

    public void DeleteRecord(int recordIndex)
    {
        RecordData.Instance.DeleteRecord(recordIndex);
    }

    public bool IsReaded(DialogAsset asset)
    {
        return GameData.Instance.IsReaded(asset);
    }
}
