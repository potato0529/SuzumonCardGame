using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToPos : MonoBehaviour
{
    private Vector3 movePos;
    private float speed;
    private bool active;
    public static List<MoveToPos> movings = new List<MoveToPos>();
    private Coroutine runningMTV;//実行中のMoveToVector

    /// <summary>
    /// Move()のコールバックなしオーバーロード
    /// </summary>
    /// <param name="thisMovePos"></param>
    /// <param name="thisSpeed"></param>
    public void Move(Vector3 thisMovePos, float thisSpeed)
    {
        Move(thisMovePos, thisSpeed, null);
    }

    /// <summary>
    /// 指定した座標まで移動アニメーションをする命令
    /// </summary>
    /// <param name="thisMovePos"></param>
    /// <param name="thisSpeed"></param>
    public void Move(Vector3 thisMovePos, float thisSpeed,Action action)
    {
        //移動中だった場合上書きする（破棄して新しい移動を優先する）
        if (runningMTV == null)
        {
            //runningProcess関連処理 上書き時はmovingsから見ると同じ処理が引き継がれていることになるからこの処理を行わない
            if (movings.Count == 0)
            {
                MatchManager.instance.runningProcess.Add(MatchManager.RunningProcessType.MovingCards, new RunningProcess(null));
            }
            movings.Add(this);

            runningMTV = StartCoroutine(MoveToVector(thisMovePos, thisSpeed,action));
        }
        else
        {
            StopCoroutine(runningMTV);
            runningMTV = StartCoroutine(MoveToVector(thisMovePos, thisSpeed,action));
        }
    }

    /// <summary>
    /// 指定した座標まで移動アニメーションをする(実際の移動処理)
    /// </summary>
    /// <param name="thisMovePos"></param>
    /// <param name="thisSpeed"></param>
    /// <returns></returns>
    private IEnumerator MoveToVector(Vector3 thisMovePos,float thisSpeed,Action action)
    {
        movePos = thisMovePos;
        speed = thisSpeed;
        active = true;

        //whileの条件に入る値とする
        float distance = 1;
        float customMoveSpeed = 0.4f;
        float nowMoveSpeed;
        while (distance > 0.1f && active)
        {
            distance = Vector3.Distance(transform.position, movePos);
            nowMoveSpeed = (distance / 0.5f) * customMoveSpeed;

            Vector3 moveVector = Vector3.MoveTowards(transform.position, movePos, nowMoveSpeed * 0.03f + 0.2f);
            transform.position = moveVector;
            
            yield return new WaitForSecondsRealtime(MatchManager.instance.matchConfig.cardMoveAnimFps);
        }
                   
        active = false;
        transform.position = movePos;

        movings.Remove(this);

        //この移動で全てのカードの移動処理が終了する場合、完了を示す
        if (movings.Count == 0)
        {
            MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.MovingCards);
        }

        if(action != null) action.Invoke();
        runningMTV = null;
    }

    /// <summary>
    /// warn:※現状、MoveToVectorの処理中にカードのtransformを直接変更する場合MoveToVectorが優先されてしまうため、この問題が起こる可能性がある場合、transform変更前にこのメソッドを呼ばなければならない
    /// </summary>
    public void StopMoveToVector()
    {
        if (runningMTV != null)
        {
            movings.Remove(this);

            //この移動で全てのカードの移動処理が終了する場合、完了を示す
            if (movings.Count == 0)
            {
                MatchManager.instance.CompRunningProcess(MatchManager.RunningProcessType.MovingCards);
            }

            StopCoroutine(runningMTV);
            runningMTV = null;
        }
    }
}
