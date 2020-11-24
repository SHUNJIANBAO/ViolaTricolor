using PbUISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBodyEvent : TalkEvent
{
    E_BodyPos _bodyPos;
    E_BodyShowType _bodyShowPos;
    GameObject _body;
    public ChangeBodyEvent(DelayEvent eventData)
    {
        _bodyPos = eventData.BodyPos;
        _bodyShowPos = eventData.BodyShowType;
        _body = eventData.Body;
    }
    public override void Play()
    {
        var dialogPanel= UIManager.Instance.GetPanel<UIDialogPanel>();
        dialogPanel.ChangeBody(_bodyPos, _body, _bodyShowPos);
    }

    public override void Stop()
    {

    }
}
