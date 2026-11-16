using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableParam
{
    private int actualValue;
    private int defaultValue;
    private int baseValue;
    public List<GivedParam> givedParams = new List<GivedParam>();

    public int ActualValue
    {
        get { return actualValue; }
        set { ChangedValue(value); }
    }

    public int DefaultValue
    {
        get { return defaultValue; }
        set
        {
            defaultValue = value;
            ActualValue = defaultValue;//降臨などでデュフォルトが更新されたとき、現在のステータスも更新される。かなりまれな処理なので、現在のステータスが更新されない例外は作る必要がないと考えた。
            UpdateValue();//多分必要なはず
        }
    }

    public int BaseValue
    {
        get { return baseValue; }
        set
        {
            baseValue = value;
            UpdateValue();
        }
    }

    public void ChangedValue(int value)
    {

    }

    public void UpdateValue()
    {

    }
}
