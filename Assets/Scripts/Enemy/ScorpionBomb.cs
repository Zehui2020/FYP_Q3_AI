using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class ScorpionBomb : PooledObject
{
    private Rigidbody2D bombRB;
    private float lifetime;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;

    private Scorpion thrower;
    private Vector2 targetPos;

    public void InitScorpionBomb(Scorpion thrower, Vector2 targetPos, float lifetime, Vector2 direction)
    {
        bombRB = GetComponent<Rigidbody2D>();
        this.lifetime = lifetime;
        this.thrower = thrower;
        this.targetPos = targetPos;
    }

    public IEnumerator ExplodeRoutine()
    {
        yield return new WaitForSeconds(lifetime);

        PlayerController player = Physics2D.OverlapCircle(transform.position, radius, playerLayer).GetComponent<PlayerController>();

        if (player != null)
        {
            float damage = thrower.CalculateDamageDealt(player, BaseStats.Damage.DamageSource.Normal, out bool crit, out DamagePopup.DamageType damageType);
            player.TakeDamage(thrower, new BaseStats.Damage(damage), crit, player.transform.position, damageType);
        }

        Release();
        gameObject.SetActive(false);
    }

    private void Update()
    {

    }
}