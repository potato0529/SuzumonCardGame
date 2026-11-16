using UnityEngine;
using UnityEngine.UI;

public class DefenceAnim : MonoBehaviour
{
    [Header("回るテキスト")] public Image defenceText;
    [Header("1fで回る角度")] public float rotateAngle;
    [Header("消滅時間")] public float destroyTime;
    [Header("大きさの変化")] public AnimationCurve scaleCurve;

    private float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;
        defenceText.GetComponent<RectTransform>().Rotate(0, 0, rotateAngle * Time.deltaTime);

        // 時間経過に応じて大きさを変化
        float scale = scaleCurve.Evaluate(currentTime / destroyTime);
        transform.localScale = Vector3.one * scale;

        if(currentTime >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
