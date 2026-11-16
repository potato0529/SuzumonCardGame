using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コライダーとマウスが触れているかを確認するときなどのために用いる。
/// </summary>
public class MousePosGUI : MonoBehaviour
{
    private RectTransform rt;
    private RectTransform rtParent;
    private Rigidbody2D rb;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rtParent = transform.parent.gameObject.GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 MousePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rtParent,
            Input.mousePosition,
            null,
            out MousePos);

        rt.anchoredPosition = new Vector2(MousePos.x, MousePos.y);
    }
}
