using UnityEngine;

/// <summary>
/// インスペクターにメモを表示するためのクラス
/// </summary>
public class Memo : MonoBehaviour
{
    [Header("メモ")][TextArea(3, 10)] public string memo;
}
