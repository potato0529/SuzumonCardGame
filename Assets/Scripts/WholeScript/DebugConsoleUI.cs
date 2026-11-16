using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class DebugConsoleUI : MonoBehaviour
{
    public DebugLogCollector debugLogCollector;
    public ScrollView scrollView;

    [Header("ログリスト")] public List<DebugLogEntry> debugLogEntries = new List<DebugLogEntry>();
    private void Start()
    {
        debugLogCollector.LogReceived += OnLogReceived;
        Debug.Log("UI登録");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLogReceived(DebugLogEntry debugLogEntry)
    {


        if (debugLogEntries.Count < 100)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, 0);

            debugLogEntries.Add(debugLogEntry);
        }
    }
}
