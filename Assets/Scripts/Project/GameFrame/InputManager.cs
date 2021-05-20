using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PbUISystem;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var topPanel= UIManager.Instance.GetTopPanel();
            if (!(topPanel is UIMainMenuPanel ))
            {
                if (topPanel is UIDialogPanel)
                {
                    UIManager.Instance.OpenPanel<UIGameMenuPanel>();
                }
                else
                {
                    UIManager.Instance.CloseTopPanel();
                }
            }
        }
        //if (Input.GetMouseButtonDown(0))
        //{
        //    DialogManager.Instance.Talk();
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    DialogManager.Instance.HideDialog();
        //}
    }
}
