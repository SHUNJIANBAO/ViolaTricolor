using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageImage : MonoBehaviour
{
    public Sprite CN;
    public Sprite JP;
    public Sprite EN;

    Image _image;

    private void OnEnable()
    {
        if (_image == null)
            _image = GetComponent<Image>();
        if (_image == null) return;
        Refresh();
        ActionManager.Instance.AddListener(ActionType.ChangeLanguage, Refresh);
    }

    void Refresh(params object[] args)
    {
        switch (GameConfigData.Instance.Language)
        {
            case E_LanguageType.CN:
                _image.sprite = CN;
                break;
            case E_LanguageType.JP:
                _image.sprite = JP;
                break;
            case E_LanguageType.EN:
                _image.sprite = EN;
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        if (_image == null) return;

        ActionManager.Instance.RemoveListener(ActionType.ChangeLanguage, Refresh);
    }
}
