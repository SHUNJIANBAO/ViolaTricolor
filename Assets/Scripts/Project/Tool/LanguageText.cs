using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageText : MonoBehaviour
{
    Text _text;

    private void OnEnable()
    {
        if (_text == null)
            _text = GetComponent<Text>();
        if (_text == null) return;
        Refresh();
        ActionManager.Instance.AddListener(ActionType.ChangeLanguage, Refresh);
    }

    void Refresh(params object[] args)
    {
        var cfg = LanguageConfig.GetData(_text.text);
        switch (GameConfigData.Instance.Language)
        {
            case E_LanguageType.CN:
                _text.text = cfg.CN;
                break;
            case E_LanguageType.JP:
                _text.text = cfg.JP;
                break;
            case E_LanguageType.EN:
                _text.text = cfg.EN;
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        if (_text == null) return;

        ActionManager.Instance.RemoveListener(ActionType.ChangeLanguage, Refresh);
    }
}
