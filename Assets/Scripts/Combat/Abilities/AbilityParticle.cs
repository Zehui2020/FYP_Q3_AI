using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityParticle : MonoBehaviour
{
    [SerializeField] private StatusEffect.StatusType status;
    [SerializeField] private List<string> targetTag = new List<string> { "Enemy" };
    [SerializeField] private float interval = 2;
    [SerializeField] private int stackPerInterval = 1;
    [SerializeField] private float lifeTime = 10;
    public AbilityProjectile projectile;
    private bool isActivated = false;
    [SerializeField] private LayerMask groundLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActivated && Utility.CheckLayer(other.gameObject, groundLayer))
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag) && !projectile.stats.Contains(other.GetComponent<BaseStats>()))
            {
                BaseStats target = other.GetComponent<BaseStats>();
                projectile.stats.Add(target);
                StartCoroutine(StatusOverTime(target));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag) && !projectile.stats.Contains(other.GetComponent<BaseStats>()))
            {
                projectile.stats.Add(other.GetComponent<BaseStats>());
                StartCoroutine(StatusOverTime(other.GetComponent<BaseStats>()));
            }
        }
    }

    public void HandleStatusOverTime()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        isActivated = true;
        yield return new WaitForSeconds(lifeTime);
        if (GetComponent<ParticleVFXManager>() != null)
            GetComponent<ParticleVFXManager>().StopEverything();
        Destroy(gameObject, 1);
        StopAllCoroutines();
    }

    private IEnumerator StatusOverTime(BaseStats stat)
    {
        if (projectile.stats.Contains(stat))
            stat.ApplyStatusEffect(status, stackPerInterval);

        yield return new WaitForSeconds(interval);

        projectile.stats.Remove(stat);
    }
}
