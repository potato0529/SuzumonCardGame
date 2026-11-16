using System;
using UnityEngine;

public class DebugLogCollector : MonoBehaviour
{
    public event Action<DebugLogEntry> LogReceived;

    private void Awake()
    {
        Application.logMessageReceived += OnLogReceived;
        Debug.Log("登録");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLogReceived(string condition,string stackTrace,LogType type)
    {
        LogReceived.Invoke(new DebugLogEntry(condition,stackTrace,type));
       
    }
}
