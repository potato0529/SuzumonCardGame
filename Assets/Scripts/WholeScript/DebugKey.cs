using UnityEngine;

public class DebugKey : MonoBehaviour
{
    public static DebugKey instance;
    public bool enableDebugLog = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            enableDebugLog = true;
        }
        else
        {
            enableDebugLog = false;
        }
    }
}
