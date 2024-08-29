using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    public int health;
    public int shield;
    public int attack;
    public int attackSpeed;
    public int critRate;
    public int critDamage;
    public bool isImmune = false;

    private Coroutine immuneRoutine;

    public void TakeDamage(int damage, Vector3 closestPoint)
    {
        health -= damage;
        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
        damagePopup.SetupPopup(damage, closestPoint, DamagePopup.DamageType.Normal);
    }

    public void Heal(int amount)
    {
        health += amount;
    }

    public void ApplyImmune(float duration)
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        immuneRoutine = StartCoroutine(ImmuneRoutine(duration));
    }

    private IEnumerator ImmuneRoutine(float duration)
    {
        float timer = 0;
        isImmune = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        isImmune = false;
        immuneRoutine = null;
    }
}