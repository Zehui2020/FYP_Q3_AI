using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public float currentLevel = 1;

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
}