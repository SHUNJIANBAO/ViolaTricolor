using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct RecordInfo
{
    public int RecordIndex;
    public DialogAsset RecordDiaologAsset;
    public int TalkIndex;
    public List<DialogAsset> ReadedAssetList;
}

/// <summary>
/// 存档数据
/// </summary>
public class RecordData : Data<RecordData>
{
    List<RecordInfo> _recordInfoList;
    Dictionary<int, RecordInfo> _recordInfoDict = new Dictionary<int, RecordInfo>();
    protected override void OnLoad()
    {
        base.OnLoad();
        if (_recordInfoList == null)
        {
            _recordInfoList = new List<RecordInfo>();
        }
        foreach (var record in _recordInfoList)
        {
            _recordInfoDict.Add(record.RecordIndex, record);
        }
    }

    public RecordInfo CreateNewRecord()
    {
        RecordInfo info = new RecordInfo();
        info.ReadedAssetList = new List<DialogAsset>();
        return info;
    }

    public void SaveRecordByIndex(RecordInfo info, int recordIndex)
    {
        info.RecordIndex = recordIndex;
        if (!_recordInfoDict.TryGetValue(recordIndex, out RecordInfo record))
        {
            _recordInfoDict.Add(recordIndex, info);
            _recordInfoList.Add(info);
        }
        else
        {
            _recordInfoDict[recordIndex] = info;
            for (int i = 0; i < _recordInfoList.Count; i++)
            {
                if (_recordInfoList[i].RecordIndex == recordIndex)
                {
                    _recordInfoList[i] = info;
                }
            }
        }
    }

    public void DeleteRecord(int recordIndex)
    {
        if (!_recordInfoDict.TryGetValue(recordIndex, out RecordInfo record))
        {
            Debug.LogError("该槽位的存档为空：" + recordIndex);
        }
        _recordInfoList.Remove(record);
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
