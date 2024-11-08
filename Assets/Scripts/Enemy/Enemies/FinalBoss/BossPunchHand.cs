using UnityEngine;
using DesignPatterns.ObjectPool;
using System.Collections;

public class BossPunchHand : PooledObject
{
    private FinalBossPhase2 boss;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private Animator animator;

    public void InitHand(FinalBossPhase2 attacker, Vector2 dir)
    {
        boss = attacker;
        if (dir == Vector2.right)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    public void OnDamageEventStart()
    {
        hitBox.enabled = true;
    }
    public void OnDamageEventEnd()
    {
        hitBox.enabled = false;
    }

    public void Retreat()
    {
        StartCoroutine(RetreatRoutine());
    }

    private IEnumerator RetreatRoutine()
    {
        yield return new WaitForSeconds(1f);

        animator.SetTrigger("retreat");
    }

    public void PunchEnd()
    {
        Release();
        gameObject.SetActive(false);
        boss.PunchEnd();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out PlayerController player))
            return;

        if (boss.AttackTarget(player, BaseStats.Damage.DamageSource.Normal, player.transform.position))
            player.movementController.Knockback(30f, transform.position.x < player.transform.position.x ? Vector2.right : Vector2.left);
    }
}