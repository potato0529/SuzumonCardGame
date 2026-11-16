using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnim : MonoBehaviour
{
    [Header("攻撃エフェクトの移動速度")] public float animSpeed;
    [Header("攻撃ヒットエフェクト")] public ParticleSystem attackHitParticle;
    [Header("攻撃ヒット時効果音")] public AudioSource attackSound;
    [Header("攻撃開始時効果音")] public AudioSource attackChargeSound;
    public SpriteRenderer spriteRenderer;
    public IEnumerator StartAttackAnim(SelectableTarget target,Attack attack)
    {
        attackChargeSound.PlayOneShot(attackChargeSound.clip);
        yield return new WaitForSeconds(0.66f);
        UtilityLog.CategorizedLog(UtilityLog.LogCategory.UI, "アタックアニメーションを実行します");
        Vector3 animVector = target.transform.position - transform.position;
        while (Vector3.Distance(target.transform.position,transform.position) > 1)
        {
            transform.position += animVector.normalized * 0.1f * animSpeed;
            yield return new WaitForSecondsRealtime(MatchManager.instance.matchConfig.attackAnimFps);
        }
        attackSound.PlayOneShot(attackSound.clip);
        attack.StartAttack(target);
        ParticleSystem newParticle = Instantiate(attackHitParticle);
        newParticle.transform.position = transform.position;
        newParticle.Play();
        Destroy(newParticle.gameObject, 5.0f);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
