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
}

/// <summary>
/// 存档数据
/// </summary>
public class RecordData : Data<RecordData>
{
    [SerializeField] List<RecordInfo> _recordInfoList;
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
        if (_recordInfoList.Count > 0)
        {
            var maxIndex = _recordInfoList.Max(record => record.RecordIndex);
            for (int i = 0; i < Mathf.Max(maxIndex, 1); i++)
            {
                if (!_recordInfoDict.ContainsKey(i))
                {
                    info.RecordIndex = i;
                    break;
                }
            }
        }
        else
        {
            info.RecordIndex = 0;
        }
        info.RecordDiaologAsset = DialogManager.Instance.DefaultDialogAsset;
        return info;
    }

    public void SaveRecord(RecordInfo info)
    {
        for (int i = 0; i < _recordInfoList.Count; i++)
        {
            if (_recordInfoList[i].RecordIndex == info.RecordIndex)
            {
                _recordInfoList[i] = info;
            }
        }
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
        for (int i = 0; i < _recordInfoList.Count; i++)
        {
            if (_recordInfoList[i].RecordIndex == recordIndex)
            {
                _recordInfoList[i] = info;
            }
        }

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

        Save();
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
