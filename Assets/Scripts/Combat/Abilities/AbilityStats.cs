using UnityEngine;

[CreateAssetMenu]
public class AbilityStats : ScriptableObject
{
    public int bloodArtsBleedChance;
    public float bloodArtsLifestealMultiplier;

    public BaseStats contagiousHazeTarget;
    public bool contagiousHazeHit;
    public int contagiousHazeStacks;

    public void ResetAbilityStats()
    {
        bloodArtsBleedChance = 0;
        bloodArtsLifestealMultiplier = 0;

        contagiousHazeTarget = null;
        contagiousHazeHit = false;
        contagiousHazeStacks = 0;
    }
}
