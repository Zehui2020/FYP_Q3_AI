using UnityEngine;

public class ShadowPillar : Enemy
{
    [SerializeField] private Rigidbody2D crystalRB;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        crystalRB.isKinematic = true;
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool takeDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);

        if (health <= 0)
        {
            crystalRB.isKinematic = false;
            crystalRB.transform.parent = null;

            Destroy(gameObject);
        }

        return takeDamage;
    }
}