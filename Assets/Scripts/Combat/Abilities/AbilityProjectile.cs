using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class AbilityProjectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private int targetLayer = 7;

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
}
