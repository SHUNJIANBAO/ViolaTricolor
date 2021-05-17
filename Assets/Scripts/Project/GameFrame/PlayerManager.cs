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
        if (!CurrentRecord.Note.TryGetValue(data.CatalogType, out var dict))
        {
            dict = new Dictionary<string, Dictionary<int, List<Sprite>>>();
            dict.Add(data.Title, new Dictionary<int, List<Sprite>>());
        }
        dict.TryGetValue(data.Title, out var pageDict);

        //string content = data.Text + "\n";
        if (!pageDict.TryGetValue(data.Page,out List<Sprite> contentList))
        {
            contentList = new List<Sprite>();
            pageDict.Add(data.Page, contentList);
        }
        contentList.Add(data.Text);
    }

    public Dictionary<int, List<Sprite>> GetRecordContent(E_CatalogType catalogType, string catalog)
    {
        if (!CurrentRecord.Note.TryGetValue(catalogType, out var dict))
        {
            return null;
        }
        dict.TryGetValue(catalog, out var pageDict);
        return pageDict;
    }
}
