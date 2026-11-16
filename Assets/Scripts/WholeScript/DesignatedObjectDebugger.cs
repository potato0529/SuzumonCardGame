using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignatedObjectDebugger : MonoBehaviour
{
    [Header("デバッグするスクリプト")] public Status debugScript;

    private void Update()
    {
        float dKey = Input.GetAxisRaw("DKey");
        if (dKey > 0)
        {
            Debug.Log(debugScript.givedStatuses.Count);
        }
    }
}
