using System.Collections;
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

    public Queue<string> promptIDQueue = new();
    public Queue<string> loadingQueue = new();

    public event System.Action<bool, string> OnLoadingQueueChanged;
    private bool canDequeue = true;

    [SerializeField] private ImageLoading imageLoading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        currentLevel = "Cave";

        imageLoading.InitImageLoading();

        DontDestroyOnLoad(gameObject);
    }

    public void EnqueuePromptID(string promptID)
    {
        Debug.Log("EN Q ID: " + promptID);
        promptIDQueue.Enqueue(promptID);
    }
    public string DequeuePromptID()
    {
        Debug.Log("DE Q ID: " + promptIDQueue.Peek());
        return promptIDQueue.Dequeue();
    }

    public void EnqueueLoading(string title, bool isBackground)
    {
        Debug.Log("EN Q: " + title);

        loadingQueue.Enqueue(title);
        OnLoadingQueueChanged?.Invoke(true, title);
    }
    public void DequeueLoading()
    {
        if (!canDequeue)
            return;

        Debug.Log("DE Q: " + loadingQueue.Peek());

        canDequeue = false;
        loadingQueue.Dequeue();
        OnLoadingQueueChanged?.Invoke(false, string.Empty);
        StartCoroutine(DequeueRoutine());
    }
    private IEnumerator DequeueRoutine()
    {
        yield return new WaitForSeconds(1f);
        canDequeue = true;
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

        loadingQueue.Clear();
        itemStats.ResetStats();

        OnLoadingQueueChanged = null;
    }

    private void OnApplicationQuit()
    {
        ResetData();
    }
}