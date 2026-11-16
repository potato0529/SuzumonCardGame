using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UniqueTextManager : MonoBehaviour
{
    [Header("ユニークテキスト")] public List<UniqueText> uniqueTexts;

    /// <summary>
    /// ユニークテキストにリンクが埋め込まれたテキストに変換する
    /// </summary>
    /// <param name="originalText"></param>
    /// <returns></returns>
    public string ConvertDescriptedText(string originalText)
    {
        string convertedText = originalText;
        foreach(UniqueText uniqueText in uniqueTexts)
        {
            int containKeywordIndex = convertedText.IndexOf(uniqueText.keyWord);
            if(containKeywordIndex != -1)
            {
                //Debug.Log(containKeywordIndex + "," + uniqueText.keyWord);
                string insertText = "<link=\"" + uniqueText.keyWord + "\">";
                convertedText = convertedText.Insert(containKeywordIndex, insertText);
                convertedText = convertedText.Insert(containKeywordIndex + insertText.Length + uniqueText.keyWord.Length, "</link>");

                string colorText = "<color=yellow>";
                convertedText = convertedText.Insert(containKeywordIndex + insertText.Length, colorText);
                convertedText = convertedText.Insert(containKeywordIndex + insertText.Length + uniqueText.keyWord.Length + colorText.Length, "</color>");
            }
        }
        return convertedText;
    }

    /// <summary>
    /// リンクに対応する説明テキストを取得する
    /// </summary>
    /// <param name="link"></param>
    /// <returns></returns>
    public string GetDescriptionCorrespondingToLink(string link)
    {
        foreach(UniqueText uniqueText in uniqueTexts)
        {
            if(link == uniqueText.keyWord)
            {
                return uniqueText.description;
            }
        }
        return "";
    }
}
