using UnityEngine;

public class ShadowPillar : Enemy
{
    [SerializeField] private Rigidbody2D crystalRB;
    [SerializeField] private Transform questPointPos;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        crystalRB.isKinematic = true;
        QuestPointer.Instance.Show(questPointPos);

        OnDieEvent += (basestats) => 
        {
            crystalRB.isKinematic = false;
            crystalRB.transform.parent = null;
            QuestPointer.Instance.Hide();

            gameObject.SetActive(false);
        };
    }

    public override bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        bool takeDamage = base.TakeDamage(attacker, damage, isCrit, closestPoint, damageType);
        return takeDamage;
    }
}