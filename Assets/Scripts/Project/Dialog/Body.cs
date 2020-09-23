using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Body : MonoBehaviour
{
    public Image BodyImage;
    public Image FaceImage;
    public Animation FaceAnimation;

    public void SetFace(Sprite sprite)
    {
        FaceImage.sprite = sprite;
        FaceImage.SetNativeSize();
    }

    public void SetFaceAnimation(AnimationClip clip)
    {
        FaceAnimation.clip = clip;
        FaceAnimation.Play();
    }
}
