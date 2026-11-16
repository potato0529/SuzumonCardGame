using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardListArea : MonoBehaviour,IPointerOver
{
    [Header("動かすRectTransform")] public RectTransform moveTargetRT;
    [Header("スクロール速度 Recommend(50~500)")] public float scrollSpeed;
    private float scrollAmount;
    private Vector3 originalPos;

    public bool isPointerOver { get; set; }

    void Awake()
    {
        originalPos = moveTargetRT.position;
    }

    private void Update()
    {
        if (isPointerOver)
        {
            float aKey = Input.GetAxisRaw("AKey");
            float accel = 1;
            if (aKey > 0)
            {
                accel = 3.0f;
            }

            float wh = Input.GetAxis("Mouse ScrollWheel");
            scrollAmount += -wh * scrollSpeed * accel;
            if (scrollAmount < 0)
            {
                scrollAmount = 0;
            }

            
            moveTargetRT.transform.position = new Vector3(moveTargetRT.transform.position.x, originalPos.y + scrollAmount, moveTargetRT.transform.position.z);
        }
    }

    /// <summary>
    /// スクロールを元に戻す
    /// </summary>
    public void SetOriginalPos()
    {
        scrollAmount = 0;
    }
}
