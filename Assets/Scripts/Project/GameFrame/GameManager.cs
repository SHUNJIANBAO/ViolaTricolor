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
        SetGameConfig();
        yield return ResourcesManager.LoadAssetsForLabelAsync<GameObject>("UI", UIManager.Instance.Init);
        yield return DialogManager.Instance.Init();
        //yield return AudioManager.Instance.Init();
        Debug.Log("资源加载完成!");
        UIManager.Instance.OpenPanel<UINoteBookPanel>();
        //UIManager.Instance.OpenPanel<UIMainMenuPanel>();
    }

    public void LoadConfig()
    {
        Debug.Log("读取配置完成");
    }

    void LoadData()
    {
        GameData.Load();
        GameConfigData.Load();
        RecordData.Load();
        Debug.Log("加载数据完成");

    }


    void SetGameConfig()
    {
        if (GameConfigData.Instance.IsFullScreen)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            //Screen.fullScreen = true;
        }
        else
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            //Screen.fullScreen = false;
        }
        //Screen.fullScreen = GameConfigData.Instance.IsFullScreen;
        Debug.Log("配置设置完成");
    }

}
