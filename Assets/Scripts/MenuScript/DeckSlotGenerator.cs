using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSlotGenerator : MonoBehaviour
{
    [Header("キャンバス")] public Canvas canvas;
    [Header("スロットプレハブ")] public DeckSlot slotPrefab;
    [Header("横列のスロット表示限界数")] public int slotsWidth;
    [Header("生成するスロット数")] public int generateSlotsNum;
    [Header("スロットの間隔（横）")] public float slotColumnDistance;
    [Header("スロットの間隔（縦）")] public float slotRowDistance;
    [Header("生成されたスロット")] public List<DeckSlot> deckBoxSlots = new List<DeckSlot>();
    [Header("デッキイメージプレハブ")] public DeckBoxImage deckBoxImagePrefab;
    [Header("生成されたデッキイメージ"),SerializeField] private List<DeckBoxImage> deckBoxImages = new List<DeckBoxImage>();

    #region Memo
    //このクラスのすべてのn番目系の値で、現実においての1番目は0なので、こんがらがらないように認識を統一すべし
    #endregion

    private void Awake()
    {
        GenerateSlots();
    }

    /// <summary>
    /// カードリストのスロットを生成する
    /// </summary>
    public void GenerateSlots()
    {
        Debug.Log("スロットを生成します");
        //次にスロットを生成する座標
        Vector2 slotPos = Vector2.zero;

        //何列目？
        int columnsNum = 0;
        //何行目？
        int rowsNum = 0;
        for (int loops = 0; loops < generateSlotsNum; loops++)
        {
            //次のスロットの設定
            columnsNum = loops % slotsWidth;
            rowsNum = loops / slotsWidth;

            slotPos = new Vector2(slotColumnDistance * columnsNum, slotRowDistance * -rowsNum);
            //Debug.Log($"スロット生成 座標:{slotPos} 桁/行 {columnsNum}/{rowsNum}");

            //スロットを生成し、設定を反映させる
            DeckSlot generatedSlot = Instantiate(slotPrefab.gameObject).GetComponent<DeckSlot>();
            generatedSlot.transform.SetParent(canvas.transform, false);
            generatedSlot.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition + slotPos;
            generatedSlot.transform.SetParent(this.transform);
            //ジェネレーターの座標に対して加算する
            
            deckBoxSlots.Add(generatedSlot);
        }
    }

    /// <summary>
    /// スロットをすべて削除する
    /// </summary>
    public void RemoveDeckImages()
    {
        foreach(DeckBoxImage deckBoxImage in GetDeckBoxImages())
        {
            Destroy(deckBoxImage.gameObject);
            Debug.Log("削除");
        }
        ResetDeckBoxImages();
    }

    /// <summary>
    /// 当てはまるデッキをスロットに当てはめて並べる
    /// </summary>
    public void EnumerateDecks()
    {
        Debug.Log("デッキイメージをスロットに割り当てます");
        for (int loops = 0; loops < deckBoxSlots.Count; loops++)
        {
            if (loops < GameManager.instance.userData.GetDeckBoxDatas().Count)
            {
                DeckBoxData deckData = GameManager.instance.userData.GetDeckBoxDatas()[loops];
                Debug.Log($"{loops} ,{deckBoxSlots[loops]},{deckData}");
                DeckBoxImage generatedDeckImage = Instantiate(deckBoxImagePrefab.gameObject).GetComponent<DeckBoxImage>();
                generatedDeckImage.deckBoxData = deckData;
                generatedDeckImage.deckNameText.text = deckData.deckName;
                generatedDeckImage.name = deckData.deckName;
                GetDeckBoxImages().Add(generatedDeckImage);
                deckBoxSlots[loops].AssignCardToSlot(generatedDeckImage);
            }
        }
    }

    public void ReloadList()
    {
        RemoveDeckImages();
        EnumerateDecks();
    }

    public List<DeckBoxImage> GetDeckBoxImages()
    {
        //Debug.Log("DeckBoxImage");
        return deckBoxImages;
    }

    public void ResetDeckBoxImages()
    {
        //Debug.Log("DeckBoxImageをリセット");
        deckBoxImages = new List<DeckBoxImage>();
    }
}
