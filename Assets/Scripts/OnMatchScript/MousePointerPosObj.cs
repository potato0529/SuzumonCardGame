using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointerPosObj : MonoBehaviour
{
    private bool inCardArea = true;
    private bool overlapOtherCard = false;
    [HideInInspector] private bool canSetCardArea;
    
    private string cardAreaTag = "CardArea";
    private string cardTag = "Card";

    void Update()
    {
        if(inCardArea && !overlapOtherCard)
        {
            canSetCardArea = true;
        }
        else
        {
            canSetCardArea = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == cardAreaTag)
        {
            inCardArea = true;
        }
        
        if (other.tag == cardTag && !other.gameObject.GetComponent<Card>().caughtByMouse)
        {
            overlapOtherCard = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == cardAreaTag)
        {
            inCardArea = false;
        }
        
        if (other.tag == cardTag && !other.gameObject.GetComponent<Card>().caughtByMouse)
        {
            overlapOtherCard = false;
        }
    }
}
