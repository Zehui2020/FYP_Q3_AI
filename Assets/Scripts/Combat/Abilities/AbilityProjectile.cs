using System.Collections.Generic;
using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private int targetLayer = 7;
    [SerializeField] private GameObject particlePrefab;
    [HideInInspector] public List<BaseStats> stats = new();
    protected List<ParticleVFXManager> particleVFXManager = new();

    public void LaunchProjectile(Vector3 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == targetLayer)
        {
            OnHit(other.GetComponent<BaseStats>());
        }
    }

    protected virtual void OnHit(BaseStats target)
    {
        Destroy(gameObject);
    }

    protected virtual void InitParticles(int amount, float interval, float verticalOffset)
    {
        for (int i = 0; i < amount; i++)
        {
            if (i == 0)
            {
                InitParticle(verticalOffset);
                continue;
            }

            GameObject obj = InitParticle(verticalOffset);
            obj.GetComponent<Rigidbody2D>().velocity = new Vector3(-interval * i, 0.5f, 0);
            obj = InitParticle(verticalOffset);
            obj.GetComponent<Rigidbody2D>().velocity = new Vector3(interval * i, 0.5f, 0);
        }
    }

    private GameObject InitParticle(float verticalOffset)
    {
        GameObject obj = Instantiate(particlePrefab);
        obj.transform.localScale = Vector3.one;
        particleVFXManager.Add(obj.GetComponent<ParticleVFXManager>());
        obj.transform.SetParent(transform);
        obj.transform.localPosition = new Vector3(0, verticalOffset, 0);
        obj.transform.SetParent(null);
        obj.GetComponent<AbilityParticle>().HandleStatusOverTime();
        obj.GetComponent<AbilityParticle>().projectile = this;

        return obj;
    }
}
