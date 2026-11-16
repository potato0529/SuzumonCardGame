using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance = null;
    public MenuTransition menuTransition;

    private void Awake()
    {
        instance = this;
        //‡‚©‚ç–ß‚Á‚½‚Ìˆ—
        if (GameManager.instance.isAfterMatchTransition)
        {
            Debug.Log("‡‚©‚ç–ß‚è‚Ü‚µ‚½");
            GameManager.instance.isAfterMatchTransition = false;
            GameManager.instance.matchMaker.matchPlayerDatas[0].deckCards = new List<CardData>();
            GameManager.instance.matchMaker.matchPlayerDatas[0].specialDeckCards = new List<CardData>();
            GameManager.instance.matchMaker.matchPlayerDatas[1].deckCards = new List<CardData>();
            GameManager.instance.matchMaker.matchPlayerDatas[1].specialDeckCards = new List<CardData>();
        }
    }
}
