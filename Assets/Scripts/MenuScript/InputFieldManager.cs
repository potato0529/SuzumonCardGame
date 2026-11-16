using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour
{
    [Header("InputField")] public TMP_InputField inputField;
    [Header("Œˆ’èƒ{ƒ^ƒ“")] public Button decideButton;

    private void Update()
    {
        if(inputField.text != "")
        {
            decideButton.enabled = true;
        }
        else
        {
            decideButton.enabled = false;
            
        }
    }
}
