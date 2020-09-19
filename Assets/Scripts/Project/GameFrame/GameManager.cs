using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbUISystem;

public class GameManager : MonoSingleton<GameManager>
{
    IEnumerator Start()
    {
        LoadConfig();
        LoadData();
        yield return ResourcesManager.LoadAssetsForLabelAsync<GameObject>("UI", UIManager.Instance.Init);
        yield return DialogManager.Instance.Init();
        //yield return AudioManager.Instance.Init();
        Debug.Log("资源加载完成!");
        UIManager.Instance.OpenPanel<UIMainMenuPanel>();
    }

    public void LoadConfig()
    {
        Debug.Log("读取配置完成");
    }

    void LoadData()
    {
        GameConfigData.Load();
        RecordData.Load();
        Debug.Log("加载数据完成");
    }




}
