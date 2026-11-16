using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Gacha : MonoBehaviour
{
    [Header("ガチャ料金")] public int gachaCost;
    [Header("ガチャ排出カード")] public List<LootCard> lootCards = new List<LootCard>();
    [Header("排出イメージ")] public Image providedCardImage;
    [Header("変換カードデータリスト")] public List<CardData> transformCardDatas = new List<CardData>();
    [Header("ルートプレハブ")] public LootCard lootCardPrefab;
    [Header("ルートカードの親")] public Transform lootCardsParent;
    public CardDataBase cardDataBase;

    /// <summary>
    /// ガチャを回す
    /// </summary>
    public void SpinGacha()
    {
        if (GameManager.instance.userData.Suzupay >= gachaCost)
        {
            GameManager.instance.userData.Suzupay -= gachaCost;
            StartCoroutine(GachaProcess());
        }
        else
        {
            Debug.Log("スズペイが足りないよ！");
        }
    }

    private IEnumerator GachaProcess()
    {
        yield return null;

        //それぞれの排出パターンが何％～何％か決める
        float probabilitySum = 0;
        foreach(LootCard lootCard in lootCards)
        {
            lootCard.lowerGetProbability = probabilitySum;
            probabilitySum += lootCard.probability;
            lootCard.upperGetProbability = probabilitySum;
        }
        //抽選し、当てはまるカードを排出する
        float randomValue = Random.Range(0, probabilitySum);

        CardData providedCard = null;
        foreach(LootCard lootCard in lootCards)
        {
            if(randomValue >=lootCard.lowerGetProbability && randomValue < lootCard.upperGetProbability)
            {
                providedCard = GameManager.instance.cardDataBase.cardDatas[lootCard.cardId];
                break;
            }
        }

        providedCardImage.sprite = providedCard.sprite;
        Debug.Log(providedCard.cardName + "をゲット！");
        GameManager.instance.userData.GetCard(providedCard);
    }

    [ContextMenu("TransformCardDatasToLootData")]
    public void TransformCardDatas()
    {
        lootCards = new List<LootCard>();
        foreach(CardData cardData in transformCardDatas)
        {
            LootCard generatedLootCard = Instantiate(lootCardPrefab);
            //generatedLootCard.cardData = cardData;
            generatedLootCard.cardId = cardDataBase.cardDatas.IndexOf(cardData);
            generatedLootCard.probability = (float)100 / transformCardDatas.Count;
            lootCards.Add(generatedLootCard);
            generatedLootCard.transform.SetParent(lootCardsParent);
            generatedLootCard.name = cardData.name + "(Loot)";
        }
    }

    [ContextMenu("SetCardDataBaseData")]
    public void SetCardDataBaseData()
    {
        
        transformCardDatas = cardDataBase.cardDatas.ToList();
    }
}
