using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// マウスのRaycastとUIに干渉する要素など周辺処理をまとめて共通化しようとしてるからこのクラスは汎用性高いから参照をGameManagerに置いておいてよさそうかな
/// </summary>
public class MouseRaycast : MonoBehaviour
{
    public List<IPointerOver> oldPointerOvered = new List<IPointerOver>();
    public string cemeteryAreaTag = "CemeteryArea";
    public string specialAreaTag = "SpecialDeckArea";
    private string deckAreaTag = "DeckArea";

    // Update is called once per frame
    void Update()
    {
        PointerEventData pointData = new PointerEventData(EventSystem.current);
        List<RaycastResult> RayResult = new List<RaycastResult>();
        pointData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointData, RayResult);
        foreach(IPointerOver oldPointerOver in oldPointerOvered)
        {
            oldPointerOver.isPointerOver = false;
        }
        oldPointerOvered = new List<IPointerOver>();

        foreach(RaycastResult result in RayResult)
        {
            IPointerOver pointerOver = result.gameObject.GetComponent<IPointerOver>();
            if(pointerOver != null)
            {
                pointerOver.isPointerOver = true;
                oldPointerOvered.Add(pointerOver);
            }
        }

        //そのうち別の処理に変えると思う
        //ワールド空間のクリック時のRaycast
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverBlockingUI())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 300);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.tag == cemeteryAreaTag)
                {
                    Cemetery cemetery = hit.collider.gameObject.GetComponent<Cemetery>();

                    //自分の墓地をクリックしたなら自分の墓地を表示し、相手の墓地をクリックしたなら相手の墓地を表示する
                    if (cemetery.player == MatchManager.instance.localPlayer)
                    {
                        cemetery.OpenCemeteryWindow();
                    }
                    else
                    {
                        cemetery.player.opponentPlayer.cemeteryScr.OpenOpponentCemeteryWindow();
                    }
                }
                else if(hit.collider.gameObject.tag == specialAreaTag)
                {
                    SpecialDeck specialDeck = hit.collider.gameObject.GetComponent<SpecialDeck>();
                    specialDeck.OpenSpecialWindow();
                }
                else if(hit.collider.gameObject.tag == deckAreaTag)
                {
                    //これだとカード貫通するからなしでもいいかも
                    //Deck deck = hit.collider.gameObject.GetComponent<Deck>();
                    //deck.OpenDeckWindow();
                }
            }
        }
        
    }

    public bool IsPointerOverBlockingUI()
    {
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        foreach (var r in results)
        {
            // 名前で判定
            if (r.gameObject.TryGetComponent<BlockWorldClick>(out _))
                return true;
        }

        return false;
    }
}
