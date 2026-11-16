using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("Á–ÅŽžŠÔ")] public float destroyTime;

    private float timer;
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
