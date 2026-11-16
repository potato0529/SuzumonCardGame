using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjToUGUI : MonoBehaviour
{
    [Header("UGUIのコピー元のオブジェクト")] public GameObject copySource;
    [Header("カード")] public Card card;

    protected SpriteRenderer sr;
    [HideInInspector] public bool linked;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        //UIに表示する用なのでsrのほうは非表示
        sr.enabled = false;
    }

    /// <summary>
    /// リンクUGUIの初期設定
    /// </summary>
    /// <param name="linkedUGUI">初期設定をするリンクUGUI</param>
    /// <param name="canvas">リンクUGUIの親に設定するキャンバス</param>
    public void InitialSetting(LinkedUGUI linkedUGUI,Player viewThisPlayer)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, $"{linkedUGUI} の初期設定をし、{viewThisPlayer} のキャンバスに表示します");
        linkedUGUI.viewThisPlayer = viewThisPlayer;
        linkedUGUI.transform.SetParent(viewThisPlayer.playerCanvas.linkedGUILayer);
        linkedUGUI.image.sprite = sr.sprite;
        linkedUGUI.LinkSource = this;
        linkedUGUI.UpdatePosition();
        linkedUGUI.transform.SetAsFirstSibling();
        //linkedUGUI.GetComponent<RectTransform>().localScale = new Vector3(transform.scale.x, transform.localScale.y, transform.localScale.z);
    }

    public virtual void GenerateUGUI()
    {
        linked = true;
    }

    /// <summary>
    /// 表示されているリンクUGUIの親をカードに戻す。(表示されなくなる)
    /// </summary>
    public virtual void ReleaseLink()
    {
        linked = false;
    }

    /// <summary>
    /// リンクUGUIが持つ情報を更新する
    /// </summary>
    /// <param name="value"></param>
    public virtual void UpdateInfo(int value)
    {

    }
}
