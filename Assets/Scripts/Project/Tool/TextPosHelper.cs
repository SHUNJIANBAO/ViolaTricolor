using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextPosHelper 
{

    public Text textComp;
    public Canvas canvas;

    public Text text;

    public Vector3 GetPosAtText(Canvas canvas, Text text, string strFragment)
    {
        int strFragmentIndex = text.text.IndexOf(strFragment);//-1��ʾ������strFragment
        Vector3 stringPos = Vector3.zero;
        if (strFragmentIndex > -1)
        {
            Vector3 firstPos = GetPosAtText(canvas, text, strFragmentIndex + 1);
            Vector3 lastPos = GetPosAtText(canvas, text, strFragmentIndex + strFragment.Length);
            stringPos = (firstPos + lastPos) * 0.5f;
        }
        else
        {
            stringPos = GetPosAtText(canvas, text, strFragmentIndex);
        }
        return stringPos;
    }

    /// <summary>
    /// �õ�Text���ַ���λ�ã�canvas:���ڵ�Canvas��text:��Ҫ��λ��Text,charIndex:Text�е��ַ�λ��
    /// </summary>
    public static Vector3 GetPosAtText(Canvas canvas, Text text, int charIndex)
    {
        string textStr = text.text;
        Vector3 charPos = Vector3.zero;
        if (charIndex <= textStr.Length && charIndex > 0)
        {
            TextGenerator textGen = new TextGenerator(textStr.Length);
            Vector2 extents = text.gameObject.GetComponent<RectTransform>().rect.size;
            textGen.Populate(textStr, text.GetGenerationSettings(extents));

            int newLine = textStr.Substring(0, charIndex).Split('\n').Length - 1;
            int whiteSpace = textStr.Substring(0, charIndex).Split(' ').Length - 1;
            int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;
            if (indexOfTextQuad < textGen.vertexCount)
            {
                charPos = (textGen.verts[indexOfTextQuad].position +
                    textGen.verts[indexOfTextQuad + 1].position +
                    textGen.verts[indexOfTextQuad + 2].position +
                    textGen.verts[indexOfTextQuad + 3].position) / 4f;


            }
        }
        charPos /= canvas.scaleFactor;//��Ӧ��ͬ�ֱ��ʵ���Ļ

        charPos.x -= text.fontSize / 2;
        charPos.y += text.fontSize / 2;

        charPos = text.transform.TransformPoint(charPos);//ת��Ϊ��������
        return charPos;
    }

}