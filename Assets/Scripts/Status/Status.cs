using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status
{
    [Header("デュフォルトのステータスの値"), SerializeField] protected int defaultStatusValue;
    [Header("影響を受けているステータス変更効果"),SerializeField] public List<GivedStatus> givedStatuses = new List<GivedStatus>();
    [Header("値変更を監視するアイコン")] public StatusIcon observingIcon;
    [Header("アイコンを表示できるステータスであるか")] public bool isDisplayableIcon = true;
    [Header("ステータスが有効となる場所")] public List<Card.CardState> enableState = new List<Card.CardState>();
    [Header("アイコンのスプライト")] public Sprite iconSprite;

    protected int statusValue;
    protected int baseStatusValue;
    public Card card;
    private bool dontDestroyTiming;
    public StatusInfo statusInfo;


    public int StatusValue
    {
        get { return statusValue; }
        set { ChangedValue(value); }
    }

    public int DefaultStatusValue
    {
        get { return defaultStatusValue; }
        set
        {
            defaultStatusValue = value;
            StatusValue = defaultStatusValue;//降臨などでデュフォルトが更新されたとき、現在のステータスも更新される。かなりまれな処理なので、現在のステータスが更新されない例外は作る必要がないと考えた。
            UpdateValue();//多分必要なはず
        }
    }

     public int BaseStatusValue
    {
        get { return baseStatusValue; }
        set
        {
            baseStatusValue = value;
            UtilityLog.CardLog($"{statusInfo.statusType} ベースステータスを {value} にしました", card, UtilityLog.LogCategory.Status);
            UpdateValue();
        }
    }

    /// <summary>
    /// AddedStatus()とは別のシンプルな初期化処理 旧Awakeと同じ役割
    /// </summary>
    public virtual void Initialize(Card addedCard)
    {
        card = addedCard;
        card.leftFieldCallback.Add(LeftField);
    }

    /// <summary>
    /// ステータスが追加されたとき、アイコンなどを用意する処理
    /// </summary>
    /// <param name="value"></param>
    public virtual void AddedStatus(StatusData.StatusType statusType,int value,StatusInfo thisStatusInfo)
    {
        UtilityLog.CardLog("ステータス追加時の処理をします", card, UtilityLog.LogCategory.Status);//dd
        statusInfo = thisStatusInfo;
        //そのクラスに対応しているiconクラスをAddComponent? StatusIconはつけといてカスタマイズとしてAtkとかそれぞれつけるのもあり
        StatusIcon generatedIcon = card.cardStatuses.GenerateStatusIcon();
        generatedIcon.GetComponent<SpriteRenderer>().sprite = thisStatusInfo.iconSprite;//ここより先にstatusIconManagerのSortIconList()によるステータス表示処理が入ってしまうのであればタイミングを調整しよう
        card.statusIconsManager.AddStatus(this, value, generatedIcon);
        //base.AddedUniqueStatus(value);それぞれつけるのもあり
        
    }

    /// <summary>
    /// 値をセットする（ベースではなく確定のステータス。ベースを変更したかったらBaseStatusValueを書き換えよう。）
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(int value)
    {
        ChangedValue(value);
    }

    /// <summary>
    /// ステータスの値を足し算で変化させる（付与ステータスは除く）
    /// </summary>
    /// <param name="value"></param>
    public void AddValue(int value)
    {
        BaseStatusValue = BaseStatusValue + value;
        //ChangedValue(statusValue += value); これでいいのかあんま自信ないからダメだったら戻そう
    }

    /// <summary>
    /// 値が更新されたときに呼び出され、ステータスUIに変更を反映させる
    /// </summary>
    /// <param name="value"></param>
    public virtual void ChangedValue(int value)
    {
        if (GameManager.instance.logController.statusUpdateInfo)
            UtilityLog.CardLog(this + "の値を" + value + "に更新しました", card, UtilityLog.LogCategory.Status);
        statusValue = value;
        if (isDisplayableIcon)
        {
            observingIcon.ChangedValue(value);
        }
        else
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, "対応アイコンがありません");
        }
        //Debug.Log($"{!dontDestroyTiming },{ statusInfo.doRemoveLower0Status },{ StatusValue <= 0 },{ givedStatuses.Count == 0}");//dd
                                                                              //付与ステータスの内部データを保持するため、付与されている間はステータスは消えない。
        if (!dontDestroyTiming && statusInfo.doRemoveLower0Status && StatusValue <= 0 && givedStatuses.Count == 0)
        {
            //Debug.Log("ステータスを削除します", this);
            //Destroy(this);
            card.cardStatuses.statusesDictionary.Remove(statusInfo.statusType);
        }
    }

    /// <summary>
    /// 外部からステータスに補正をかける。もともとその補正がかかっていた場合更新する。
    /// </summary>
    /// <param name="effectNugget"></param>
    public void AddGivedStatus(Effect effect)
    {
        bool isNewStatus = false;
        //もともと同じ補正がかかっていたら追加せず値を更新して終了する
        foreach (GivedStatus givedStatus in givedStatuses)
        {
            if (givedStatus.sourceEffect == effect)
            {                
                givedStatus.givedStatusValue = effect.mainValue.GetValue(card);                
                isNewStatus = true;
                if (GameManager.instance.logController.statusUpdateInfo)
                    UtilityLog.CardLog($"{givedStatus} の値を {effect.mainValue.GetValue(card)} に変更しました", card, UtilityLog.LogCategory.Status);
                break;
            }
        }

        //その補正がかかっていなかったら追加する
        if (!isNewStatus)
        {
            GivedStatus newGivedStatus = new GivedStatus(effect,card);
            givedStatuses.Add(newGivedStatus);
            if (GameManager.instance.logController.statusUpdateInfo)
                UtilityLog.CardLog($"{newGivedStatus} を付与しました", card, UtilityLog.LogCategory.Status);
        }
        UpdateValue();
    }

    /// <summary>
    /// 付与ステータスの値を更新する
    /// </summary>
    /// <param name="effect"></param>
    public void ChangeGivedStatus(Effect effect,int changeValue)
    {
        foreach(GivedStatus givedStatus in givedStatuses)
        {
            if(givedStatus.sourceEffect == effect)
            {
                UtilityLog.CardLog("付与ステータスの値を" + changeValue + "にします", card, UtilityLog.LogCategory.Status);
                givedStatus.givedStatusValue = changeValue;
                //もともとなかったからあるべきじゃないのかもしれんけどないと動かなくね
                UpdateValue();
            }
        }
    }

    /// <summary>
    /// 1つのステータス補正を削除する
    /// </summary>
    /// <param name="effectNugget"></param>
    public void RemoveGivedStatus(Effect effect)
    {
        foreach (GivedStatus givedStatus in givedStatuses)
        {
            if (givedStatus.sourceEffect == effect)
            {
                UtilityLog.CardLog($"付与ステータス {givedStatus} を削除します", card, UtilityLog.LogCategory.Status);
                givedStatuses.Remove(givedStatus);
                break;
            }
        }
        UpdateValue();
    }

    public void ClearGivedStatus()
    {
        UtilityLog.CardLog("付与ステータスをクリアしました", card, UtilityLog.LogCategory.Status);
        givedStatuses.Clear();
        UpdateValue();
    }

    /// <summary>
    /// 耐えるや避けるなどで、ステータスが消費される
    /// </summary>
    public void DecreaseStatusValue()
    {
        if (givedStatuses.Count > 0)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, givedStatuses[0] + "の効果が1失われます");
            givedStatuses[0].decreasedValue++;
            UpdateValue();
            return;
        }
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, "素のステータスが削られました");
        AddValue(-1);
    }

    /// <summary>
    /// ステータス補正などを含めて計算し、ステータスの実数値を更新する
    /// </summary>
    public virtual void UpdateValue()
    {
        if (GameManager.instance.logController.statusUpdateInfo)
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.Status, "付与ステータスを含めて計算し、ステータスを更新");
        //付与ステータスの合計を
        int givedStatusesSum = 0;
        foreach(GivedStatus givedStatus in givedStatuses)
        {
            givedStatusesSum += givedStatus.givedStatusValue - givedStatus.decreasedValue;
        }
        
        SetValue(BaseStatusValue + givedStatusesSum);
    }

    /// <summary>
    /// 現在のステータスがデュフォルトか調べる
    /// </summary>
    /// <returns></returns>
    public bool IsDefault()
    {
        return DefaultStatusValue == StatusValue;
    }

    /// <summary>
    /// ステータスの値が処理中で条件に当てはまってもまだステータスを破壊しないでほしい処理の間をBeginとEndで囲う。バケツリレーみたいに引数渡すよりはこっちのほうがいいときもあるかも？綺麗ではないから多用はすべきでない。
    /// </summary>
    public void BeginDontDestroyTiming()
    {
        dontDestroyTiming = true;
    }

    /// <summary>
    /// ステータスの値が処理中で条件に当てはまってもまだステータスを破壊しないでほしい処理の間をBeginとEndで囲う。
    /// </summary>
    public void EndDontDestroyTiming()
    {
        dontDestroyTiming = false;
    }

    public virtual void LeftField()
    {
        
    }
}
