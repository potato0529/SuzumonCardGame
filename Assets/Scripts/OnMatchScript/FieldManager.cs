using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [Header("現在のフィールド")] public Field nowField;
    [Header("フィールド")] public List<Field> fields;

    public static FieldManager instance;

    public int NowFieldNum
    {
        get
        {
            return fields.IndexOf(nowField);
        }
    }

    void Awake()
    {
        instance = this;
    }

    public void ChangeField(int fieldNum)
    {
        nowField.transform.position = new Vector3(0, -10, 0);
        nowField = fields[fieldNum];
        nowField.transform.position = transform.position;
        Debug.Log("フィールドを変更しました");
        TriggerEffectManager.instance.HappendPlayerTriggerMatter(null, EffectNugget.InvokeTiming.ChangedField);
    }
}
