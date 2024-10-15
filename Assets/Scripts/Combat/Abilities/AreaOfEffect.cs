using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] private StatusEffect.StatusType status;
    [SerializeField] private List<string> targetTag = new List<string> { "Enemy" };
    [SerializeField] private float interval = 2;
    [SerializeField] private int stackPerInterval = 1;
    [SerializeField] private float lifeTime = 10;
    [SerializeField] private GameObject particlePrefab;
    public List<ParticleVFXManager> particleVFXManager;
    private List<BaseStats> stats = new List<BaseStats>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag))
            {
                stats.Add(other.GetComponent<BaseStats>());
                StartCoroutine(StatusOverTime(other.GetComponent<BaseStats>()));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag))
            {
                stats.Remove(other.GetComponent<BaseStats>());
                StopCoroutine(StatusOverTime(other.GetComponent<BaseStats>()));
            }
        }
    }

    public void HandleStatusOverTime()
    {
        StartCoroutine(DeathRoutine());
    }

    public void InitParticles(int amount, float interval, float verticalOffset)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(particlePrefab);
            obj.transform.localScale = Vector3.one;
            obj.transform.SetParent(transform);
            particleVFXManager.Add(obj.GetComponent<ParticleVFXManager>());
            if (i > 0)
                obj.transform.localPosition = new Vector3(i * interval, verticalOffset, 0);
            else
            {
                obj.transform.localPosition = new Vector3(0, verticalOffset, 0);
                continue;
            }

            obj = Instantiate(particlePrefab);
            obj.transform.localScale = Vector3.one;
            obj.transform.SetParent(transform);
            particleVFXManager.Add(obj.GetComponent<ParticleVFXManager>());
            obj.transform.localPosition = new Vector3(i * -interval, verticalOffset, 0);
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
        StopAllCoroutines();
    }

    private IEnumerator StatusOverTime(BaseStats stat)
    {
        if (stats.Contains(stat))
            stat.ApplyStatusEffect(status, stackPerInterval);

        yield return new WaitForSeconds(interval);

        StartCoroutine(StatusOverTime(stat));
    }
}
