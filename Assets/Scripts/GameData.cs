using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public float levelCount = 1;
    public float maxLevels = 1;

    public float timer;
    public bool pauseTimer = false;

    public ItemStats itemStats;
    public WeaponData currentWeapon;
    public List<Item> items = new();
    public List<BaseAbility> abilities = new();

    public string levelThemes;
    public string choseThemes;
    public string currentLevel;

    public Queue<string> currentlyLoadingImage = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentLevel = "Cave";

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Time.timeScale < 1 || pauseTimer)
            return;

        timer += Time.deltaTime;
    }

    public void ResetTimer()
    {
        timer = 0;
    }

    public void SavePlayerData()
    {
        PlayerController player = PlayerController.Instance;

        abilities = player.abilityController.abilities;
        items = player.itemManager.itemList;
        currentWeapon = player.combatController.wData;
    }

    public void ResetData()
    {
        levelCount = 1;
        maxLevels = 1;
        timer = 0;

        pauseTimer = false;
        currentWeapon = null;

        PlayerController.Instance.abilityController.RemoveAllAbilities();

        items.Clear();
        abilities.Clear();

        levelThemes = string.Empty;
        choseThemes = string.Empty;
        currentLevel = string.Empty;

        currentlyLoadingImage.Clear();
        itemStats.ResetStats();
    }
}