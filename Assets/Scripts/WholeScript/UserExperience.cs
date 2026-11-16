using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

public class UserExperience
{
    private int Rank = 0;
    private int exp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddExp(int _exp)
    {
        exp += _exp;

        var expEvent = new ExpEvent
        {
            Exp = exp
        };

        AnalyticsService.Instance.RecordEvent(expEvent);

    }
}
