using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class CustomButtonAC : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler
{
    public Action onClickCallback;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        onClickCallback?.Invoke();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        
    }
}
