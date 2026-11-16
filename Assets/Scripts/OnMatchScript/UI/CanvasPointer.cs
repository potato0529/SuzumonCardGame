using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CanvasPointer : MonoBehaviour
{
    private string miniCardInfoBGTag = "MiniCardInfoBG";
    private string listedPiledCardTag = "ListedPiledCard";
    private PointerEventData pointer;
    public bool overlapMiniCardInfoBG;
    private ListedPiledCard clickedListedPiledCard;
    // Start is called before the first frame update
    void Start()
    {
        pointer = new PointerEventData(EventSystem.current);
    }

    // Update is called once per frame
    void Update()
    {
        List<RaycastResult> results = new List<RaycastResult>();

        pointer.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointer, results);

        overlapMiniCardInfoBG = false;
        

        foreach (RaycastResult target in results)
        {
            string tag = target.gameObject.tag;
            if (tag == miniCardInfoBGTag)
            {
                overlapMiniCardInfoBG = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (results.Count > 0)
            {
                FocusedUI focusedUI = results[0].gameObject.GetComponent<FocusedUI>();

                //フォーカス中のUI以外がクリックされたら
                if (!(focusedUI && focusedUI.isFocused))
                {
                    //フォーカス中の重なったカードをアンフォーカスする
                    if (clickedListedPiledCard)
                    {
                        clickedListedPiledCard.Close();
                    }
                    clickedListedPiledCard = null;

                    //新しく重なったカードリストがクリックされたら
                    if (results[0].gameObject.tag == listedPiledCardTag)
                    {
                        clickedListedPiledCard = results[0].gameObject.GetComponent<ListedPiledCard>();
                        clickedListedPiledCard.Clicked();
                    }

                    //focusedUI.gameObject.SetActive(false);
                }
            }
        }
    }
}
