using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWithUGUI : MonoBehaviour,ILikendUGUIInfo
{
    private Text text;
    private TMP_Text tmp_Text;
    private void Awake()
    {
        text = GetComponent<Text>();
        tmp_Text = GetComponent<TMP_Text>();
    }

    public void LinkInfo(ref int variable)
    {

    }

    public void UpdateInfo(int value)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI,"テキストUGUIを更新しました");
        //text.text = value.ToString();
        tmp_Text.text = value.ToString();
    }
}
