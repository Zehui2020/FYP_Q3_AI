using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public float currentLevel = 1;
    public float timer;
    public bool pauseTimer = false;

    public WeaponData currentWeapon;
    public List<Item> items = new();
    public List<BaseAbility> abilities = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentLevel = 1;

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
}