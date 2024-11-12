using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : PooledObject
{
    [SerializeField] private float lerpSpeed;
    [SerializeField] private List<Rigidbody2D> rbs;
    [SerializeField] private List<ShockwaveChild> shockwaves;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private AudioSource audioSource;

    private BaseStats thrower;
    private float damage;

    public override void Init()
    {
        base.Init();
    }

    public void InitShockwave(BaseStats thrower, float damage, Vector3 spawnPos)
    {
        transform.position = new Vector3(spawnPos.x, spawnPos.y + sr.size.y / 2f, spawnPos.z);
        rbs[0].velocity = Vector2.left * lerpSpeed;
        rbs[1].velocity = Vector2.right * lerpSpeed;

        foreach (ShockwaveChild shockwave in shockwaves)
        {
            shockwave.OnTriggerEnter.AddListener(OnEnterTrigger);
        }

        this.thrower = thrower;
        this.damage = damage;

        StartCoroutine(lifetimeRoutine());
    }

    private IEnumerator lifetimeRoutine()
    {
        yield return new WaitForSecondsRealtime(5);
        ReleaseShockwave();
    }

    public void OnEnterTrigger(Collider2D collision)
    {
        if (!collision.TryGetComponent<PlayerController>(out PlayerController player))
            return;

        player.TakeDamage(thrower, new BaseStats.Damage(BaseStats.Damage.DamageSource.Unparriable, damage), false, player.transform.position, DamagePopup.DamageType.Health);
    }

    public void ReleaseShockwave()
    {
        rbs[0].velocity = Vector2.zero;
        rbs[1].velocity = Vector2.zero;

        rbs[0].transform.localPosition = Vector2.zero;
        rbs[1].transform.localPosition = Vector2.zero;
       
        Release();
        gameObject.SetActive(false);
    }
}