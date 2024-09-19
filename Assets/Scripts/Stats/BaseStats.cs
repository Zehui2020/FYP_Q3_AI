using DesignPatterns.ObjectPool;
using System.Collections;
using UnityEngine;
using static StatusEffectManager;

public class BaseStats : MonoBehaviour
{
    public enum ImmuneType
    {
        None,
        Dodge,
        Block,
        Parry
    }
    protected ImmuneType immuneType;

    public struct Damage
    {
        // Constructor for normal damage
        public Damage(float damage)
        {
            damageSource = DamageSource.Normal;
            this.damage = damage;
            counter = 0;
        }

        public Damage(DamageSource damageSource, float damage)
        {
            this.damageSource = damageSource;
            this.damage = damage;
            counter = 0;
        }

        public enum DamageSource
        {
            Normal,
            StatusEffect,
            FrazzledWire
        }

        public DamageSource damageSource;
        public float damage;
        public int counter;
    }

    [HideInInspector] protected StatusEffectManager statusEffectManager;
    [SerializeField] public ParticleVFXManager particleVFXManager;
    [SerializeField] protected StatusEffectStats statusEffectStats;
    [SerializeField] protected ItemStats itemStats;

    [Header("Base Stats")]
    public int health;
    public int maxHealth;
    public int shield;
    public int maxShield;
    public int attack;
    public float attackSpeed;
    public float shieldRegenDelay;
    public bool isImmune = false;
    public bool isFrozen = false;

    [Header("Modifiers")]
    public StatModifier attackSpeedMultiplier = new();
    public StatModifier attackIncrease = new();
    public StatModifier critRate = new();
    public StatModifier critDamage = new();
    public StatModifier comboDamageMultipler = new();
    public StatModifier damageMultipler = new();
    public StatModifier breachDamageMultiplier = new();
    public StatModifier breachedMultiplier = new();
    public StatModifier totalDamageMultiplier = new();
    public StatModifier damageReduction = new();
    public StatModifier movementSpeedMultiplier = new();

    private Coroutine immuneRoutine;
    protected Coroutine shieldRegenRoutine;

    protected Coroutine frozenRoutine;
    protected Coroutine stunnedRoutine;
    protected Coroutine dazedRoutine;

    protected Coroutine burnRoutine;
    private Coroutine poisonRoutine;
    private float poisonTimer;

    // bool increase, isCrit
    public event System.Action<bool, bool> OnHealthChanged;
    public event System.Action<bool, bool> OnShieldChanged;
    public event System.Action<float> OnBreached;
    public event System.Action<BaseStats> OnParry;
    public event System.Action<BaseStats> OnDieEvent;

