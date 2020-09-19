using UnityEngine;


public abstract class Data<T> where T : Data<T>, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Load();
            }
            return _instance;
        }
    }

    public static void Load()
    {
        string json = PlayerPrefs.GetString(typeof(T).Name);
        if (string.IsNullOrEmpty(json))
            _instance = new T();
        else
            _instance = JsonUtility.FromJson<T>(json);
        _instance.OnLoad();
    }
    protected virtual void OnLoad() { }
    public static void Save()
    {
        string json = JsonUtility.ToJson(_instance);
        PlayerPrefs.SetString(typeof(T).Name, json);
    }

    public static void Clear()
    {
        string json = PlayerPrefs.GetString(typeof(T).Name);
        if (string.IsNullOrEmpty(json))
            return;
        else
            PlayerPrefs.SetString(typeof(T).Name, null);
    }
}
