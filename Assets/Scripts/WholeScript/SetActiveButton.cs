using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveButton : MonoBehaviour
{
    public GameObject setActiveObj;
    
    public void ActivateObj()
    {
        setActiveObj.SetActive(true);
    }
}
