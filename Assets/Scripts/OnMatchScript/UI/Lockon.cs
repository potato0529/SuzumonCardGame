using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lockon : MonoBehaviour
{
    public SpriteRenderer sr;
    [Header("最大サイズ")] public float maxSize;
    [Header("最小サイズ")] public float minimumSize;
    [Header("縮小速度")] public float shrinkSpeed;
    [Header("フェードアウト速度(現象α値/f)")] public float fadeoutRemoveOpacityParFrame;

    private bool startLockon;
    
    void Update()
    {
        if (startLockon)
        {
            if(transform.localScale.x >= minimumSize)
            {
                transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed);
            }
            else
            {
                transform.localScale = new Vector3(minimumSize, minimumSize, minimumSize);
                startLockon = false;
            }
        }
    }

    /// <summary>
    /// 対象をロックオンされた（ターゲットにされた）状態とする
    /// </summary>
    /// <param name="lockonCard">対象のカード</param>
    public void LockonTarget(SelectableTarget lockonTarget)
    {
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI,lockonTarget.name + "をロックオン");
        lockonTarget.targeted = true;
        transform.position = lockonTarget.transform.position;
        transform.localScale = new Vector3(maxSize, maxSize, maxSize);
        startLockon = true;
    }

    public IEnumerator UnLockon(SelectableTarget selectableTarget)
    {
        selectableTarget.targeted = false;
        
        while(sr.color.a > 0)
        {
            sr.color -= new Color(0, 0, 0, fadeoutRemoveOpacityParFrame);
            yield return null;
        }

        Destroy(gameObject);
    }
}
