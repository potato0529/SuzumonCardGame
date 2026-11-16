using UnityEngine;

/// <summary>
/// CPUがフィールドにカードを並べられるよう、カードを配置すべき場所を記録するスロット
/// </summary>
public class FieldCardSlot : MonoBehaviour
{
    [Header("このスロットに配置されたカード")] public Card card;//本当はprivateにしたいけどInspectorに表示したいので仕方なく

    /// <summary>
    /// スロットにカードをセットする(transform.positionには干渉せず、スロットの枠に割り当てるだけ)
    /// </summary>
    /// <param name="card"></param>
    public void SetCard(Card _card)
    {
        card = _card;
    }

    /// <summary>
    /// リストからカードを外す
    /// </summary>
    public void ReleaseCard()
    {
        card = null;
    }
}
