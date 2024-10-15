using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public float currentLevel = 1;
    public float timer;
    public bool pauseTimer = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

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
}