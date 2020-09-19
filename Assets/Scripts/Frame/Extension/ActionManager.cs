using System;
using System.Collections.Generic;

public enum ActionType
{
    GameLoad,
    GameSave,
}

public class ActionBase
{
    private Action<object[]> action;
    public void RegistAction(Action<object[]> action)
    {
        this.action += action;
    }
    public void RemoveAction(Action<object[]> action)
    {
        this.action -= action;
    }

    public void CallBack(params object[] obj)
    {
        if (action != null)
        {
            foreach (var act in action.GetInvocationList())
            {
                var tempAction = (Action<object[]>)act;
                tempAction(obj);
            }
        }
        else
        {
            throw new Exception("委托为空，没有要执行的委托");
        }
    }
}

public class ActionManager
{
    private static ActionManager _instance;
    public static ActionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ActionManager();
            }
            return _instance;
        }
    }
    //保存所有注册的委托
    private Dictionary<ActionType, ActionBase> actions = new Dictionary<ActionType, ActionBase>();

    /// <summary>
    /// 注册委托
    /// </summary>
    /// <param name="type">委托类型</param>
    /// <param name="action">委托方法</param>
    public void AddListener(ActionType type, Action<object[]> action)
    {
        UnRegistAction(type, action);
        if (!actions.ContainsKey(type))
        {
            actions.Add(type, new ActionBase());
        }
        actions[type].RegistAction(action);
    }

    /// <summary>
    /// 将重复注册的委托移除
    /// </summary>
    /// <param name="type">委托</param>
    /// <param name="action">委托</param>
    private void UnRegistAction(ActionType type, Action<object[]> action)
    {
        if (actions.ContainsKey(type))
        {
            actions[type].RemoveAction(action);
        }
    }

    /// <summary>
    /// 将所有委托清除
    /// </summary>
    public void ClearAction()
    {
        actions.Clear();
    }

    /// <summary>
    /// 执行委托
    /// </summary>
    /// <param name="type">委托类型</param>
    public void Invoke(ActionType type, params object[] obj)
    {
        if (actions.ContainsKey(type))
            actions[type].CallBack(obj);
    }

    /// <summary>
    /// 移除委托
    /// </summary>
    /// <param name="type"></param>
    public void RemoveAction(ActionType type)
    {
        if (actions.ContainsKey(type))
        {
            actions[type] = null;
            actions.Remove(type);
        }
    }

    /// <summary>
    /// 判断是否有委托
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool HaveAction(ActionType type)
    {
        if (actions.ContainsKey(type))
        {
            return true;
        }
        return false;
    }
}
