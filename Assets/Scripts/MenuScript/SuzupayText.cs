using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuzupayText : MonoBehaviour
{
    private Text suzupayText;
    private UserData userData;
    private int oldSuzupay;

    private void Awake()
    {
        suzupayText = GetComponent<Text>();
        userData = GameManager.instance.userData;
    }

    void Update()
    {
        if (userData.Suzupay != oldSuzupay)
        {
            suzupayText.text = "Suzupay:" + userData.Suzupay;
        }

        oldSuzupay = userData.Suzupay;
    }
}
