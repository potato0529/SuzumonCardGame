using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
ハイパーリンクの使い方
1.このコンポーネントをどこかに置く(基本的にはハイパーリンクを持たせたいテキストのオブジェクト
2.LinkへのEvent（Enter,Exit,Clickなど)を受け取ってリンクの内容に応じて処理するクラスを作り、IHyperLinkを継承し、HyperLink.objectHasIHyperLinkにアタッチする
3.HyperLink.textにも対象テキストコンポーネントをアタッチ
4.リンクを持たせたいテキスト部分に<link=\"[設定したいリンク]\">[リンクの範囲内にしたいテキスト]</link>  をいれる
5.2で定義したクラスに、4で設定したリンクに応じた処理(ClickLink()等の引数をみて)を記述
 */
public class HyperLink : MonoBehaviour,IPointerClickHandler, IPointerEnterHandler,IPointerExitHandler
{
    public TMP_Text text;
    [Header("リンク情報を送る対象")] public GameObject objectHasIHyperLink;

    private string pointerOveredLink = ""; //前フレームと同じリンクにカーソルを合わせているかどうかを確認するため、そのフレームでカーソルを合わせたリンクを保持
    private bool isPointerOveredWholeText;
    private IHyperLink hyperLinkInterface;　//リンク情報を送る対象のオブジェクトが持っているIHyperLink(インスペクターにIHyperLinkのフィールドを表示できないのでこういう仕組みにしている）

    private void Awake()
    {
        hyperLinkInterface = objectHasIHyperLink.GetComponent<IHyperLink>();
        if(hyperLinkInterface == null)
        {
            Destroy(this);
        }
    }

    //2.「IPointerClickHandler」のルールで「OnPointerClick」メソッドはクリックすると呼ばれます。
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOveredWholeText = true;
        //IHyperLink hyperLink = objectHasIHyperLink.GetComponent<IHyperLink>();
        //if (hyperLink != null)
        //{
        //    pointerOveredLink = GetLink();
        //    hyperLink.EnterLink(pointerOveredLink);
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOveredWholeText = false;
        //IHyperLink hyperLink = objectHasIHyperLink.GetComponent<IHyperLink>();
        //if(hyperLink != null)
        //    hyperLink.ExitLink(GetLink());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IHyperLink hyperLink = GetComponent(typeof(IHyperLink)) as IHyperLink;
        if (hyperLink != null)
        {
            string link = GetLink();
            if(link != "")
                hyperLink.ClickLink(link);
        }
    }

    private void Update()
    {
        if (isPointerOveredWholeText)
        {
            string link = GetLink();
            if (link != "" && pointerOveredLink != link)
            {
                hyperLinkInterface.EnterLink(link);
                pointerOveredLink = link;
            }
            else if (link == "" && pointerOveredLink != "")
            {
                hyperLinkInterface.ExitLink(link);
                pointerOveredLink = "";
            }
        }
        else
        {
            hyperLinkInterface.ExitLink("");
            pointerOveredLink = "";
        }
    }

    public string GetLink()
    {
        Vector2 pos = Input.mousePosition;

        //4.このスクリプトのついたゲームオブジェクトから「TextMeshProUGUI」コンポーネントを取得し、text変数に代入。

        //5.text=TextMeshProUGUIコンポーネントのルートにもっとも近いCanvasオブジェクトを代入。
        Canvas canvas = text.canvas;

        //6.カメラの変数を生成して、canvasのrenderModeを三項条件演算子で評価してカメラを代入。
        //Camera camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera; //overlay確定のため省略
        Camera camera = null;

        //7.FindIntersectingLink(TMP_Text, Vector3, Camera)で指定された位置にのリンクのインデックスを返します。(存在する場合)
        int index = TMP_TextUtilities.FindIntersectingLink(text, pos, camera);

        //8.インデックスに要素が存在しない場合は-1を返すので-1では無い場合に処理を続行します。
        if (index != -1)
        {
            //9.テキストオブジェクトにリンク情報IDを取得します。
            TMP_LinkInfo linkUrlInfo = text.textInfo.linkInfo[index];

            //10.文字列urlにGetLinkIDで取得したリンクIDを文字列として返して代入。
            string url = linkUrlInfo.GetLinkID();
            
            return url;
            //11.Web ブラウザーで url の場所を開きます。
            //Application.OpenURL(url);
        }
        return "";
    }
}