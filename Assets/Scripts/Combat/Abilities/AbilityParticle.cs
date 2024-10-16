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
    private int count = 0;
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(gameObject.tag) && isActivated)
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            count++;
        }

        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag) && !projectile.stats.Contains(other.GetComponent<BaseStats>()))
            {
                projectile.stats.Add(other.GetComponent<BaseStats>());
                StartCoroutine(StatusOverTime(other.GetComponent<BaseStats>()));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag(gameObject.tag) && isActivated)
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag) && !projectile.stats.Contains(other.GetComponent<BaseStats>()))
            {
                projectile.stats.Add(other.GetComponent<BaseStats>());
                StartCoroutine(StatusOverTime(other.GetComponent<BaseStats>()));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(gameObject.tag))
        {
            count--;
        }
        if (count <= 0)
            GetComponent<Rigidbody2D>().isKinematic = false;
    }

    public void HandleStatusOverTime()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        isActivated = true;
        yield return new WaitForSeconds(lifeTime);
        GetComponent<ParticleVFXManager>().StopBurning();
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
