using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityLog
{
    public enum LogCategory
    {
        none,
        EffectNuggetProcess,//効果処理全体の広い規模での処理
        Cost,//コストにかかわる処理
        Effect,//効果にかかわる処理
        Pointer,//カード選択などにかかわる処理
        ChangeCardState,//CardState変更に関する処理
        UI,//UIにかかわる処理
        Whole,//ゲーム全体の処理
        Defect,
        Network, //ネットワークの処理
        Status, //ステータスやその表示関係の処理
        OtherSystem, //その他の重要でない細かなログ
        Condition, //効果使用条件、召喚条件など様々な条件の確認処理
    }
    /// <summary>
    /// カテゴリーに応じて■マークを付ける
    /// </summary>
    /// <param name="category"></param>
    /// <param name="message"></param>
    [HideInCallstack]
    public static void CategorizedLog(LogCategory category,string message)
    {
        if (category != LogCategory.Status && category != LogCategory.UI && category != LogCategory.OtherSystem)
        {

            Debug.Log($"{CategoryToColor(category)}{message} ");
        }
    }

    public static string CategoryToColor(LogCategory category)
    {
        switch (category)
        {
            case LogCategory.none:
                return "";

            case LogCategory.EffectNuggetProcess:
                return "<color=green>■</color>";

            case LogCategory.Cost:
                return "<color=blue>■</color>";

            case LogCategory.Effect:
                return "<color=red>■</color>";

            case LogCategory.Pointer:
                return "<color=yellow>■</color>";

            case LogCategory.ChangeCardState:
                return "<color=lime>■</color>";

            case LogCategory.UI:
                return "<color=aqua>■</color>";

            case LogCategory.Whole:
                return "<color=magenta>■</color>";

            case LogCategory.Defect:
                return "<color=red>■</color>";

            case LogCategory.Network:
                return "<color=teal>■</color>";
        }
        return "";
    }

    /// <summary>
    /// 個のカードに関する処理のログを出力させるとき、カードの情報を付加してログを出す
    /// </summary>
    /// <param name="message"></param>
    /// <param name="card"></param>
    [HideInCallstack]
    public static void CardLog(string message,Card card,LogCategory logCategory)
    {
        //ログのコントローラープレイヤー表示のために自分か相手かを得る
        string playerName = card.controlerPlayer == MatchManager.instance.localPlayer ? "自分" : "相手";

        CategorizedLog(logCategory, $"{message} |名:{card.gameObject.name},ID:{MatchManager.instance.GetCardId(card)},場所:{card.cardState},Ctrlr:{playerName}");
    }

    [HideInCallstack]
    public static void CardLog(string message, Card card)
    {
        CardLog(message, card, LogCategory.none);
    }

    [HideInCallstack]
    /// <summary>
    /// デバッグ用のログ
    /// </summary>
    /// <param name="message"></param>
    public static void DebugLog(string message)
    {
        Debug.Log($"テスト {message} <color=yellow>■■■■■</color>"); 
    }
}

