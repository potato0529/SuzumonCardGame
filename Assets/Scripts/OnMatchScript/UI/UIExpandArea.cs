using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIExpandArea : MonoBehaviour
{
    [Header("展開するキャンバス（オブジェクトなら何でも）")] public GameObject cemeteryCanvas;//名前をexpandCanvasに変えたほうがいいs
    [Header("閉じるときに実行するメソッド")] public UnityEvent closeEvent;
    [Header("閉じる時直接非アクティブにするか")] public bool directDisable = true;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            List<RaycastResult> RayResult = new List<RaycastResult>();
            pointData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointData, RayResult);
            UIExpandArea uiExpandArea = null;
            foreach (RaycastResult result in RayResult)
            {
                 UIExpandArea resultsUIExpandArea = result.gameObject.GetComponent<UIExpandArea>();
                if (resultsUIExpandArea)
                {
                    uiExpandArea = resultsUIExpandArea;
                }
            }
            if (!uiExpandArea)
            {
                closeEvent.Invoke();
                if(directDisable)
                    cemeteryCanvas.gameObject.SetActive(false);
            }
        }
    }
}
