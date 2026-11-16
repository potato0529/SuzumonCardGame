using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueStatus : Status
{
    [Header("Type取得のためだけにインスタンス化していてこのクラス自体は使用しない")] public bool onlyForGettingType;
    protected bool isSet;
    
}
