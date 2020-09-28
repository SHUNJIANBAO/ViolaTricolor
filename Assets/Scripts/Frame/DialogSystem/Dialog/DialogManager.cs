﻿using UnityEngine.ResourceManagement.AsyncOperations;
using PbUISystem;

public class DialogManager : Singleton<DialogManager>
{
    bool _isOpen;
    bool _isClosing;
    public static bool IsTalking { get; private set; }
    DialogAsset _curDialogAsset;
    int _index;

    UIDialogPanel _dialogPanel;

    private const string _firstDialogLabel = "DefaultDialog";

    public DialogAsset DefaultDialogAsset;

    public AsyncOperationHandle<DialogAsset> Init()
    {
        var handle = ResourcesManager.LoadAssetAsync<DialogAsset>(_firstDialogLabel, (asset) =>
        {
            DefaultDialogAsset = asset;
        });
        return handle;
    }


    public void SetTalkAsset(DialogAsset asset)
    {
        _index = 0;
        _curDialogAsset = asset;
    }

    public void Talk(int talkId = -1)
    {
        if (!_curDialogAsset) return;
        if (_isClosing) return;

        if (talkId != -1)
        {
            for (int i = 0; i < _curDialogAsset.TalkAssetsList.Count; i++)
            {
                if (_curDialogAsset.TalkAssetsList[i].TalkId == talkId)
                {
                    _index = i;
                    break;
                }
            }
        }
        else
        {
            if (!IsTalking)
                _index = 0;
        }

        if (!IsTalking)
        {
            IsTalking = true;
            //ActionManager.Instance.Invoke(ActionType.TalkStart);
            _dialogPanel = UIManager.Instance.OpenPanel<UIDialogPanel>(true, () =>
             {
                 _isOpen = true;
                 Talk();
             }, _curDialogAsset.TalkAssetsList[_index]);
        }

        if (!_isOpen || _dialogPanel == null) return;

        if (!_isClosing && _index >= _curDialogAsset.TalkAssetsList.Count)
        {
            if (_dialogPanel.IsTalking)
                _dialogPanel.StopTalk();
            else
                TalkEnd();
        }
        else
        {
            if (!_dialogPanel.IsCanTalk()) return;
            //对话框类型相同直接谈话，不同则切换模式后谈话
            if (_dialogPanel.IsSameDialogType(_curDialogAsset.TalkAssetsList[_index].DialogType))
            {
                _dialogPanel.Talk(_curDialogAsset.TalkAssetsList[_index]);
                _index++;
            }
            else
            {
                switch (_curDialogAsset.TalkAssetsList[_index].DialogType)
                {
                    case E_DialogType.Normal:
                        break;
                    case E_DialogType.FullScreen:
                        _dialogPanel.HideBodys();
                        _dialogPanel.HideDialog(true);
                        break;
                }
                UIManager.Instance.OpenPanel<UIMaskPanel>(true, () =>
                {
                    _dialogPanel.InitByAsset(_curDialogAsset.TalkAssetsList[_index]);
                    Talk(_curDialogAsset.TalkAssetsList[_index].TalkId);
                }, E_MaskType.GameStateChange);
            }
        }
    }

    /// <summary>
    /// 段落结束
    /// </summary>
    public void TalkEnd()
    {
        GameData.Instance.ReadAsset(_curDialogAsset);
        GameData.Save();
        switch (_curDialogAsset.TalkEndEventType)
        {
            case E_TalkEndEventType.Night:
                UIManager.Instance.OpenPanel<UIMaskPanel>(true, () =>
                {
                    _index = 0;
                    _isOpen = false;
                    IsTalking = false;
                    _isClosing = false;
                    _dialogPanel = null;
                    UIManager.Instance.CloseAllNormalPanel(false);
                    UIManager.Instance.OpenPanel<UINoteBookPanel>();
                    SetTalkAsset(_curDialogAsset.LinkedDialogAsset);
                }, E_MaskType.GameStateChange);
                break;
            case E_TalkEndEventType.Select:
                UIManager.Instance.OpenPanel<UISelectPanel>(true, null, _curDialogAsset.SelectDialogAssetList);
                break;
            case E_TalkEndEventType.GameOver:
                _isClosing = true;
                UIManager.Instance.OpenPanel<UIMaskPanel>(true, () =>
                {
                    _index = 0;
                    _isOpen = false;
                    IsTalking = false;
                    _isClosing = false;
                    _curDialogAsset = null;
                    _dialogPanel = null;
                    UIManager.Instance.CloseAllNormalPanel(false);
                    UIManager.Instance.OpenPanel<UIMainMenuPanel>();
                }, E_MaskType.GameStateChange);
                break;
        }
    }

    public void HideDialog()
    {
        if (!_isOpen) return;
        if (_dialogPanel.IsShowDialog)
            _dialogPanel.HideDialog(false);
    }
}
