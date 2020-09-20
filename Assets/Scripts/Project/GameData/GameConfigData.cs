using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigData : Data<GameConfigData>
{
    [Range(0, 2)]
    public int WordSizeLevel = 1; //字体大小
    [Range(0, 2)]
    public int TyperSpeedLevel = 1;//打字速度
    [Range(0, 2)]
    public int DialogAlphaLevel = 1;//对话框透明度等级

    public bool IsFullScreen = true; //是否全屏显示
    public bool IsSkipUnRead;//是否可以快进跳过未读部分
    public bool IsShowShortcutKey;//是否显示快捷键

    [Range(0, 10)]
    public int BgmVolumeLevel;
    [Range(0, 10)]
    public int DubVoumeLevel;
    [Range(0, 10)]
    public int AudioVolumeLevel;
    [Range(0, 10)]
    public int MasterVolumeLevel;
}