    public virtual void TakeTrueDamage(Damage damage)
    {
        if (health <= 0)
            return;

        health -= Mathf.CeilToInt(damage.damage);
        OnHealthChanged?.Invoke(false, true);

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
        damagePopup.SetupPopup(Mathf.CeilToInt(damage.damage).ToString(), transform.position, Color.red, new Vector2(1, 2));

        if (health <= 0)
        {
            OnDieEvent?.Invoke(this);
            statusEffectManager.OnDie();
            particleVFXManager.StopEverything();
        }
    }
    public virtual void TakeTrueShieldDamage(Damage damage)
    {
        shield -= Mathf.CeilToInt(damage.damage);
        OnShieldChanged?.Invoke(false, true);

        DamagePopup damagePopup;

        if (shield <= 0)
        {
            damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
            damagePopup.SetupPopup("Breached!", new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Color.blue, new Vector2(0, 3));
            shield = 0;
            OnBreached?.Invoke(breachedMultiplier.GetTotalModifier());
        }

        if (shieldRegenRoutine != null)
            StopCoroutine(shieldRegenRoutine);
        shieldRegenRoutine = StartCoroutine(ShieldRegenRoutine());

        damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
        damagePopup.SetupPopup(Mathf.CeilToInt(damage.damage).ToString(), transform.position, Color.blue, new Vector2(1, 2));
    }
    public virtual bool TakeDamage(BaseStats attacker, Damage damage, bool isCrit, Vector3 closestPoint, DamagePopup.DamageType damageType)
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
                case ImmuneType.Parry:
                    OnParry?.Invoke(attacker);
                    popup.SetupPopup("Parried!", transform.position, Color.white, new Vector2(1, 3));
                    break;
            }

            return false;
        }

        // Start regen shield
        if (shieldRegenRoutine != null)
            StopCoroutine(shieldRegenRoutine);
        shieldRegenRoutine = StartCoroutine(ShieldRegenRoutine());

        DamagePopup damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;

        int finalDamage = CalculateFinalDamage(attacker, damage.damage);

        // Check for shield active
        if (shield > 0)
        {
            shield -= finalDamage;
            damageType = DamagePopup.DamageType.Shield;
            damagePopup.SetupPopup(finalDamage, closestPoint, damageType, new Vector2(1, 2));
            OnShieldChanged?.Invoke(false, isCrit);

            if (shield <= 0)
            {
                damagePopup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;
                damagePopup.SetupPopup("Breached!", new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Color.blue, new Vector2(0, 3));
                shield = 0;
                OnBreached?.Invoke(breachedMultiplier.GetTotalModifier());
            }

            return true;
        }

        health -= finalDamage;
        OnHealthChanged?.Invoke(false, isCrit || shield <= 0);
        damageType = DamagePopup.DamageType.Health;
        if (isCrit)
            damageType = DamagePopup.DamageType.Crit;
        damagePopup.SetupPopup(finalDamage, closestPoint, damageType, new Vector2(1, 1));

        if (health <= 0)
        {
            OnDieEvent?.Invoke(this);
            statusEffectManager.OnDie();
            particleVFXManager.StopEverything();
        }

        return true;
    }
    public virtual float CalculateDamageDealt(BaseStats target, out bool isCrit, out DamagePopup.DamageType damageType)
    {
        float finalCritRate = critRate.GetTotalModifier();
        float finalCritDamage = 1;
        // Offense
        // Crit Calculation

        if (target.isFrozen)
        {
            finalCritRate += target.statusEffectStats.frozenCritRate;
            finalCritDamage = critDamage.GetTotalModifier() + target.statusEffectStats.frozenCritDmg;
        }

        if (Random.Range(0, 100) < finalCritRate)
        {
            finalCritDamage = critDamage.GetTotalModifier();
            damageType = DamagePopup.DamageType.Crit;
            isCrit = true;
        }
        else
        {
            damageType = DamagePopup.DamageType.Health;
            isCrit = false;
        }

        float totalDamage = (attack + attackIncrease.GetTotalModifier()) * comboDamageMultipler.GetTotalModifier() * damageMultipler.GetTotalModifier() * finalCritDamage;

        return totalDamage;
    }
    public virtual Damage CalculateProccDamageDealt(BaseStats target, Damage damage, out bool isCrit, out DamagePopup.DamageType damageType)
    {
        float finalCritRate = critRate.GetTotalModifier();
        float finalCritDamage = 1;
        // Offense
        // Crit Calculation

        if (target.isFrozen)
        {
            finalCritRate += target.statusEffectStats.frozenCritRate;
            finalCritDamage = critDamage.GetTotalModifier() + target.statusEffectStats.frozenCritDmg;
        }

        if (Random.Range(0, 100) < finalCritRate)
        {
            finalCritDamage = critDamage.GetTotalModifier();
            damageType = DamagePopup.DamageType.Crit;
            isCrit = true;
        }
        else
        {
            damageType = DamagePopup.DamageType.Health;
            isCrit = false;
        }

        float totalDamage = damage.damage * damageMultipler.GetTotalModifier() * finalCritDamage;

        return new Damage(totalDamage);
    }
    public int CalculateFinalDamage(BaseStats attacker, float totalDamageDealt)
    {
        if (shield <= 0)
            totalDamageDealt *= breachedMultiplier.GetTotalModifier();
        else
            totalDamageDealt *= attacker.breachDamageMultiplier.GetTotalModifier();

        totalDamageDealt *= (1 - damageReduction.GetTotalModifier());
        totalDamageDealt *= attacker.totalDamageMultiplier.GetTotalModifier();

        return Mathf.CeilToInt(totalDamageDealt);
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;

        OnHealthChanged?.Invoke(true, false);
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
    public void RemoveImmune()
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        isImmune = false;
        immuneRoutine = null;
        immuneType = ImmuneType.None;
    }

    private IEnumerator ImmuneRoutine(float duration)
    {
        float timer = duration;
        isImmune = true;

        while (timer > 0)
        {
            if (health <= 0)
                yield break;

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

    public void Cleanse(StatusEffect.StatusType.Type type)
    {
        statusEffectManager.Cleanse(type);
    }

    public virtual void OnCleanse(StatusEffect.StatusType.Status status)
    {
    }

    private void OnDisable()
    {
        OnHealthChanged = null;
        OnShieldChanged = null;
        OnBreached = null;
        OnDieEvent = null;
        OnParry = null;
    }

    private IEnumerator ShieldRegenRoutine()
    {
        yield return new WaitForSeconds(shieldRegenDelay);

        shield = maxShield;
        OnShieldChanged?.Invoke(true, false);
        statusEffectManager.RemoveEffectUI(StatusEffect.StatusType.Status.Breached);
    }

    protected void InvokeOnShieldChanged(bool shieldRestored, bool isCrit)
    {
        OnShieldChanged?.Invoke(shieldRestored, isCrit);
    }

    public virtual void TriggerStatusState(StatusEffect.StatusType.Status state)
    {
        if (this == null)
            return;

        DamagePopup popup = ObjectPool.Instance.GetPooledObject("DamagePopup", true) as DamagePopup;

        switch (state)
        {
            case StatusEffect.StatusType.Status.BloodLoss:
                TakeTrueDamage(new Damage(Damage.DamageSource.StatusEffect, maxHealth * statusEffectStats.bloodLossDamage));
                popup.SetupPopup("Blood Loss!", transform.position, Color.red, new Vector2(0, 3));
                break;
            case StatusEffect.StatusType.Status.Frozen:
                if (frozenRoutine != null)
                    return;
                popup.SetupPopup("Frozen!", transform.position, Color.blue, new Vector2(0, 3));
                frozenRoutine = StartCoroutine(FrozenRoutine());
                break;
            case StatusEffect.StatusType.Status.Dazed:
                if (dazedRoutine != null)
                    return;

                popup.SetupPopup("Dazed!", transform.position, Color.yellow, new Vector2(0, 3));
                dazedRoutine = StartCoroutine(DazedRoutine());
                break;
            case StatusEffect.StatusType.Status.Stunned:
                if (stunnedRoutine != null)
                    return;

                popup.SetupPopup("Stunned!", transform.position, Color.yellow, new Vector2(0, 3));
                stunnedRoutine = StartCoroutine(StunnedRoutine());
                break;
        }
    }

    public virtual void TriggerStatusEffect(StatusEffect.StatusType statusEffect, int amount)
    {
        switch (statusEffect.statusEffect)
        {
            case StatusEffect.StatusType.Status.Burn:
                for (int i = 0; i < amount; i++)
                    burnRoutine = StartCoroutine(BurnRoutine());
                break;
            case StatusEffect.StatusType.Status.Poison:
                poisonTimer = statusEffectStats.poisonDuration;
                if (poisonRoutine == null)
                    poisonRoutine = StartCoroutine(PoisonRoutine());
                break;
        }
    }

    public void ApplyStatusEffect(StatusEffect.StatusType statusEffect, int amount)
    {
        if (health > 0)
            statusEffectManager.ApplyStatusEffect(statusEffect, amount);
    }

    public virtual IEnumerator FrozenRoutine()
    {
        yield return null;
    }

    public virtual IEnumerator StunnedRoutine()
    {
        yield return null;
    }

    public virtual IEnumerator DazedRoutine()
    {
        yield return null;
    }

    public virtual IEnumerator BurnRoutine()
    {
        particleVFXManager.OnBurning(statusEffectManager.burnStacks.stackCount);

        yield return new WaitForSeconds(0.5f);

        int count = 0;

        while (count < statusEffectStats.burnsPerStack)
        {
            if (health <= 0 || statusEffectManager.burnStacks.stackCount <= 0)
                break;

            count++;

            if (shield > 0)
                TakeTrueShieldDamage(new Damage(Damage.DamageSource.StatusEffect, Mathf.CeilToInt(maxShield * statusEffectStats.burnShieldDamage)));
            else
                TakeTrueDamage(new Damage(Damage.DamageSource.StatusEffect, Mathf.CeilToInt(statusEffectStats.burnHealthDamage)));

            yield return new WaitForSeconds(statusEffectStats.burnInterval);
        }

        statusEffectManager.ReduceEffectStack(StatusEffect.StatusType.Status.Burn, 1);

        if (statusEffectManager.burnStacks.stackCount <= 0)
            particleVFXManager.StopBurning();
        else
            particleVFXManager.OnBurning(statusEffectManager.burnStacks.stackCount);
    }

    public virtual IEnumerator PoisonRoutine()
    {
        particleVFXManager.OnPoison();

        yield return new WaitForSeconds(0.5f);

        while (poisonTimer > 0)
        {
            if (health <= 0 || statusEffectManager.poisonStacks.stackCount <= 0)
                break;

            TakeTrueDamage(new Damage(Damage.DamageSource.StatusEffect, maxHealth * (statusEffectStats.basePoisonHealthDamage + (statusEffectStats.stackPoisonHealthDamage * statusEffectManager.poisonStacks.stackCount))));
            poisonTimer -= statusEffectStats.poisonInterval;

            if (poisonTimer > 0)
                yield return new WaitForSeconds(statusEffectStats.poisonInterval);
        }

        poisonTimer = 0;
        statusEffectManager.RemoveEffectUI(StatusEffect.StatusType.Status.Poison);
        poisonRoutine = null;
        particleVFXManager.StopPoison();
    }
}