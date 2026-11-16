using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjToEitherCanvas : ObjToUGUI
{
    private Canvas[] canvases = new Canvas[2];
    private LinkedUGUI[] linkedUGUIs;

    protected override void Awake()
    {
        base.Awake();
        canvases[0] = MatchManager.instance.canvas1;
        canvases[1] = MatchManager.instance.canvas2;
    }

    public override void GenerateUGUI()
    {
        base.GenerateUGUI();
        linkedUGUIs = new LinkedUGUI[2];//2のキャンバスに表示するために2つ入る配列

        for (byte loops = 0; loops < linkedUGUIs.Length; loops++)
        {
            linkedUGUIs[loops] = Instantiate(copySource).GetComponent<LinkedUGUI>();
            //リンクUGUIの初期設定
            InitialSetting(linkedUGUIs[loops], MatchManager.instance.players[loops]);
        }
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, $"リンクUGUIの {linkedUGUIs[0]} と {linkedUGUIs[1]} を生成しました");
    }

    /// <summary>
    /// 表示されているリンクUGUIの親をカードに戻す。(表示されなくなる)
    /// </summary>
    public override void ReleaseLink()
    {
        base.ReleaseLink();
        
        //親をObjToUGUIに戻す
        foreach(LinkedUGUI linkedUGUI in linkedUGUIs)
        {
            UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, linkedUGUI + "を削除します");
            Destroy(linkedUGUI.gameObject);
        }
        linkedUGUIs = null;
    }

    /// <summary>
    /// リンクUGUIが持つ情報を更新する
    /// </summary>
    /// <param name="value"></param>
    public override void UpdateInfo(int value)
    {
        foreach (LinkedUGUI linkedUGUI in linkedUGUIs)
        {
            (linkedUGUI.hadValueChild.GetComponent(typeof(ILikendUGUIInfo)) as ILikendUGUIInfo).UpdateInfo(value);
        }
    }
}
