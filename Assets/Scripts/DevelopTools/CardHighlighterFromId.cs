#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class CardHighlighterFromId : MonoBehaviour
{
    [Header("ハイライトするカードのID")] public int cardId;

    [ContextMenu("HighlightCard")]
    public void HighlightCardFromId()
    {
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(MatchManager.instance.GetGameCards()[cardId].gameObject);
#endif
    }

    [ContextMenu("HighlightSearchCard0")]
    public void HighlightCardFromSearchId0()
    {
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(MatchManager.instance.players[0].searchCard.inSearchWindowSCards[cardId].gameObject);
#endif     
    }

    [ContextMenu("HighlightSearchCard1")]
    public void HighlightCardFromSearchId1()
    {
#if UNITY_EDITOR
        EditorGUIUtility.PingObject(MatchManager.instance.players[1].searchCard.inSearchWindowSCards[cardId].gameObject);
#endif     
    }

}
