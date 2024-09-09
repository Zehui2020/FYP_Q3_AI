using UnityEngine;

[System.Serializable]
public class StatusEffect
{
    public enum StatusType
    {
        Bleed,
        Burn,
        Poison,
        Freeze,
        Static
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