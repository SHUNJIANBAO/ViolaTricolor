using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageImageConfig : CsvCfg<LanguageImageConfig> 
{
    public string JP { get; protected set; }
    public string EN { get; protected set; }
    public static string FilePath = "Config/LanguageImageConfig";
}
