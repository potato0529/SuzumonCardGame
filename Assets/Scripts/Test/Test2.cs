using UnityEngine;

public class Test2 : MonoBehaviour
{
    public DebugLogCollector debugLogCollector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //debugLogCollector.LogReceived += OnLogReceived;
        //Debug.Log("UI登録");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + 0.01f, transform.position.y, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, 0);
    }

    public void OnLogReceived(string a)
    {
        transform.position += Vector3.up * 0.05f;
    }
}
