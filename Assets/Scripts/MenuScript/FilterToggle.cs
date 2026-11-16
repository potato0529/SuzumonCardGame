using UnityEngine;
using UnityEngine.UI;

public class FilterToggle : MonoBehaviour
{
    [Header("トグル")]public Toggle toggle;
    [Header("背景")] public Image backGroundImage;
    [Header("オン時の色")] public Color onColor;
    [Header("オフ時の色")] public Color offColor;

    public int indexNum; //本来配列の要素が自身のインデックス番号を知れるのは不適切な場合が多いが、今回は用途的に厳密な設計が不要なのと利便性を重視する。

    /// <summary>
    /// トグルされたとき ref+1
    /// </summary>
    /// <param name="toggle"></param>
    public void OnToggled(bool isOn)
    {
        SetColorFromToggle();
        //UIからtoggle.isOnが変更されているので、SetToggle()を呼ぶ必要はない
    }

    /// <summary>
    /// トグルの状態をUIではなく外部から設定する
    /// </summary>
    /// <param name="isOn"></param>
    public void SetToggle(bool isOn)
    {
        toggle.isOn = isOn;
        SetColorFromToggle();
    }

    /// <summary>
    /// トグルのオンオフに応じて背景色を変更する
    /// </summary>
    public void SetColorFromToggle()
    {
        if (toggle.isOn)
        {
            backGroundImage.color = onColor;
        }
        else
        {
            backGroundImage.color = offColor;
        }
    }
}
