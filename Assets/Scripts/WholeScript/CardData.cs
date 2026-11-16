using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardData : MonoBehaviour,ICardData
{
    [Header("カードイラスト")] public Sprite sprite;
    [Header("カード名")] public string cardName;
    [Header("効果テキスト"),TextArea] public string effectText;
    [Header("カードタイプ")] public Card.CardType cardType;
    [Header("ランク")] public byte rank;
    [Header("属性")] public CardAttribute attribute;
    [Header("カテゴリ")] public List<Card.Category> categories = new List<Card.Category>();
    [Header("コストにできない")] public bool cantCost = false;//af:手札コストにもできないのか場に出た時コストにできないのか分ける必要あり
    public EffectData effectData;//効果データ（カードのメインテキストに入るもの）    
    [Header("入るデッキタイプ")] public BelongDeckType belongDeckType;
    public SpecialCardData specialCardData;
    [Header("デッキに入る枚数")]public byte deckLimit = 4;
    [Header("カードの使いやすさ")] public UsabilityType usabilityType;
    public Transform effectNuggetsParent;//効果ナゲットの親
    public int cardID;//試合中における試合上の各カードを識別するためのカードIDとは別物で、これはカードデーターベースでカードの「種類」を識別するためのID。ゲーム内で常に不変

    [HideInInspector] public CardData sourceCardData;

    public enum CardAttribute
    {
        none,
        Tami,
        fire,
        water,
        electric,
        wind,
        dark,
        food,
        Strong,
        nature,
        fighter,
        god
    }

    public enum BelongDeckType
    {
        MainDeck,
        SpecialDeck,
        Multi
    }

    public enum UsabilityType
    {
        None,
        ForBeginner,
        Middle,
        Trickey
    }

    /// <summary>
    /// このカードデータを元にゲームカードを生成する
    /// </summary>
    public Card GenerateBattleCard(Player thisOwnerPlayer)
    {
        Card generatedCard = null;
        switch (cardType)
        {
            case Card.CardType.Monster:
                switch (effectData.monsterEffectData.monsterType)
                {
                    case MonsterEffectData.MonsterType.Normal:
                    case MonsterEffectData.MonsterType.Evolution://今んとこ進化モンスターは同じプレハブでいい
                        generatedCard = Instantiate(GameManager.instance.assetsDatas.monsterPrefab).card;
                        generatedCard.monsterScr.barrier.enableBarrier = !effectData.monsterEffectData.dontHaveBarrier;
                        break;

                    
                    case MonsterEffectData.MonsterType.Advent:
                        generatedCard = Instantiate(GameManager.instance.assetsDatas.adventMonsterPrefab).monster.card;
                        generatedCard.monsterScr.adventMonster.adventNum = effectData.monsterEffectData.adventMonsterEffectData.adventNum;//多分仕様変える
                        generatedCard.monsterScr.adventMonster.adventedEffectData = Instantiate(effectData.monsterEffectData.adventMonsterEffectData.adventedEffectData);
                        //generatedCard.transform.SetParent(generatedCard.monsterScr.adventMonster.adventedEffectData.transform);//修正したけどなんで前うまくいったんだ？
                        generatedCard.monsterScr.adventMonster.adventedEffectData.transform.SetParent(generatedCard.transform);
                        break;
                }

                //アタックダメージ設定
                generatedCard.cardStatuses.SetStatusValue<AttackStatus>(StatusData.StatusType.Attack, effectData.monsterEffectData.attackDamage, true,false);
                generatedCard.monsterScr.attack.attackStatus.DefaultStatusValue = effectData.monsterEffectData.attackDamage;
                generatedCard.monsterScr.attack.turnCanAttackNum = effectData.monsterEffectData.turnCanAttackNum;
                generatedCard.monsterScr.turnCanDefenceNum = effectData.monsterEffectData.turnCanDefenceNum;
                generatedCard.monsterScr.notAttacked = effectData.monsterEffectData.notAttacked;
                Condition generatedCondition = Instantiate(effectData.monsterEffectData.attackCondition);
                generatedCondition.transform.SetParent(generatedCard.transform);
                generatedCard.monsterScr.attack.attackCondition = generatedCondition;
                //アタックコスト設定
                //generatedCard.monsterScr.attack.attackCost = DuplicateEffectNuggetData(effectData.monsterEffectData.attackCost, generatedCard);
                //可変アタックダメージ設定
                //VariableAttackDamage generatedVariableAttackDamage = Instantiate(effectData.monsterEffectData.variableAttackDamage);
                //generatedVariableAttackDamage.transform.SetParent(generatedCard.transform); //とりあえず何も分類されない子にしとく
                //generatedVariableAttackDamage.attack = generatedCard.monsterScr.attack;
                //generatedVariableAttackDamage.attack.variableAttackDamage = generatedVariableAttackDamage;
                //generatedVariableAttackDamage.gameObject.SetActive(true);

                break;

            case Card.CardType.Item:
                switch (effectData.itemEffectData.itemType)
                {
                    case ItemEffectData.ItemType.Normal:
                        generatedCard = Instantiate(GameManager.instance.assetsDatas.itemPrefab).card;
                        break;

                    case ItemEffectData.ItemType.Eternal:
                        generatedCard = Instantiate(GameManager.instance.assetsDatas.eternalPrefab).card;
                        break;

                    case ItemEffectData.ItemType.Weapon:
                        generatedCard = Instantiate(GameManager.instance.assetsDatas.weaponPrefab).card;
                        generatedCard.weapon.canEquipConditions = effectData.itemEffectData.canEquipConditionsNugget;
                        break;
                }
                
                break;
        }

        //生成されたカードのデータを表示したい場合はこのコメントアウトを外そう
        //Debug.Log(name, gameObject);
        //Debug.Log(generatedCard);
        //Debug.Log(cardType);

        generatedCard.cardData = this;
        int ObjNameNum = MatchManager.instance.GeneratedCard(generatedCard) + 1;

        generatedCard.ownerPlayer = thisOwnerPlayer;
        generatedCard.controlerPlayer = thisOwnerPlayer;
        generatedCard.sr.sprite = sprite;
        generatedCard.name = cardName + ObjNameNum;
        generatedCard.cardType = cardType;//いらんかも
        generatedCard.rank = rank;
        generatedCard.attribute = attribute;
        generatedCard.categories = categories;
        generatedCard.useRights = effectData.useRights;
        generatedCard.cantCost = cantCost;

        //必要な設定　効果ナゲット 出すコスト ターゲット条件 アタック関連 場合によってナゲットの追加
        GameObject generatedEffectNuggetsParent = Instantiate(effectData.effectNuggetsParent.gameObject);
        generatedEffectNuggetsParent.transform.SetParent(generatedCard.transform);

        for (int loops = 0; generatedEffectNuggetsParent.transform.childCount > loops; loops++)
        {
            GameObject childObj = generatedEffectNuggetsParent.transform.GetChild(loops).gameObject;
            IInEffectText childsEffectNugget = childObj.GetComponent<IInEffectText>();
            if (childsEffectNugget != null)
            {
                childsEffectNugget.card = generatedCard;
                childObj.name = "EffectNugget(" + cardName + ")" + ObjNameNum;
                //EffectNuggetの子オブジェクトにそれぞれ識別番号を付ける
                for (int i = 0; childObj.transform.childCount > i; i++)
                {
                    childObj.transform.GetChild(i).gameObject.name += ObjNameNum;

                }
                childObj.gameObject.SetActive(true);
            }
        }

        //初期ステータスを反映 があった場所        
        
        MatchManager.instance.AddGameCard(generatedCard);

        //Debug.Log($"{generatedCard} の生成が完了しました");
        return generatedCard;
    }

    [ContextMenu(nameof(AutoGenerateEffectText))]
    /// <summary>
    /// 効果テキストを自動生成し、インスペクターにセットする。
    /// </summary>
    public void AutoGenerateEffectText()
    {
        Debug.Log("テキスト自動生成ボタン");
        List<EffectNugget> effectNuggets = new List<EffectNugget>();
        for (int loops = 0; effectNuggetsParent.transform.childCount > loops; loops++)
        {
            EffectNugget childsEffectNugget = effectNuggetsParent.transform.GetChild(loops).GetComponent<EffectNugget>();
            if (childsEffectNugget)
            {
                effectNuggets.Add(childsEffectNugget);
                
            }
        }
        string text = "";
        foreach (EffectNugget effectNugget in effectNuggets)
        {
            text += effectNugget.AutoGenerateEffectNuggetText();
        }

    }

    private void OnValidate()
    {
        if (sprite)
        {
            cardName = sprite.name;
        }
    }

    /// <summary>
    /// このカードのDetailCardTypeを取得する
    /// </summary>
    /// <returns></returns>
    public TargetCardConditions.DetailCardType GetDetailCardType()
    {
        if (cardType == Card.CardType.Monster)
        {
            if (effectData.monsterEffectData.monsterType == MonsterEffectData.MonsterType.Normal)
                return TargetCardConditions.DetailCardType.NormalMonster;

            if (effectData.monsterEffectData.monsterType == MonsterEffectData.MonsterType.Evolution)
                return TargetCardConditions.DetailCardType.EvoMonster;

            if (effectData.monsterEffectData.monsterType == MonsterEffectData.MonsterType.Advent)
                return TargetCardConditions.DetailCardType.AdventMonster;
        }
        else if (cardType == Card.CardType.Item)
        {
            if (effectData.itemEffectData.itemType == ItemEffectData.ItemType.Normal)
                return TargetCardConditions.DetailCardType.NormalItem;

            if (effectData.itemEffectData.itemType == ItemEffectData.ItemType.Weapon)
                return TargetCardConditions.DetailCardType.Weapon;

            if (effectData.itemEffectData.itemType == ItemEffectData.ItemType.Eternal)
                return TargetCardConditions.DetailCardType.Eternal;

            if (effectData.itemEffectData.itemType == ItemEffectData.ItemType.Field)
                return TargetCardConditions.DetailCardType.Field;
        }
        return TargetCardConditions.DetailCardType.none;
    }

    #region interface
    public CardData GetCardData()
    {
        return this;
    }

    public void SetCardData(CardData thisCardData)
    {
        
    }
    #endregion
}
