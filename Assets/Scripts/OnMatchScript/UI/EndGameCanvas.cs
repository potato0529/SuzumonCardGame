using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EndGameCanvas : MonoBehaviourPunCallbacks
{
    public GameObject matchResultParent;
    public MatchResult matchResult;

    public void GoToResult()
    {
        Debug.Log("Œ‹‰Ê‰æ–Ê‚É‘JˆÚ‚µ‚Ü‚·");
        matchResultParent.gameObject.SetActive(true);
    }

    public void GoMenu()
    {
        StartCoroutine(LeaveMatch());
    }

    /// <summary>
    /// ‘Îí‚ğ‹‚é‚Æ‚«‚Ìˆ—(‰æ–Ê‚Ì‘JˆÚ‚â•”‰®‚Ì‘Şo‚È‚Çj
    /// </summary>
    /// <returns></returns>
    public IEnumerator LeaveMatch()
    {
        if (!MatchManager.instance.isSoloMode)
        {
            PhotonNetwork.LeaveRoom();
        }

        yield return new WaitForSeconds(1);
        GameManager.instance.isAfterMatchTransition = true;
        GameManager.instance.TransitionMenu(MenuTransition.MenuType.MainMenu);
    }

    public void ViewMatch()
    {
        matchResultParent.SetActive(false);
    }
}
