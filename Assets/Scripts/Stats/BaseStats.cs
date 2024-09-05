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

    [Header("Base Stats")]
    public int health;
    public int maxHealth;
    public int shield;
    public int maxShield;
    public int attack;
    public float attackSpeed;
    public float shieldRegenDelay;
    public bool isImmune = false;

    [Header("Modifiers")]
    public StatModifier attackIncrease = new();

    public StatModifier critRate = new();
    public StatModifier critDamage = new();
    public StatModifier comboDamageMultipler = new();
    public StatModifier damageMultipler = new();
    public StatModifier breachedMultiplier = new();
    public StatModifier damageReduction = new();

    private Coroutine immuneRoutine;
    private Coroutine shieldRegenRoutine;

    // bool increase, isCrit
    public event System.Action<bool, bool> OnHealthChanged;
    public event System.Action<bool, bool> OnShieldChanged;
    public event System.Action<float> OnBreached;

    public virtual bool TakeDamage(float damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
    {
        if (health <= 0)
            return false;

        // Check for immunity
        if (isImmune)
        {
            DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;

            switch (immuneType)
            {
                case ImmuneType.Dodge:
                    popup.SetupPopup("Dodged!", transform.position, Color.white, new Vector2(1, 3));
                    break;
                case ImmuneType.Block:
                    popup.SetupPopup("Blocked!", transform.position, Color.white, new Vector2(1, 3));
                    break;
            }

            return false;
        }

        // Start regen shield
        if (shieldRegenRoutine != null)
            StopCoroutine(shieldRegenRoutine);
        shieldRegenRoutine = StartCoroutine(ShieldRegenRoutine());

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;

        int finalDamage = CalculateFinalDamage(damage);

        // Check for shield active
        if (shield > 0)
        {
            shield -= finalDamage;
            damageType = DamagePopup.DamageType.Shield;
            damagePopup.SetupPopup(finalDamage, closestPoint, damageType, new Vector2(1, 2));
            OnShieldChanged?.Invoke(false, isCrit);

            if (shield < 0)
            {
                damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
                damagePopup.SetupPopup("Breached!", new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Color.blue, new Vector2(0, 3));
                shield = 0;
                OnBreached?.Invoke(breachedMultiplier.GetTotalModifier());
            }

            return true;
        }

        health -= finalDamage;
        OnHealthChanged?.Invoke(false, isCrit || shield < 0);
        damageType = DamagePopup.DamageType.Health;
        if (isCrit)
            damageType = DamagePopup.DamageType.Crit;
        damagePopup.SetupPopup(finalDamage, closestPoint, damageType, new Vector2(1, 1));

        return true;
    }

    public float CalculateDamageDealt(out bool isCrit, out DamagePopup.DamageType damageType)
    {
        float critDamage = 1;
        // Offense
        // Crit Calculation
        if (Random.Range(0, 100) < critRate.GetTotalModifier())
        {
            critDamage = this.critDamage.GetTotalModifier();
            damageType = DamagePopup.DamageType.Crit;
            isCrit = true;
        }
        else
        {
            damageType = DamagePopup.DamageType.Health;
            isCrit = false;
        }

        float totalDamage = (attack + attackIncrease.GetTotalModifier()) * comboDamageMultipler.GetTotalModifier() * damageMultipler.GetTotalModifier() * critDamage;

        return totalDamage;
    }

    public int CalculateFinalDamage(float totalDamageDealt)
    {
        if (shield <= 0)
            totalDamageDealt *= breachedMultiplier.GetTotalModifier();

        totalDamageDealt *= (1 - damageReduction.GetTotalModifier());

        return Mathf.CeilToInt(totalDamageDealt);
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;

        DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
        popup.SetupPopup("+" + amount, transform.position, Color.green, new Vector2(1, 2));
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
                change = (int)(value * attack / 100);
            else if (effectType == BaseAbility.AbilityEffectType.Decrease)
                change = -(int)(value * attack / 100);
        }
        attackIncrease.AddModifier(change);

        yield return new WaitForSeconds(duration);

        attackIncrease.RemoveModifier(change);
    }

    public void OnHealthChange(float value, BaseAbility.AbilityEffectType effectType, BaseAbility.AbilityEffectValueType valueType, float duration)
    {
        StartCoroutine(HealthChangeRoutine(value, effectType, valueType, duration));
    }

    public IEnumerator HealthChangeRoutine(float value, BaseAbility.AbilityEffectType effectType, BaseAbility.AbilityEffectValueType valueType, float duration)
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

        if (change < 0)
        {
            health -= change;
            if (health <= 0)
                health = 1;

            OnHealthChanged?.Invoke(false, false);
        }
        else
        {
            health += change;
            if (health > maxHealth)
                health = maxHealth;

            OnHealthChanged?.Invoke(true, false);
        }

        yield return new WaitForSeconds(duration);

        if (duration > 0)
        {
            change = -change;
            if (change < 0)
            {
                health -= change;
                if (health <= 0)
                    health = 1;

                OnHealthChanged?.Invoke(false, false);
            }
            else
            {
                health += change;
                if (health > maxHealth)
                    health = maxHealth;

                OnHealthChanged?.Invoke(true, false);
            }
        }
    }

    private void OnDisable()
    {
        OnHealthChanged = null;
        OnShieldChanged = null;
        OnBreached = null;
    }

    private IEnumerator ShieldRegenRoutine()
    {
        yield return new WaitForSeconds(shieldRegenDelay);

        shield = maxShield;
        OnShieldChanged?.Invoke(true, false);
    }
}