using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbFramework;

public class HintEvent : TalkEvent
{
    DelayEvent _data;
    public HintEvent(DelayEvent eventData)
    {
        _data = eventData;
    }
    public override void Play()
    {
        var panel= UIManager.Instance.GetPanel<UIDialogPanel>();
        if (panel.IsShowHint())
        {
            panel.HideHint();
        }
        panel.ShowHint(_data.StringValue);

        if (_data.HintTime>0)
        {
            TimerManager.Instance.AddListener(_data.HintTime, panel.HideHint);
        }
    }

    public override void Stop()
    {

    }
}
