using UnityEngine.ResourceManagement.AsyncOperations;
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

        if (talkId!=-1)
        {
            for (int i = 0; i < _curDialogAsset.TalkAssetsList.Count; i++)
            {
                if (_curDialogAsset.TalkAssetsList[i].TalkId==talkId)
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

        if (!_isOpen) return;

        if (!_isClosing && _index >= _curDialogAsset.TalkAssetsList.Count)
        {
            if (_dialogPanel.IsTalking)
                _dialogPanel.StopTalk();
            else
                TalkEnd();
        }
        else
        {
            bool result = _dialogPanel.Talk(_curDialogAsset.TalkAssetsList[_index]);
            if (result)
                _index++;
        }
    }

    //todo
    public void TalkEnd()
    {
        switch (_curDialogAsset.TalkEndEventType)
        {
            case E_TalkEndEventType.Night:
                break;
            case E_TalkEndEventType.Select:
                UIManager.Instance.OpenPanel<UISelectPanel>(true, null, _curDialogAsset.SelectDialogAssetList);
                break;
            case E_TalkEndEventType.GameOver:
                _isClosing = true;
                UIManager.Instance.ClosePanel<UIDialogPanel>(true, () =>
                {
                    _index = 0;
                    _isOpen = false;
                    IsTalking = false;
                    _isClosing = false;
                    _curDialogAsset = null;
                    //ActionManager.Instance.Invoke(ActionType.TalkEnd);
                });
                break;
        }
    }
}
