using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyer : MonoBehaviour
{
    [Header("”j‰óŽžŠÔ")] public float destroyFrame;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        destroyFrame--;
        if (destroyFrame <= 0)
        {
            Debug.Log("Á‚¦‚½");
            Destroy(gameObject);

        }
    }
}
