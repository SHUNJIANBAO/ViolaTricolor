using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PbAudioSystem;
using PbFramework;

public class PlayAudioEvent : TalkEvent
{
    E_AudioType _audioType;
    AudioClip _audioClip;
    public PlayAudioEvent(DelayEvent eventData)
    {
        _audioType = eventData.AuidoType;
        _audioClip = eventData.Audio;

    }
    public override void Play()
    {
        AudioManager.Instance.Play(_audioClip, _audioType);
    }

    public override void Stop()
    {

    }
}
