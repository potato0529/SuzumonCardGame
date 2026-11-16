using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] Slider mainVolumeSlider;
    [SerializeField] Slider bgmVolumeSlider;
    [SerializeField] Slider seVolumeSlider;
    private float currentVolume = -1.0f;

    private void Update()
    {
        if (Mathf.Approximately(currentVolume, -30 + mainVolumeSlider.value * 50)) return;

        audioMixer.SetFloat("Master", -30 + mainVolumeSlider.value * 50);
        currentVolume = mainVolumeSlider.value;
        
    }
}
