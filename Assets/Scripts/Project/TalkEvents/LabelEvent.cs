using PbFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelEvent : TalkEvent
{
    DelayEvent _data;
    public LabelEvent(DelayEvent eventData)
    {
        _data = eventData;
    }

    public override void Play()
    {
        var panel = UIManager.Instance.GetPanel<UIDialogPanel>();
        panel.ShowLabel(_data.StringValue);
    }

    public override void Stop()
    {

    }
}
