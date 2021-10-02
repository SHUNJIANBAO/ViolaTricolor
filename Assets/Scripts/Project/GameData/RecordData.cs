using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class RecordInfo
{
    public int RecordIndex;
    public DialogAsset RecordDiaologAsset;
    public int TalkIndex;
    /// <summary>
    /// 人事记类型、标题名、页数、多语言图片key列表
    /// </summary>
    public Dictionary<E_CatalogType, Dictionary<string, Dictionary<int, List<string>>>> Note = new Dictionary<E_CatalogType, Dictionary<string, Dictionary<int, List<string>>>>();
}

/// <summary>
/// 存档数据
/// </summary>
public class RecordData : Data<RecordData>
{
    Dictionary<int, RecordInfo> _recordInfoDict = new Dictionary<int, RecordInfo>();
    public bool IsClearGame;
    public Dictionary<E_CatalogType, Dictionary<string, Dictionary<int, List<string>>>> Note = new Dictionary<E_CatalogType, Dictionary<string, Dictionary<int, List<string>>>>();
    protected override void OnLoad()
    {
        base.OnLoad();
    }

    public RecordInfo CreateNewRecord()
    {
        RecordInfo info = new RecordInfo();
        info.RecordIndex = 0;
        if (_recordInfoDict.Count > 0)
        {
            var maxIndex = _recordInfoDict.Values.Max(record => record.RecordIndex);
            for (int i = 0; i < Mathf.Max(maxIndex, 1); i++)
            {
                if (!_recordInfoDict.ContainsKey(i))
                {
                    info.RecordIndex = i;
                    break;
                }
            }
            if (info.RecordIndex == 0) info.RecordIndex = maxIndex + 1;
        }

        info.RecordDiaologAsset = DialogManager.Instance.DefaultDialogAsset;
        return info;
    }

    public void SaveRecord(RecordInfo info)
    {
        if (!_recordInfoDict.ContainsKey(info.RecordIndex))
        {
            _recordInfoDict.Add(info.RecordIndex, info);
        }
        else
        {
            _recordInfoDict[info.RecordIndex] = info;
        }
        Save();
    }

    public void SaveRecordByIndex(ref RecordInfo info, int recordIndex)
    {
        info.RecordIndex = recordIndex;

        if (!_recordInfoDict.TryGetValue(recordIndex, out RecordInfo record))
        {
            _recordInfoDict.Add(recordIndex, info);
        }
        else
        {
            _recordInfoDict[recordIndex] = info;
        }

        Save();
    }

    public void DeleteRecord(int recordIndex)
    {
        if (!_recordInfoDict.TryGetValue(recordIndex, out RecordInfo record))
        {
            Debug.LogError("该槽位的存档为空：" + recordIndex);
        }
        _recordInfoDict.Remove(recordIndex);
    }

    public RecordInfo GetRecordByIndex(int recordIndex)
    {
        if (!_recordInfoDict.TryGetValue(recordIndex, out RecordInfo record))
        {

        }
        return record;
    }
}
