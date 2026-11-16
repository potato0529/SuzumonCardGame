using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一旦はCPUが順番にカードを配置するための仕組みとするが、CPU関係なく汎用的に配置されている場所や重複しない新たな配置場所を見つけるシステムでプレイヤーもCPUもいちげんかんりすることになった場合このクラスと性質が大きく違うので、このクラスを消すか放置して全く新しい仕組みを作る必要があるかも
/// </summary>
public class FieldCardPlacement : MonoBehaviour
{
    [Header("フィールドにカードを並べる用のスロット(自由設置モードの場合無関係）")] public FieldCardSlot[] fieldCardSlots = new FieldCardSlot[20];

    /// <summary>
    /// 新たに場に置くカードをスロットに割り当て、置くべき場所を返す。あくまで配置そのものをしてくれるシステムではなく、配置すべき場所を示して配置されている場所を記録し、他と重複しないよう管理するという、仮想上の数値を管理する統制局のような立ち位置。配置すべき場所を示すと同時に配置場所を、実際に配置されたはずだとみなして記録してしまうため、配置すべき場所を知りたいだけという目的で呼んではならない。 ※もしこの配置システムをもっと汎用的にし、この配置システムがカードのpositionを直接いじってくれるようにしたい場合は少し仕組みを変える必要あり
    /// </summary>
    /// <param name="card">配置されたカード</param>
    public Vector3 Placed(Card card)
    {
        for(int slotNum = 0;slotNum < fieldCardSlots.Length; slotNum++)
        {
            FieldCardSlot fieldCardSlot = fieldCardSlots[slotNum];  
            if(fieldCardSlot.card == null)
            {
                fieldCardSlot.SetCard(card);
                return fieldCardSlot.transform.position;
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// カードが再配置されたときにそれを記録する
    /// </summary>
    /// <param name="card"></param>
    public void Replaced(Card card)
    {
        //今のところ必要ないので未実装 CPUがカードを再配置しないので
    }

    public void LeaveField(Card card)
    {
        for (int slotNum = 0; slotNum < fieldCardSlots.Length; slotNum++)
        {
            FieldCardSlot fieldCardSlot = fieldCardSlots[slotNum];
            if (card == fieldCardSlot.card)
            {
                fieldCardSlot.ReleaseCard();
                break;
            }
        }
    }
}
