using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Data<GameData>
{
    public List<DialogAsset> ReadAssetList;

    protected override void OnLoad()
    {
        base.OnLoad();
        if (ReadAssetList == null) ReadAssetList = new List<DialogAsset>();
    }

    public void ReadAsset(DialogAsset asset)
    {
        ReadAssetList.Add(asset);
    }

    public bool IsReaded(DialogAsset asset)
    {
        return ReadAssetList.Contains(asset);
    }
}
