using UnityEngine;

public class MatchCardHighlight : MonoBehaviour
{
    [Header("カードハイライトの各辺のボーダーオブジェクト")] public GameObject[] highlightBorders;//インスペクターで編集するためにpublicだが、外部から参照されることを想定していない
    [Header("カードハイライトの親")] public GameObject highlightsParent;//4辺のハイライトボーダーの親
    private Vector3 highlightParentOriginScale;

    private SpriteRenderer[] highlightBorderSrs;
    private float animTime = 0;

    private void Awake()
    {
        highlightParentOriginScale = highlightsParent.transform.localScale;

        //ハイライトボーダーのレンダラーを取得
        highlightBorderSrs = new SpriteRenderer[highlightBorders.Length];
        for (int index = 0; index < highlightBorders.Length; index++)
        {
            highlightBorderSrs[index] = highlightBorders[index].GetComponent<SpriteRenderer>();
            highlightBorderSrs[index].enabled = false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        animTime += Time.deltaTime;
        if(animTime < MatchManager.instance.matchConfig.cardHighlightAnimCycleTime)
        {
            //minScale～特定の値までで波を作りたい。asinは-a～aになってしまうので、minScale+a+asinとする。また、インスペクターの値が実際の拡大率に一致してほしいので、sinの幅は-n～nで2nであるから、/2をする。また、既にminScaleあるのに2倍したい場合minScale+1*2になってしまうから、AnimSinScale-minScaleとしている。
            float scale = MatchManager.instance.matchConfig.cardHighlightAnimMinScale + (MatchManager.instance.matchConfig.cardHighlightAnimSinScale - MatchManager.instance.matchConfig.cardHighlightAnimMinScale) / 2 + Mathf.Sin(animTime / MatchManager.instance.matchConfig.cardHighlightAnimCycleTime * Mathf.PI * 2) * (MatchManager.instance.matchConfig.cardHighlightAnimSinScale - MatchManager.instance.matchConfig.cardHighlightAnimMinScale) / 2;
            foreach (GameObject highlightBorder in highlightBorders)
            {
                highlightsParent.transform.localScale = highlightParentOriginScale * scale;
            }
        }
        else
        {
            animTime = 0;
        }
    }

    /// <summary>
    /// セレクトフレームの制御
    /// </summary>
    /// <param name="enable">有効か無効か</param>
    public void HighlightBorderEnable(bool enable)
    {
        if (enable)
        {

            foreach (SpriteRenderer sr in highlightBorderSrs)
            {
                sr.enabled = true;
            }
        }
        else
        {

            foreach (SpriteRenderer sr in highlightBorderSrs)
            {
                sr.enabled = false;
            }
        }

    }

    /// <summary>
    /// セレクトフレームの色を変える
    /// </summary>
    /// <param name="color"></param>
    public void ChangeHighlightBorderColor(Color color)
    {
        foreach (SpriteRenderer sr in highlightBorderSrs)
        {
            sr.color = color;
        }
    }

}
