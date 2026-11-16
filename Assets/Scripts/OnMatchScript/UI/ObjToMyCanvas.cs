using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjToMyCanvas : ObjToUGUI
{
    
    [HideInInspector] public LinkedUGUI linkedUGUI;

    private Player myPlayer; //このリンクUGUIを見るプレイヤー

    protected override void Awake()
    {
        base.Awake();
        myPlayer = card.controlerPlayer;
    }

    /// <summary>
    /// このオブジェクトに設定したテクスチャとおなじテクスチャのUGUIを生成しリンクする。既にUGUIが存在した場合、座標を更新する。
    /// </summary>
    public override void GenerateUGUI()
    {
        base.GenerateUGUI();
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "UGUIを生成します");
        //既にUIが作られてないか
        if (!linkedUGUI)
        {
            linkedUGUI = Instantiate(copySource).GetComponent<LinkedUGUI>();
            //リンクUGUIの初期設定
            InitialSetting(linkedUGUI, myPlayer);

            UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, linkedUGUI + "を生成しました");

        }
        else
        {
            linkedUGUI.UpdatePosition();
        }
    }

    /// <summary>
    /// 表示されているリンクUGUIの親をカードに戻す。(表示されなくなる)
    /// </summary>
    public override void ReleaseLink()
    {
        base.ReleaseLink();
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, linkedUGUI + "を削除します");
        Destroy(linkedUGUI);
        linkedUGUI = null;
    }

    /// <summary>
    /// リンクUGUIが持つ情報を更新する
    /// </summary>
    /// <param name="value"></param>
    public override void UpdateInfo(int value)
    {       
        (linkedUGUI.hadValueChild.GetComponent(typeof(ILikendUGUIInfo)) as ILikendUGUIInfo).UpdateInfo(value);
        
    }
}
