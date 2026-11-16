using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideSupportGuide : MonoBehaviour
{
    [Header("ガイドテキスト")] public TMP_Text guideText;
    [Header("折りたたみボタン")] public Image foldButton;

    [Header("折り畳みアニメーション距離/フレーム")] public float foldAnimSpeed;

    private bool isFolded = false;

    private RectTransform rt;
    private bool duringAnimation = false;

    private Vector2 originPos;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        originPos = rt.anchoredPosition;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGuideText(string text)
    {
        guideText.text = text;
    }

    public void OnClickedFoldButton()
    {
        if (duringAnimation)
            return;

        isFolded = !isFolded;
        if(isFolded)
        {
            // ガイドを折りたたむ処理
            StartCoroutine(FoldAnim());
            foldButton.rectTransform.localScale = new Vector3(-1, 1, 1); // ボタンの向きを反転
        }
        else
        {
            // ガイドを展開する処理
            StartCoroutine(OpenAnim());
            foldButton.rectTransform.localScale = new Vector3(1, 1, 1); // ボタンの向きを元に戻す
        }
    }

    private IEnumerator FoldAnim()
    {
        while(rt.anchoredPosition.x < -originPos.x)
        {
            rt.anchoredPosition += new Vector2(foldAnimSpeed,0);
            yield return null;
        }
        rt.anchoredPosition = new Vector2(-originPos.x, rt.anchoredPosition.y);

    }

    private IEnumerator OpenAnim()
    {
        while (rt.anchoredPosition.x > originPos.x)
        {
            rt.anchoredPosition += new Vector2(-foldAnimSpeed, 0);
            yield return null;
        }
        rt.anchoredPosition = new Vector2(originPos.x, rt.anchoredPosition.y);
    }
}
