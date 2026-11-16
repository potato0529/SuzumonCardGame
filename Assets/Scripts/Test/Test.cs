using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
//using Oculus.Interaction;
//using Oculus.Interaction.InterfaceSupport;

public class Test:MonoBehaviour
{
    [FormerlySerializedAs("detailCardType")]public TargetCardConditions.DetailCardType detailCardType;
    public TMP_Text effectText;
    public string originalText;

    [SerializeField, InterfaceType(typeof(ITest))]
    private Object _pointable;
    private ITest Pointable => _pointable as ITest;

    public void AAA()
    {
        
        
    }

    private void Awake()
    {
        //effectText.text = GameManager.instance.uniqueTextManager.ConvertDescriptedText(originalText);
        //Debug.Log(GameManager.instance.uniqueTextManager.ConvertDescriptedText(originalText));]
    }
}
