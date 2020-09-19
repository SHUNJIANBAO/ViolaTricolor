using System;

public class Timer
{
    //总时长
    private float totalTime;
    //间隔时长
    private float intervalTime;
    //间隔调用的方法
    public Action intervalAction;
    //完成时调用的方法
    public Action completeAction;
    //是否循环
    private bool loop;
    //是否结束
    public bool isComplete;
    //记录时间
    private float timeCount;
    public Timer(float totalTime,float intervalTime=0,bool loop=false)
    {
        this.totalTime = totalTime;
        this.intervalTime = intervalTime;
        this.loop = loop;
    }

    public void Update(float deltaTime)
    {
        if (isComplete) return;
        timeCount += deltaTime;
        if (intervalTime!=0)
        {
            if (timeCount >= intervalTime)
            {
                totalTime -= timeCount;
                timeCount -= intervalTime;
                if (intervalAction != null) intervalAction();
            }
        }
        else
        {
            if (timeCount>=totalTime)
            {
                totalTime -= timeCount;
            }
        }
        if (totalTime<=0&&!loop)
        {
            if (completeAction != null) completeAction();
            isComplete = true;
        }
    }
}
