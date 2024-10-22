using UnityEngine;

[CreateAssetMenu]
public class AbilityStats : ScriptableObject
{
    public int bloodArtsBleedChance;
    public float bloodArtsLifestealMultiplier;

    public int contagiousHazeStacks;

    public void ResetAbilityStats()
    {
        bloodArtsBleedChance = 0;
        bloodArtsLifestealMultiplier = 0;

        contagiousHazeStacks = 0;
    }
}