using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] private StatusEffect.StatusType status;
    [SerializeField] private List<string> targetTag = new List<string> { "Enemy" };
    [SerializeField] private float interval = 2;
    [SerializeField] private float lifeTime = 10;
    [SerializeField] private List<BaseStats> stats = new List<BaseStats>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (string tag in targetTag)
        {
            if (other.CompareTag(tag))
            {
                stats.Add(other.GetComponent<BaseStats>());
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
            }
        }
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    public IEnumerator StatusOverTime()
    {
        foreach (BaseStats stat in stats)
        {
            stat.ApplyStatusEffect(status, 1);
            Debug.Log("Status Applied");
        }
        yield return new WaitForSeconds(interval);

        StartCoroutine(StatusOverTime());
    }
}
