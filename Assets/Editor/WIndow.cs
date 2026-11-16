using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Window : EditorWindow
{
    [MenuItem("CONTEXT/[Effect]/[ore]")]
    static void Open()
    {
        var window = GetWindow<Window>();
        window.titleContent = new GUIContent("オリジナルのウィンドウ");
    }

    private void OnGUI()
    {
        
    }
}
