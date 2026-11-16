using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGCardGenerator : MonoBehaviour
{
    [Header("ƒJ[ƒh–ÍŒ^")] public GameObject cardModelPrefab;
    private CardDataBase cardDataBase;
    private float interval;
    private void Start()
    {
        cardDataBase = GameManager.instance.cardDataBase;
    }

    private void Update()
    {
        interval += Time.deltaTime;
        if(interval >= 3.0f)
        {
            interval = 0;
            GameObject generatedCard = Instantiate(cardModelPrefab);

        }
    }
}
