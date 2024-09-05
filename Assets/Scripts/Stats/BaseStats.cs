using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEditor.Experimental.GraphView;
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
    public int maxHealth;
    public int shield;
    public int attack;
    public int baseAttack;
    public float attackSpeed;
    public int critRate;
    public float critDamage;
    public bool isImmune = false;

    private Coroutine immuneRoutine;

    public virtual void TakeDamage(float damage, int critRate, float critMultiplier, Vector3 closestPoint)
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
        DamagePopup.DamageType isCrit;
        if (Random.Range(0, 100) < critRate)
        {
            damage *= critMultiplier;
            isCrit = DamagePopup.DamageType.Crit;
        }
        else
        {
            isCrit = DamagePopup.DamageType.Normal;
        }

        health -= (int)damage;
        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
        damagePopup.SetupPopup((int)damage, closestPoint, isCrit);
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

    public IEnumerator AttackChangeRoutine(float value, BaseAbility.AbilityEffectType effectType, BaseAbility.AbilityEffectValueType valueType, float duration)
    {
        int change = 0;
        if (valueType == BaseAbility.AbilityEffectValueType.Flat)
        {
            if (effectType == BaseAbility.AbilityEffectType.Increase)
                change = (int)value;
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                change = -(int)value;
        }
        else if (valueType == BaseAbility.AbilityEffectValueType.Percentage)
        {
            if (effectType == BaseAbility.AbilityEffectType.Increase)
                change = (int)(value * baseAttack / 100);
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                change = -(int)(value * baseAttack / 100);
        }
        attack += change;

        yield return new WaitForSeconds(duration);

        attack -= change;
    }

    public void OnHealthChange(float value, BaseAbility.AbilityEffectType effectType, BaseAbility.AbilityEffectValueType valueType, float duration)
    {
        StartCoroutine(HealthChangeRoutine(value, effectType, valueType, duration));
    }

    public IEnumerator HealthChangeRoutine(float value, BaseAbility.AbilityEffectType effectType, BaseAbility.AbilityEffectValueType valueType,  float duration)
    {
        int change = 0;
        if (valueType == BaseAbility.AbilityEffectValueType.Flat)
        {
            if (effectType == BaseAbility.AbilityEffectType.Increase)
                change = (int)value;
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                change = -(int)value;
        }
        else if (valueType == BaseAbility.AbilityEffectValueType.Percentage)
        {
            if (effectType == BaseAbility.AbilityEffectType.Increase)
                change = (int)(value * maxHealth / 100);
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                change = -(int)(value * maxHealth / 100);
        }
        int temp = health + change;
        temp = Mathf.Clamp(temp, 0, maxHealth);
        change = temp - health;
        if (change < 0)
        {
            TakeDamage(-change, 0, 0, transform.position);
        }
        else
        {
            health += change;
            DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
            popup.SetupPopup("+ " + change, transform.position, Color.green);
        }

        yield return new WaitForSeconds(duration);

        if (duration > 0)
        {
            change = -change;
            if (change < 0)
            {
                TakeDamage(-change, 0, 0, transform.position);
            }
            else
            {
                health += change;
                DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
                popup.SetupPopup("+ " + change, transform.position, Color.green);
            }
        }
    }
}