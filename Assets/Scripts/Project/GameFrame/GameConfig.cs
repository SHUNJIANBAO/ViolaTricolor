using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoSingleton<GameConfig>
{
    [Header("字体大小")]
    public int WordSizeLevel1 = 10;
    public int WordSizeLevel2 = 15;
    public int WordSizeLevel3 = 20;
    [Header("文字速度(每个文字等待时间)")]
    public float TyperSpeedLevel1 = 0.1f;
    public float TyperSpeedLevel2 = 0.2f;
    public float TyperSpeedLevel3 = 0.3f;
    [Header("对话框透明度")]
    public float DialogAlphaLevel1 = 0.2f;
    public float DialogAlphaLevel2 = 0.5f;
    public float DialogAlphaLevel3 = 0.8f;
    [Header("切屏过渡时间")]
    public float BlackTransitionTime=0.1f;
    public float WhiteTransitionTime=0.1f;
    public float GameStateChangeTransitionTime=0.5f;
}
