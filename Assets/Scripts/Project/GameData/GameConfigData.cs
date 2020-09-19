using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfigData : Data<GameConfigData>
{
    public int WordSizeLevel; //字体大小
    public int TyperSpeedLevel;//打字速度
    public int DialogAlphaLevel;//对话框透明度等级

    public bool IsFullScreen; //是否全屏显示
    public bool IsSkipUnRead;//是否可以快进跳过未读部分
    public bool IsShowShortcutKey;//是否显示快捷键

    public int BgmVolumeLevel;
    public int DubVoumeLevel;
    public int AudioVolumeLevel;
    public int MasterVolumeLevel;
}
