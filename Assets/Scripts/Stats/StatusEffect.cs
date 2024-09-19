using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    [System.Serializable]
    public struct StatusType
    {
        public enum Type
        {
            Buff,
            Debuff
        }
        public enum Status
        {
            // Status Effects
            Bleed,
            Burn,
            Poison,
            Freeze,
            Static,
            BloodLoss,

            // States
            Breached,
            Frozen,
            Stunned,
            Dazed
        }

        public Type statusType;
        public Status statusEffect;

        public StatusType(Type type, Status status)
        {
            statusType = type;
            statusEffect = status;
        }
    }

    public float stackResetDuration;
    public int stackCount;
    public int stackThreshold;

    private float timer;

    public bool AddStack(int amount)
    {
        stackCount += amount;
        timer = 0;

        if (stackThreshold > 0 && stackCount >= stackThreshold)
        {
            RemoveAllStacks();
            return true;
        }

        return false;
    }

    public void RemoveStack(int amount) 
    {
        stackCount -= amount;
        if (stackCount < 0)
            stackCount = 0;
    }

    public void RemoveAllStacks()
    {
        stackCount = 0;
    }

    public void SetThreshold(int amount)
    {
        stackThreshold = amount;
    }

    public void UpdateStack()
    {
        if (stackCount <= 0 || stackResetDuration < 0)
            return;

        timer += Time.deltaTime;

        if (timer >= stackResetDuration)
            RemoveAllStacks();
    }
}