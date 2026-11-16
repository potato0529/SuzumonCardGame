using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBGM : MonoBehaviour
{
    [Header("‹ÈƒŠƒXƒg")] public AudioSource[] bgmList;
    private AudioSource runningBgm;
    
    // Start is called before the first frame update
    void Start()
    {
        int bgmValue = MatchManager.instance.duelRandom.Range(0, bgmList.Length);
        bgmList[bgmValue].Play();
        runningBgm = bgmList[bgmValue];
    }

    /// <summary>
    /// Ä¶’†‚Ìbgm‚ğ~‚ß‚é
    /// </summary>
    public void StopBGM()
    {
        runningBgm.Stop();
        runningBgm = null;
    }
}
