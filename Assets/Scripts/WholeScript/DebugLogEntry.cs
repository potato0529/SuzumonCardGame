using UnityEngine;

public class DebugLogEntry : MonoBehaviour
{
    public readonly string condition;
    public readonly string stackTrace;
    public readonly LogType logType;
    public DebugLogEntry(string _condition,string _stackTrace,LogType _logType)
    {
        condition = _condition;
        stackTrace = _stackTrace;
        logType = _logType;
    }
}
