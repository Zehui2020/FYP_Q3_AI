using UnityEngine;

[CreateAssetMenu]
public class AbilityStats : ScriptableObject
{
    public int bloodArtsBleedChance;
    public float bloodArtsLifestealMultiplier;

    public int rabidExecutionStacks;

    public void ResetAbilityStats()
    {
        bloodArtsBleedChance = 0;
        bloodArtsLifestealMultiplier = 0;

        rabidExecutionStacks = 0;
    }
}