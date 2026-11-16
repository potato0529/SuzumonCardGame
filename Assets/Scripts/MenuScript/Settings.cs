using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [Header("îwåi")] public GameObject settingBG;

    public void OpenSettings()
    {
        settingBG.SetActive(true);
    }

    public void CloseSettings()
    {
        settingBG.SetActive(false);
    }
}
