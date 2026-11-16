using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カードに直接アタッチはされていないけど同じオブジェクトにプレイヤーの情報があるときなどに使う
/// </summary>
public interface IOwnerPlayer
{
    Player GetPlayer();
}
