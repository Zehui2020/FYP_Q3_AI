using UnityEngine;

public class Crystal : BaseStats
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Vector2 leftDir;
    [SerializeField] private Vector2 rightDir;
    [SerializeField] private float hitForce;

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        LaunchCrystal();
        return base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);
    }

    private void LaunchCrystal()
    {
        Vector2 dir = transform.position.x < PlayerController.Instance.transform.position.x ? leftDir : rightDir;
        rb.AddForce(dir * hitForce, ForceMode2D.Impulse);
    }
}