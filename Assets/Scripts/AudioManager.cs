using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Šî”Õƒ\[ƒX")] public AudioSource audioSource;
    [Header("‘Ï‚¦‚éÁ”ï‰¹")] public AudioClip withStandSound;

    private float baseVolume;

    private void Awake()
    {
        baseVolume = audioSource.volume;
    }

    /// <summary>
    /// Œø‰Ê‰¹‚ğ1‰ñÄ¶‚·‚é
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlaySoundClip(AudioClip audioClip,float volume)
    {
        audioSource.volume = baseVolume * volume;
        audioSource.PlayOneShot(audioClip);
    }
}
