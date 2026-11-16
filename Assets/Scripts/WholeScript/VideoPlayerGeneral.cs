using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.UI;

public class VideoPlayerGeneral : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    [Header("再生遅延")] public float delayTime;


    private void Start()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        videoPlayer.loopPointReached += LoopPointReached;
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    public void LoopPointReached(VideoPlayer videoPlayer)
    {
        gameObject.SetActive(false);
    }

    private void OnPrepared(VideoPlayer videoPlayer)
    {
        Debug.Log("テスト");
        //DelayStartAnim().Forget();
        videoPlayer.Play();
        rawImage.enabled = true;
    }

    public async UniTask DelayStartAnim()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        videoPlayer.Play();
    }
}
