using System.Collections.Generic;

[System.Serializable]
public class StatModifier
{
    public float baseAmount;
    public List<float> modifiers = new();

    public float GetTotalModifier()
    {
        float totalModifier = 0;
        foreach (float modifier in modifiers)
            totalModifier += modifier;

        return baseAmount + totalModifier;
    }

    public void AddModifier(float amount)
    {
        modifiers.Add(amount);
    }

    public void RemoveModifier(float amount)
    {
        modifiers.Remove(amount);
    }

    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }

    public void ReplaceAllModifiers(float amount)
    {
        RemoveAllModifiers();
        AddModifier(amount);
    }
}