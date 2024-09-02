using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public enum ImmuneType
    {
        None,
        Dodge,
        Block
    }
    private ImmuneType immuneType;

    public int health;
    public int shield;
    public int attack;
    public int attackSpeed;
    public int critRate;
    public int critDamage;
    public bool isImmune = false;

    private Coroutine immuneRoutine;

    public void TakeDamage(float damage, int critRate, float critMultiplier, Vector3 closestPoint)
    {
        if (isImmune)
        {
            DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;

            switch (immuneType)
            {
                case ImmuneType.Dodge:
                    popup.SetupPopup("Dodged!", transform.position, Color.white);
                    break;
                case ImmuneType.Block:
                    popup.SetupPopup("Blocked!", transform.position, Color.white);
                    break;
            }

            return;
        }

        // crit calculation
        if (Random.Range(0, 100) < critRate)
        {
            damage *= critMultiplier;
            health -= (int)damage;
            DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
            damagePopup.SetupPopup((int)damage, closestPoint, DamagePopup.DamageType.Crit);
        }
        else
        {
            health -= (int)damage;
            DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
            damagePopup.SetupPopup((int)damage, closestPoint, DamagePopup.DamageType.Normal);
        }
    }

    public void Heal(int amount)
    {
        health += amount;

        DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
        popup.SetupPopup("+ " + amount, transform.position, Color.green);
    }

    public void ApplyImmune(float duration, ImmuneType immuneType)
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        immuneRoutine = StartCoroutine(ImmuneRoutine(duration));
        this.immuneType = immuneType;
    }

    private IEnumerator ImmuneRoutine(float duration)
    {
        float timer = duration;
        isImmune = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isImmune = false;
        immuneRoutine = null;
        immuneType = ImmuneType.None;
    }
}