using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPGuage : MonoBehaviour
{
    [Header("プレイヤー")] public Player pScr;
    //[Header("ゲージフレーム")] public RectTransform guageFrame;
    //[Header("+-ゲージを表示する場所(空Obj)")] public RectTransform oldDifferenceGuagePos;
    //[Header("ライフ差ゲージ")] public RectTransform oldDifferenceGuage;
    [Header("緑色のゲージ")] public Image mainGuage;
    [Header("変化ゲージ")] public Image differenceGuage;
    [Header("ライフテキスト")] public TMP_Text lifeText;

    //private RectTransform rectTransform;

    private Vector3 defaultLifeTextPos;

    void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
        defaultLifeTextPos = lifeText.rectTransform.localPosition;
    }

    /// <summary>
    /// HPゲージを更新する
    /// </summary>
    /// <param name="lifeDifference"></param>
    public void UpdateGuage(int lifeDifference)
    {
        //旧ゲージ
        //rectTransform.localScale = new Vector2(pScr.life * 0.1f, rectTransform.localScale.y);

        lifeText.text = pScr.life.ToString();
        mainGuage.fillAmount = pScr.life * 0.1f;
        StartCoroutine(UpdateGuageAnim(lifeDifference));
    }

    /// <summary>
    /// 変化したHPゲージの差を表示するゲージのアニメーション
    /// </summary>
    /// <param name="lifeDifference"></param>
    /// <returns></returns>
    public IEnumerator UpdateGuageAnim(int lifeDifference)
    {
        //旧ゲージ
        //oldDifferenceGuage.gameObject.SetActive(true);
        //oldDifferenceGuage.position = oldDifferenceGuagePos.position;
        ////ゲージのフレームの大きさの倍率に合わせてスケールを調整
        //oldDifferenceGuage.localScale = new Vector2(-lifeDifference * 0.1f * guageFrame.localScale.x, oldDifferenceGuage.localScale.y);

        differenceGuage.fillAmount = Mathf.Abs(lifeDifference) * 0.1f;
        differenceGuage.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, pScr.life * -36);

        //ライフが減る場合か増える場合かでディふぇれんすゲージの色を変える
        if (lifeDifference > 0)
        {
            //旧ゲージ
            //oldDifferenceGuage.gameObject.GetComponent<Image>().color = Color.blue;

            differenceGuage.gameObject.GetComponent<Image>().color = Color.blue;
            differenceGuage.GetComponent<RectTransform>().localScale = new Vector3(-5.5f, 5.5f, 1);
        }
        else
        {
            //旧ゲージ
            //oldDifferenceGuage.gameObject.GetComponent<Image>().color = Color.red;

            differenceGuage.gameObject.GetComponent<Image>().color = Color.red;
            differenceGuage.GetComponent<RectTransform>().localScale = new Vector3(5.5f, 5.5f, 1);
        }

        yield return new WaitForSeconds(1);
        while(differenceGuage.fillAmount > 0) //旧ゲージ条件:oldDifferenceGuage.localScale.x > 0.01f || oldDifferenceGuage.localScale.x < -0.01f
        {
            //旧ゲージ
            //oldDifferenceGuage.localScale -= new Vector3(-lifeDifference * 0.005f, 0, 0);

            differenceGuage.fillAmount -= Mathf.Abs(lifeDifference) * 0.0015f;
            yield return null;            
        }

        //旧ゲージ
        //oldDifferenceGuage.localScale = new Vector2(0, oldDifferenceGuage.localScale.y);
        //oldDifferenceGuage.gameObject.GetComponent<Image>().color = Color.white;
        //oldDifferenceGuage.gameObject.SetActive(false);

        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "ライフアニメーションを終了しました");
    }

    /// <summary>
    /// デジタル数字フォントだと不自然なズレ方をする場合があるので、そのずれを補正する(仮、今は使ってない）
    /// </summary>
    public void MoveLifeTextForDigitalFont()
    {
        int lifeNum = pScr.life;
        if (lifeNum >= 10 && lifeNum <= 19)
        {
            lifeText.rectTransform.localPosition = defaultLifeTextPos + new Vector3(-14, 0, 0);
        }

        
    }
}
