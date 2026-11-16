using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_StartButton : MonoBehaviour
{
    private bool firstPush;

    public void Clicked()
    {
        Debug.Log("ゲームスタート！");
        //初回クリックのみシーン遷移の処理を行う
        if (!firstPush)
        {
            Debug.Log("メニューシーンに遷移します...");
            SceneManager.LoadScene("Menu");
        }
    }
}
