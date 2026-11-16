using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTarget : MonoBehaviour
{
    public bool targeted;
    private Lockon generatedLockon;

    /// <summary>
    /// このオブジェクトをロックオンする
    /// </summary>
    public void LockOnThis()
    {
        generatedLockon = Instantiate(GameManager.instance.assetsDatas.lockonPrefab);
        generatedLockon.LockonTarget(this);
    }

    /// <summary>
    /// ロックオンを外す
    /// </summary>
    public void UnLockOnThis()
    {
        Debug.Log("ロックオンを外す " + generatedLockon);
        StartCoroutine(generatedLockon.UnLockon(this));
    }
}
