using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PbUISystem;

public class UISelectItem : UIItemBase
{
    DialogAsset _branchAsset;

    Text Text_Title;
    Button Button_Branch;
    GameObject Image_Lock;
    protected override void GetUIComponent()
    {
        base.GetUIComponent();
        Button_Branch = GetUI<Button>("Button_Branch");
        Text_Title = GetUI<Text>("Text_Title");
        Image_Lock = GetUI<GameObject>("Image_Lock");
    }
    protected override void AddUIListener()
    {
        base.AddUIListener();
        AddButtonListen(Button_Branch, OnClickButtonSelectBranch);
    }

    public void Init(DialogAsset asset)
    {
        _branchAsset = asset;
        Text_Title.text = _branchAsset.OptionName;
        switch (asset.UnLockType)
        {
            case E_UnLockType.None:
                Button_Branch.enabled = true;
                Image_Lock.SetActive(false);
                break;
            case E_UnLockType.Talked:
                bool isUnLock= PlayerManager.Instance.IsReaded(asset.NeedDialogAsset);
                Button_Branch.enabled = isUnLock;
                Image_Lock.SetActive(!isUnLock);
                break;
        }
    }

    void OnClickButtonSelectBranch()
    {
        DialogManager.Instance.SetTalkAsset(_branchAsset);
        UIManager.Instance.ClosePanel<UISelectPanel>(true, () => DialogManager.Instance.Talk());
    }
}
