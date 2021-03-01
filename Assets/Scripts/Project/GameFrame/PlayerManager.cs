using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbUISystem;

public class PlayerManager : Singleton<PlayerManager>
{
    public RecordInfo CurrentRecord { get; private set; }

    public void QuickSave()
    {
        CurrentRecord.RecordDiaologAsset = DialogManager.Instance.CurDialogAsset;
        CurrentRecord.TalkIndex = DialogManager.Instance.CurTalkIndex;
        RecordData.Instance.SaveRecord(CurrentRecord);
    }
    public void QuickLoad()
    {
        UIManager.Instance.OpenPanel<UIMaskPanel>(true, () =>
         {
             DialogManager.Instance.SetTalkAsset(CurrentRecord.RecordDiaologAsset);
             DialogManager.Instance.Talk(CurrentRecord.TalkIndex);
         }, E_MaskType.GameStateChange);
    }

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
