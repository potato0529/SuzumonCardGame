using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchLog : MonoBehaviour
{
    public enum IntMatchData
    {
        EffectInvoked,
        Attacked,
        Defenced,
        Destroyed,
        SentCemetery,
        DuelTime,
        ElapsedTurn
    }

    public Dictionary<IntMatchData, int> intMatchDatas = new Dictionary<IntMatchData, int>();
}
