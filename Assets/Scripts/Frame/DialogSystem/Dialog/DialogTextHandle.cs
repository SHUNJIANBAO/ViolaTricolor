using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DialogTextHandle : MonoSingleton<DialogTextHandle>
{
    /// <summary>
    /// 用于匹配标点符号（正则表达式）
    /// </summary>
    private readonly string strRegex = @"(\！|\？|\，|\。|\《|\》|\（|\）|\(|\)|\：|\“|\‘|\、|\；|\+|\-|\·|\#|\￥|\；|\”|\【|\】|\——|\/)";


    /// <summary>
    /// 用于存储text组件中的内容
    /// </summary>
    private System.Text.StringBuilder MExplainText = null;

    /// <summary>
    /// 用于存储text生成器中的内容
    /// </summary>
    private IList<UILineInfo> MExpalinTextLine;

    Text _text;

    private void Start()
    {
        if (_text == null) _text = GetComponent<Text>();
    }

    public string FinalText => _text.text;

    public IEnumerator MClearUpExplainMode(Text component, string text)
    {
        _text.rectTransform.sizeDelta = component.rectTransform.sizeDelta;
        _text.fontSize = component.fontSize;

        _text.text = text;

        //如果直接执行下边方法的话，那么_component.cachedTextGenerator.lines将会获取的是之前text中的内容，而不是_text的内容，所以需要等待一下
        yield return new WaitForSeconds(0.001f);
        MExpalinTextLine = _text.cachedTextGenerator.lines;
        MExplainText = new System.Text.StringBuilder(_text.text);


        for (int i = 1; i < MExpalinTextLine.Count; i++)
        {
            int CheckId = MExpalinTextLine[i].startCharIdx;
            //首位是否有标点
            while (Regex.IsMatch(_text.text[CheckId].ToString(), strRegex))
            {
                CheckId--;
                if (CheckId == 0 || CheckId - 1 == 0)
                {
                    break;
                }
                //不考虑标点符号前有人为换行的情况
                if (_text.text[CheckId].ToString() == "\n")
                {
                    MExplainText.Remove(CheckId, 1);
                }
                else
                {
                    MExplainText.Insert(CheckId, '\n');
                }
            }
        }


        _text.text = MExplainText.ToString();
    }
}
