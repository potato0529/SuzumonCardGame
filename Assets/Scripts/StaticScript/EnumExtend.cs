using System.Collections.Generic;
using System;
public static partial class EnumExtend
{
    /// <summary>
    /// 1つの効果の値があれば実行できる効果であるか確かめる(EffectManagerの簡略メソッドに対応するか見るためと、選択が不要であることが分かる。
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns></returns>
    public static bool NeedOnlyOneValue(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.changeField:
            case Effect.EffectTypes.handdeath://ランダムの場合 選択の場合は条件が相手の手札の効果でその場合見るとかそういう感じで
            case Effect.EffectTypes.ChangeSummonRights:
            case Effect.EffectTypes.ChangeItemRights:
                //エフェクトを全てコルーチンの演出待機式にしたらDice参戦！
                return true;
        }
        return false;
    }

    public static bool IsEffectTargetPlayer(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.Draw:
            case Effect.EffectTypes.Damage:
            case Effect.EffectTypes.healLife:
            case Effect.EffectTypes.handdeath://ランダムの場合 選択の場合は条件が相手の手札の効果でその場合見るとかそういう感じで
            case Effect.EffectTypes.SetLife:
                return true;
        }
        return false;
    }

    /// <summary>
    /// ターゲットに対して実行する効果かどうか調べる。
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns></returns>
    public static bool IsEffectTargetCard(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.Bounce:
            case Effect.EffectTypes.changeAttackDamage:
            case Effect.EffectTypes.DestroyCard:
            case Effect.EffectTypes.Noroi:
            case Effect.EffectTypes.Paralysis:
            case Effect.EffectTypes.PileCard:
            case Effect.EffectTypes.Preservation:
            case Effect.EffectTypes.protect:
            case Effect.EffectTypes.ReturnDeck:
            case Effect.EffectTypes.scapegoat:
            case Effect.EffectTypes.Search:
            case Effect.EffectTypes.Summon:
            case Effect.EffectTypes.ForcedSummon:
            case Effect.EffectTypes.SendCemetery:
            case Effect.EffectTypes.setAttackDamage:
            case Effect.EffectTypes.AddStatus:
            case Effect.EffectTypes.GiveBarrier:
            case Effect.EffectTypes.ReturnSpecialDeck:
            case Effect.EffectTypes.Poison:
            case Effect.EffectTypes.Choose:
                return true;
        }
        return false;
    }

    /// <summary>
    /// valueやターゲットなどの情報が付属しない効果かどうか調べる
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns></returns>
    public static bool IsEffectNeedNoData(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.ExchangeLife:
                return true;
        }
        return false;
    }

    public static bool IsEffectHaveValue(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.Draw:
            case Effect.EffectTypes.Damage:
            case Effect.EffectTypes.healLife:
            case Effect.EffectTypes.handdeath:
            case Effect.EffectTypes.lifeBlocker:
            case Effect.EffectTypes.changeField:
            case Effect.EffectTypes.setAttackDamage:
            case Effect.EffectTypes.changeAttackDamage:
            case Effect.EffectTypes.Paralysis:
            case Effect.EffectTypes.Preservation:
            case Effect.EffectTypes.AntiqueBurst:
            case Effect.EffectTypes.ChooseEffect:
            case Effect.EffectTypes.ReturnDeck:
            case Effect.EffectTypes.AddStatus:
            case Effect.EffectTypes.RollDice:
            case Effect.EffectTypes.ChangeSummonRights:
            case Effect.EffectTypes.ChangeItemRights:
            case Effect.EffectTypes.Poison:
            case Effect.EffectTypes.SetLife:
                return true;
        }
        return false;
    }

    public static bool IsEffectMultipleTargetCard(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.PileCard:
            case Effect.EffectTypes.Preservation:
                return true;
        }
        return false;
    }

    public static bool IsCostTargetCard(this Cost.Costs costTypes)
    {
        switch (costTypes)
        {
            case Cost.Costs.fieldMonster:
            case Cost.Costs.SendCemeteryCard:
            case Cost.Costs.DestroyFieldMonster:
            case Cost.Costs.hand:
            case Cost.Costs.Status:
            case Cost.Costs.Bounce:
            case Cost.Costs.ReturnDeck:
                return true;
        }
        return false;
    }

    public static bool IsCostHaveValue(this Cost.Costs costTypes)
    {
        switch (costTypes)
        {
            case Cost.Costs.life:
            case Cost.Costs.Status:
            case Cost.Costs.ReturnDeck:
                return true;
        }
        return false;
    }

    /// <summary>
    /// モンスターに対しその効果を付与するとそのモンスターが避ける効果を持っていれば回避できるような、ターゲットモンスターが不利益を被る可能性のあるような効果であるか確かめる
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns></returns>
    public static bool IsDisruptEffect(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.DestroyCard:
            case Effect.EffectTypes.Noroi:
            case Effect.EffectTypes.Paralysis:
            case Effect.EffectTypes.Preservation:
            case Effect.EffectTypes.SendCemetery:
            case Effect.EffectTypes.Bounce:
                return true;
        }
        return false;
    }

    public static bool OtherEffect(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.AntiqueBurst:
            case Effect.EffectTypes.ChooseEffect:
            case Effect.EffectTypes.RollDice:
                return true;
        }
        return false;
    }

    /// <summary>
    /// そのCardStateに当てはまるカード群を返す
    /// </summary>
    /// <param name="cardState"></param>
    /// <returns></returns>
    public static List<Card> GetCardsCorrespondingToState(this Card.CardState cardState)
    {
        List<Card> cards = new List<Card>();

        //※まさかのこの処理片方のプレイヤーの所有カードしか対応してなかった...!?なぜ今までうまくいっていたかなど謎が多いので両方対応でダメだったら直しましょう
        foreach (Player player in MatchManager.instance.players)
        {
            switch (cardState)
            {
                case Card.CardState.Deck:
                    cards.AddRange(player.deck.Cards);
                    break;

                case Card.CardState.Hand:
                    cards.AddRange(player.handScr.hands);
                    break;

                case Card.CardState.Field:
                    cards.AddRange(player.fieldCards.cards);
                    break;

                case Card.CardState.Cemetery:
                    cards.AddRange(player.cemeteryScr.GetCemeteryCards());
                    break;

                case Card.CardState.SpecialDeck:
                    cards.AddRange(player.specialDeck.GetSpecialDeckCards());
                    break;

                case Card.CardState.Piled:
                    List<Card> piledCards = new List<Card>();
                    foreach (Card parentLayerCard in player.fieldCards.cards)
                    {
                        foreach (Card piledCard in parentLayerCard.cardLayer.layer)
                        {
                            piledCards.Add(piledCard);
                        }
                    }
                    break;

                case Card.CardState.none:
                    cards.AddRange(player.deck.Cards);

                    cards.AddRange(player.handScr.hands);

                    cards.AddRange(player.fieldCards.cards);

                    cards.AddRange(player.cemeteryScr.GetCemeteryCards());

                    cards.AddRange(player.specialDeck.GetSpecialDeckCards());

                    break;
            }
        }
        return cards;
    }

    /// <summary>
    /// CardStateに当てはまるカード群のうちさらに、そのプレイヤーの所有するもののみを返す。
    /// </summary>
    /// <param name="cardState"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public static List<Card> GetCardsCorrespondingToState(this Card.CardState cardState, Player player)
    {
        List<Card> cards = new List<Card>();

        //※まさかのこの処理片方のプレイヤーの所有カードしか対応してなかった...!?なぜ今までうまくいっていたかなど謎が多いので両方対応でダメだったら直しましょう
        switch (cardState)
        {
            case Card.CardState.Deck:
                cards.AddRange(player.deck.Cards);
                break;

            case Card.CardState.Hand:
                cards.AddRange(player.handScr.hands);
                break;

            case Card.CardState.Field:
                cards.AddRange(player.fieldCards.cards);
                break;

            case Card.CardState.Cemetery:
                cards.AddRange(player.cemeteryScr.GetCemeteryCards());
                break;

            case Card.CardState.SpecialDeck:
                cards.AddRange(player.specialDeck.GetSpecialDeckCards());
                break;

            case Card.CardState.none:
                cards.AddRange(player.deck.Cards);

                cards.AddRange(player.handScr.hands);

                cards.AddRange(player.fieldCards.cards);

                cards.AddRange(player.cemeteryScr.GetCemeteryCards());

                cards.AddRange(player.specialDeck.GetSpecialDeckCards());

                break;
        }
        return cards;
    }

    /// <summary>
    /// サーチウィンドウでなければ選択できないカード場所
    /// </summary>
    /// <param name="cardState"></param>
    /// <returns></returns>
    public static bool IsCardStateNeedSearchWindow(this Card.CardState cardState)
    {
        switch (cardState)
        {
            case Card.CardState.Deck:
            case Card.CardState.Cemetery:
            case Card.CardState.Piled:
            case Card.CardState.SpecialDeck:
                return true;
        }
        return false;
    }

    public static Type GetStatusType(this StatusData.StatusType statusType)
    {
        switch (statusType)
        {
            case StatusData.StatusType.Attack:
                return typeof(AttackStatus);

            case StatusData.StatusType.WithStand:
                return typeof(Withstand);

            case StatusData.StatusType.Dodge:
                return typeof(Dodge);

            case StatusData.StatusType.Paralysis:
                return typeof(Paralysis);

            case StatusData.StatusType.Preserved:
                return typeof(Preserved);

            case StatusData.StatusType.Poison:
                return typeof(Poison);

            case StatusData.StatusType.Noroi:
                return typeof(Noroi);

            case StatusData.StatusType.Electricity:
            case StatusData.StatusType.Mining:
            case StatusData.StatusType.FoodBreak:
            case StatusData.StatusType.Heartful:
            case StatusData.StatusType.Energy:
            case StatusData.StatusType.BreakRush:
            case StatusData.StatusType.SnowScene:
            case StatusData.StatusType.Note:
            case StatusData.StatusType.Order:
            case StatusData.StatusType.Blood:
            case StatusData.StatusType.SpellMemory:
                return typeof(Status);
        }
        return typeof(Status);
    }

    /// <summary>
    /// 結局アイコンごとにクラス分けるなら必要になるかも
    /// </summary>
    /// <param name="statusType"></param>
    /// <returns></returns>
    public static Type GetStatusIconType(this StatusData.StatusType statusType)
    {
        switch (statusType)
        {
            case StatusData.StatusType.Attack:
                return typeof(AtkIcon);

            case StatusData.StatusType.WithStand:
                return typeof(WithstandIcon);

            case StatusData.StatusType.Dodge:
                return typeof(DodgeIcon);

            case StatusData.StatusType.Paralysis:
                return typeof(ParalysisIcon);

            case StatusData.StatusType.Preserved:
                return typeof(PreservingIcon);

            case StatusData.StatusType.Electricity:
            case StatusData.StatusType.Mining:
            case StatusData.StatusType.FoodBreak:
                return typeof(StatusIcon);
        }
        return typeof(StatusIcon);
    }

    public static List<Player> GetTargetPlayers(this TargetCardConditions.TargetOwners targetPlayers, Player userPlayer)
    {
        List<Player> targetPlayersResult = new List<Player>();
        switch (targetPlayers)
        {
            case TargetCardConditions.TargetOwners.Myself:
                targetPlayersResult.Add(userPlayer);
                break;

            case TargetCardConditions.TargetOwners.Opponent:
                targetPlayersResult.Add(userPlayer.opponentPlayer);
                break;

            case TargetCardConditions.TargetOwners.Either:
                targetPlayersResult.AddRange(MatchManager.instance.players);
                break;
        }
        return targetPlayersResult;
    }

    public static bool IsSurrondingTiming(this EffectNugget.InvokeTiming invokeTiming)
    {
        switch (invokeTiming)
        {
            case EffectNugget.InvokeTiming.TurnStart:
            case EffectNugget.InvokeTiming.TurnEnd:
            case EffectNugget.InvokeTiming.ReceivedDamage:
            case EffectNugget.InvokeTiming.HealedLife:
            case EffectNugget.InvokeTiming.ChangedField:
                return true;
        }
        return false;
    }

    public static bool IsCardTiming(this EffectNugget.InvokeTiming invokeTiming)
    {
        switch (invokeTiming)
        {
            case EffectNugget.InvokeTiming.Appear:
            case EffectNugget.InvokeTiming.Destroyed:
            case EffectNugget.InvokeTiming.SentCementery:
            case EffectNugget.InvokeTiming.BeforeAttack:
            case EffectNugget.InvokeTiming.AfterAttack:
            case EffectNugget.InvokeTiming.BeforeBeAttacked:
            case EffectNugget.InvokeTiming.AfterWasAttacked:
            case EffectNugget.InvokeTiming.BeforeAttackMonster:
            case EffectNugget.InvokeTiming.AfterAttackMonster:
            case EffectNugget.InvokeTiming.BeforeAttackPlayer:
            case EffectNugget.InvokeTiming.AfterAttackPlayer:
            case EffectNugget.InvokeTiming.LifeBlocked:
            case EffectNugget.InvokeTiming.AntiqueBurst:
            case EffectNugget.InvokeTiming.Bounced:
            case EffectNugget.InvokeTiming.BeParalysed:
            case EffectNugget.InvokeTiming.Advented:
            case EffectNugget.InvokeTiming.BeforeDestroyed:
            case EffectNugget.InvokeTiming.Withstand:
            case EffectNugget.InvokeTiming.Poisoned:
            case EffectNugget.InvokeTiming.Preserved:
            case EffectNugget.InvokeTiming.DestroyedByPoison:
            case EffectNugget.InvokeTiming.Defenced:
            case EffectNugget.InvokeTiming.Equiped:
                return true;
        }
        return false;
    }

    /// <summary>
    /// カードタイプの大まかな種類を取得する
    /// </summary>
    /// <param name="detailCardType"></param>
    /// <returns></returns>
    public static Card.CardType GetRoughCardType(this TargetCardConditions.DetailCardType detailCardType)
    {
        switch (detailCardType)
        {
            case TargetCardConditions.DetailCardType.Monster:
            case TargetCardConditions.DetailCardType.NormalMonster:
            case TargetCardConditions.DetailCardType.EvoMonster:
            case TargetCardConditions.DetailCardType.AdventMonster:
                return Card.CardType.Monster;
            case TargetCardConditions.DetailCardType.Item:
            case TargetCardConditions.DetailCardType.NormalItem:
            case TargetCardConditions.DetailCardType.Weapon:
            case TargetCardConditions.DetailCardType.Eternal:
            case TargetCardConditions.DetailCardType.ExtraItem:
            case TargetCardConditions.DetailCardType.Field:
                return Card.CardType.Item;
        }
        return Card.CardType.none;
    }

    public static bool CanInvokeEffectDuringProcess(this MatchManager.RunningProcessType runningProcessType)
    {
        switch (runningProcessType)
        {
            case MatchManager.RunningProcessType.AntiqueBurst:
            case MatchManager.RunningProcessType.PhaseProcess:
            case MatchManager.RunningProcessType.AttackWhole:
            case MatchManager.RunningProcessType.Select:
                return true;
        }
        return false;
    }

    /// <summary>
    /// 属性enumの日本語名を取得する
    /// </summary>
    /// <param name="attribute"></param>
    /// <returns></returns>
    public static string GetJapaneseNameOfAttribute(this CardData.CardAttribute attribute)
    {
        switch (attribute)
        {
            case CardData.CardAttribute.Tami:
                return "民";

            case CardData.CardAttribute.fire:
                return "火";

            case CardData.CardAttribute.water:
                return "水";

            case CardData.CardAttribute.electric:
                return "電気";

            case CardData.CardAttribute.wind:
                return "風";

            case CardData.CardAttribute.dark:
                return "魔";

            case CardData.CardAttribute.food:
                return "食";

            case CardData.CardAttribute.Strong:
                return "剛";

            case CardData.CardAttribute.nature:
                return "自然";

            case CardData.CardAttribute.fighter:
                return "戦士";

            case CardData.CardAttribute.god:
                return "神";
        }
        return "";
    }

    public static string GetJapaneseNameOfCategory(this Card.Category category)
    {
        switch (category)
        {
            case Card.Category.Potaroid:
                return "ポテロイド";

            case Card.Category.Papuppet:
                return "ペーパペット";
            case Card.Category.BlackHorn:
                return "黒ツノ";
            case Card.Category.Reload:
                return "リロード";
            case Card.Category.APK:
                return "APK";
            case Card.Category.Tami:
                return "民";
            case Card.Category.Ei:
                return "エイ";
            case Card.Category.Torque:
                return "トルク";
            case Card.Category.Universe:
                return "ユニバース";
            case Card.Category.Borgmar:
                return "ボーグマー";
            case Card.Category.Heartful:
                return "ハートフル";
            case Card.Category.MirrorCopy:
                return "鏡写";
            case Card.Category.Undermine:
                return "蝕む";
            case Card.Category.Gate:
                return "ゲート";
            case Card.Category.Cooko:
                return "cooko";
            case Card.Category.Harp:
                return "ハープ";
            case Card.Category.Shadow:
                return "シャドウ";
            case Card.Category.FlowerAppliances:
                return "家電";
            case Card.Category.ElekiChange:
                return "エレキチェンジ";
            case Card.Category.LifeBlocker:
                return "ライフブロッカー";
            case Card.Category.ScarlotForest:
                return "朱腐の森";
            case Card.Category.Ultimate:
                return "アルティメット";
            case Card.Category.Ulti_mate:
                return "アルティメイト";
            case Card.Category.WhiteDream:
                return "白夢";
            case Card.Category.Bloody:
                return "ブラッディ";
            case Card.Category.Unlife:
                return "魔導書アンライフ";
            case Card.Category.Heart:
                return "ハート";
            case Card.Category.Food:
                return "食";
            case Card.Category.HellBeast:
                return "獄夢";
            case Card.Category.Dreamer:
                return "夢見";
            case Card.Category.Cactus:
                return "サボテン";
            case Card.Category.Break:
                return "破解";

        }
        return "";
    }

    /// <summary>
    /// 詳細タイプの日本語名を返す
    /// </summary>
    /// <param name="detailCardType"></param>
    /// <returns></returns>
    public static string GetJapaneseNameOfDetailCategory(this TargetCardConditions.DetailCardType detailCardType)
    {
        switch (detailCardType)
        {
            case TargetCardConditions.DetailCardType.Monster:
                return "モンスター";
            case TargetCardConditions.DetailCardType.Item:
                return "アイテム";
            case TargetCardConditions.DetailCardType.NormalMonster:
                return "モンスター";
            case TargetCardConditions.DetailCardType.EvoMonster:
                return "進化モンスター";
            case TargetCardConditions.DetailCardType.AdventMonster:
                return "降臨モンスター";
            case TargetCardConditions.DetailCardType.NormalItem:
                return "アイテム";
            case TargetCardConditions.DetailCardType.Weapon:
                return "ウェポンアイテム";
            case TargetCardConditions.DetailCardType.Eternal:
                return "エターナルアイテム";
            case TargetCardConditions.DetailCardType.Field:
                return "フィールドアイテム";
        }
        return "";
    }

    /// <summary>
    /// 効果の名前を返す（CardPointerなどで「○○するターゲット」などを書き起こせるように）
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns></returns>
    public static string GetEffectName(this Effect.EffectTypes effectTypes)
    {
        switch (effectTypes)
        {
            case Effect.EffectTypes.Search:
                return "手札に加える";
            case Effect.EffectTypes.Summon:
            case Effect.EffectTypes.ForcedSummon:
                return "場に出す";
            case Effect.EffectTypes.DestroyCard:
                return "破壊する";
            case Effect.EffectTypes.SendCemetery:
                return "墓地に送る";
            case Effect.EffectTypes.Bounce:
                return "手札に戻す";
            case Effect.EffectTypes.ReturnDeck:
                return "デッキに戻す";
            case Effect.EffectTypes.Noroi:
                return "呪う";
            case Effect.EffectTypes.Paralysis:
                return "動けなくする";
            case Effect.EffectTypes.Preservation:
                return "保存する";
            case Effect.EffectTypes.AddStatus:
                return "ステータスを追加する";
            case Effect.EffectTypes.setAttackDamage:
            case Effect.EffectTypes.changeAttackDamage:
                return "アタックダメージを変える";
            case Effect.EffectTypes.PileCard:
                return "重ねる";
            case Effect.EffectTypes.GiveBarrier:
                return "バリアを付与する";
            case Effect.EffectTypes.ReturnSpecialDeck:
                return "スペシャルデッキに戻す";
            case Effect.EffectTypes.Poison:
                return "毒をかける";
            case Effect.EffectTypes.Choose:
                return "選ぶ";
        }
        return "";
    }

    /// <summary>
    /// コストの名前を返す（CardPointerなどで「○○するターゲット」などを書き起こせるように）
    /// </summary>
    /// <param name="effectTypes"></param>
    /// <returns></returns>
    public static string GetCostName(this Cost.Costs effectTypes)
    {
        switch (effectTypes)
        {
            case Cost.Costs.hand:
                return "手札から捨てる";
            case Cost.Costs.fieldMonster:
                return "コストとして墓地に送る";
            case Cost.Costs.Status:
                return "コストとしてステータスを変動させる";
            case Cost.Costs.Bounce:
                return "コストとして手札に戻す";
            case Cost.Costs.DestroyFieldMonster:
                return "コストとして破壊する";
            case Cost.Costs.SendCemeteryCard:
                return "コストとして墓地に送る";
        }
        return "";
    }
}
