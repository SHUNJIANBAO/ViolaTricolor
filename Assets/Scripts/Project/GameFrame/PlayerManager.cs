using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbFramework;//using PbUISystem;

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
             DialogManager.Instance.SetTalkAsset(CurrentRecord.RecordDiaologAsset, CurrentRecord.TalkIndex);
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
        DialogManager.Instance.SetTalkAsset(GameConfig.Instance.Asset ?? DialogManager.Instance.DefaultDialogAsset);
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

    public void NoteRecord(List<NoteRecordData> dataList)
    {
        foreach (var data in dataList)
        {
            NoteRecord(data);
        }
    }
    public void NoteRecord(NoteRecordData data)
    {
        if (!CurrentRecord.Note.TryGetValue(data.CatalogType, out Dictionary<string, Dictionary<int, List<string>>> dict))
        {
            dict = new Dictionary<string, Dictionary<int, List<string>>>();
            CurrentRecord.Note.Add(data.CatalogType, dict);
        }
        if (!dict.TryGetValue(data.Title, out var pageDict))
        {
            pageDict = new Dictionary<int, List<string>>();
            dict.Add(data.Title, pageDict);
        }

        //string content = data.Text + "\n";
        if (!pageDict.TryGetValue(data.Page, out List<string> contentList))
        {
            contentList = new List<string>();
            pageDict.Add(data.Page, contentList);
        }
        contentList.Add(data.Text.name);
    }

    public Dictionary<int, List<string>> GetRecordContent(bool isClear, E_CatalogType catalogType, string catalog)
    {
        var note = isClear ? RecordData.Instance.Note : CurrentRecord.Note;

        if (!note.TryGetValue(catalogType, out Dictionary<string, Dictionary<int, List<string>>> dict))
        {
            return null;
        }
        dict.TryGetValue(catalog, out var pageDict);
        return pageDict;
    }
}
