using System.Collections.Generic;
using UnityEngine;

public abstract class CsvCfg<T> where T : CsvCfg<T>, new()
{
    public string CN { get; protected set; }
    static Dictionary<string, T> dataMap;

    public static T GetData(string cn)
    {
        if (dataMap == null)
        {
            LoadCsvCfg();
        }
        return dataMap[cn];
    }

    public static bool HasData(string cn)
    {
        if (dataMap == null)
        {
            LoadCsvCfg();
        }
        return dataMap.ContainsKey(cn);
    }

    public static Dictionary<string, T> GetMap()
    {
        if (dataMap == null)
        {
            LoadCsvCfg();
        }
        return dataMap;
    }

    public static Dictionary<string, T> DataMap
    {
        get
        {
            if (dataMap == null)
            {
                LoadCsvCfg();
            }
            return dataMap;
        }
    }

    protected virtual void OnInit() { }

    public static void LoadCsvCfg()
    {
        string path = typeof(T).GetField("FilePath").GetValue(null).ToString();
        TextAsset asset = Resources.Load<TextAsset>(path);
        ReadText(asset.text);
    }

    static void ReadText(string text)
    {
        dataMap = new Dictionary<string, T>();
        string[] lines = text.Split('\n');
        string[] keys = ReadLine(lines[1]);
        for (int i = 2; i < lines.Length; i++)
        {
            Dictionary<string, string> tempDict = new Dictionary<string, string>();
            string[] values = ReadLine(lines[i]);
            if (string.IsNullOrEmpty(values[0])) continue;
            for (int j = 0; j < keys.Length; j++)
            {
                tempDict.Add(keys[j], values[j]);
            }
            WriteToCfg(tempDict);
        }
    }

    static void WriteToCfg(Dictionary<string, string> cfgDict)
    {
        var infos = typeof(T).GetProperties();
        T cfg = new T();
        string cn = "";
        foreach (var info in infos)
        {
            if (cfgDict.ContainsKey(info.Name))
            {
                if (info.Name == "CN")
                {
                    cn = cfgDict[info.Name];
                    if (string.IsNullOrEmpty(cn)) break;
                    info.SetValue(cfg, cn, null);
                }
                else
                {
                    var obj = Util.GetValue(cfgDict[info.Name], info.PropertyType, info.Name);
                    info.SetValue(cfg, obj, null);
                }
            }
        }
        if (!string.IsNullOrEmpty(cn))
        {
            dataMap.Add(cn, cfg);
        }
    }

    static string[] ReadLine(string line)
    {
        string[] tempArray = line.Split(',');
        tempArray[tempArray.Length - 1] = tempArray[tempArray.Length - 1].TrimEnd('\r');
        return tempArray;
    }
}
