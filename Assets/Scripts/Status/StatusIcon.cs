using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIcon : MonoBehaviour
{
    [Header("アイコンマネージャー")] public StatusIconsManager statusIconsManager;
    [Header("参照するステータス")] public Status status;
    [Header("アイコン表示優先度")] public int displayPriority;
    [Header("ステータス説明")] public GameObject statusDescription;
    [Header("ステータス説明テキスト")] public TMP_Text statusDescriptionText;

    [SerializeField] private bool enabledIcon;//このアイコンを表示してよい状態であるか
    public bool EnabledIcon
    {
        get { return enabledIcon;}
        set { enabledIcon = value;UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, $"{this}の有効状態が{enabledIcon}に変更されました！");}
    }
    private bool activeIcon;//このアイコンが表示されているか
    [HideInInspector] public ObjToUGUI objToUGUI;
    private StatusInfo statusInfo;
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        objToUGUI = GetComponent<ObjToUGUI>();
        if (status != null) //pin
        {
            ChangedValue(status.StatusValue);
        }
    }

    public void SetStatusInfo(StatusInfo setStatusInfo)
    {
        statusInfo = setStatusInfo;
        GetComponent<SpriteRenderer>().sprite = setStatusInfo.iconSprite;
    }

    private void Update()
    {
        CheckEnableIcon();
    }

    /// <summary>
    /// アイコンマネージャーがアイコンを開いて表示するとき、有効であるこのアイコンを表示する
    /// </summary>
    public void DisplayIcon()
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, statusIconsManager.card + "のアイコンを表示します");
        //非表示から表示に切り替わるときのみ操作を行う
        if (!activeIcon)
        {
            statusIconsManager.activeIcons.Add(this);
        }
        activeIcon = true;
    }

    /// <summary>
    /// アイコンマネージャーがアイコンを閉じて非表示にするとき、有効であるこのアイコンを非表示にする
    /// </summary>
    public void HiddenIcon()
    {
        //表示から非表示に切り替わるときのみ操作を行う
        if (activeIcon)
        {
            statusIconsManager.activeIcons.Remove(this);
            objToUGUI.ReleaseLink();
        }
        activeIcon = false;
    }

    /// <summary>
    /// アイコンが有効な状態か判断してenableIconを変更する
    /// </summary>
    public virtual void CheckEnableIcon()
    {
        if (status.card.cardStatuses.statusesDictionary.ContainsKey(status.statusInfo.statusType) && (!status.statusInfo.doRemoveLower0Status || status.StatusValue > 0))//pin status
        {
            EnableIcon();
        }
        else
        {
            DisableIcon();
        }
    }

    /// <summary>
    /// 条件がそろったとき、アイコンを有効にする
    /// </summary>
    public void EnableIcon()
    {
        //無効から有効に切り替わるときのみリストを操作する
        if (!enabledIcon)
        {
            //Debug.Log(gameObject.name + "を有効にします",gameObject);
            enabledIcon = true;
            statusIconsManager.AddEnableList(this);
        }
    }

    /// <summary>
    /// 条件がそろったとき、アイコンを無効にする
    /// </summary>
    public void DisableIcon()
    {
        //有効から無効に切り替わるときのみリストを操作する
        if (enabledIcon)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, gameObject.name + "を無効にします");
            enabledIcon = false;
            statusIconsManager.RemoveEnableList(this);
            Destroy(this);
        }
    }
    
    /// <summary>
    /// 値が変更されたときUIテキストを更新する
    /// </summary>
    /// <param name="value"></param>
    public void ChangedValue(int value)
    {
        //Debug.Log("アイコンの値を更新します");
        if (objToUGUI.linked)
        {
            objToUGUI.UpdateInfo(value);
        }
        else
        {
            //Debug.Log("UGUIとリンクできていません");
        }
    }

    public void LinkStatusScript(Status thisStatus)
    {
        if (status == null) //pin !status
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, $"アイコン {this} と ステータス　{thisStatus} をリンクします");
            status = thisStatus;
            status.observingIcon = this;
        }
    }

    public void RemovedStatusScript()
    {
        status = null;
    }

    /// <summary>
    /// ステータスの説明を取得する
    /// </summary>
    public string GetStatusDescription()
    {
        switch (status.statusInfo.statusType)
        {
            case StatusData.StatusType.Attack:
                return "このモンスターが相手ライフにアタックしたときに与えるダメージ量。";

            case StatusData.StatusType.WithStand:
                return "このカードは" + status.StatusValue + "回破壊を耐えることができる";

            case StatusData.StatusType.Dodge:
                return "このカードは";

            case StatusData.StatusType.Paralysis:
                return "このカードは" + status.StatusValue + "ターン後までアタック・ディフェンスを行うことができず、バリアが無くなってしまう";
            case StatusData.StatusType.Preserved:
                return "このカードは" + status.StatusValue + "ターン後まで何も行動できず、他から干渉もされない状態で保存される";
            case StatusData.StatusType.Poison:
                return "このカードは" + status.StatusValue + "ターン後に破壊され、持ち主のプレイヤーは" + "" + "ダメージ受けてしまう";

            case StatusData.StatusType.Electricity:
                return "電気ステータス。主に特定のカードの効果の使用条件として指定されたり消費されたりする";

            case StatusData.StatusType.Mining:
                return "採掘ステータス。「カード鉱山」の効果で自分ターン開始時 1 貯まり、2 貯まると1枚ドローできる。";

            case StatusData.StatusType.Energy:
                return "エネルギーステータス。主にこのカードの効果で増減し、このカードの効果の条件として指定されたり消費されたりする。";

            case StatusData.StatusType.Blood:
                return "ブラッドステータス。カードが破壊されるごとに1貯まり、「ブラッディ」カテゴリカードの効果で貯まることもあり、主にこのカテゴリのカードの効果によって消費される。";

            case StatusData.StatusType.SpellMemory:
                return "スペルメモリーステータス。「魔導書人形ペーパペット」カードが破壊されるたびに1貯まる。「魔導書アンライフ」カードの効果の使用条件として消費される。";

            case StatusData.StatusType.Noroi:
                return "のろいステータス。自分ターン終了時、自分の場のモンスターが受けているのろいの数だけ手札を選んで捨てなければならない。のろいを持ったモンスターが場から離れれば解除される。";
        }
        return "";
    }
}
