using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoSingleton<GameConfig>
{
    public DialogAsset Asset;

    [Header("字体大小")]
    public int WordSizeLevel1 = 10;
    public int WordSizeLevel2 = 15;
    public int WordSizeLevel3 = 20;
    [Header("文字速度倍率")]
    public float TyperSpeedLevel1 = 0.8f;
    public float TyperSpeedLevel2 = 1f;
    public float TyperSpeedLevel3 = 1.2f;
    [Header("对话框透明度")]
    public float DialogAlphaLevel1 = 0.2f;
    public float DialogAlphaLevel2 = 0.5f;
    public float DialogAlphaLevel3 = 0.8f;
    [Header("切屏过渡时间")]
    public float BlackTransitionTime=0.1f;
    public float WhiteTransitionTime=0.1f;
    public float GameStateChangeTransitionTime=0.5f;
}
