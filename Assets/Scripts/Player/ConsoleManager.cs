using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager Instance;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private List<GameObject> enemies = new();

    private bool isDeveloperMode = true;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SetDevMode(bool devMode)
    {
        isDeveloperMode = devMode;
    }

    public void SetConsole()
    {
        if (!isDeveloperMode)
            return;

        if (!gameObject.activeInHierarchy)
        {
            Time.timeScale = 0;
            gameObject.SetActive(true);
            inputField.Select();
        }
        else
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }

    public void OnInputCommand()
    {
        if (!gameObject.activeInHierarchy)
            return;

        string[] words = inputField.text.Split(' ');
        string command = words[0];

        if (command.Equals("/give"))
        {
            if (words[1].Equals("item"))
            {
                if (words[2].Equals("all"))
                    PlayerController.Instance.GiveAllItems();
                else
                    PlayerController.Instance.GiveItem(words[2], words[3]);
            }
            else if (words[1].Equals("ability"))
            {
                PlayerController.Instance.GiveAbility(words[2]);
            }
            else if (words[1].Equals("gold"))
            {
                if (int.Parse(words[2]) > 500)
                    PlayerController.Instance.AddGold(int.Parse(words[2]));
                else
                    PlayerController.Instance.SpawnGoldPickup(int.Parse(words[2]), PlayerController.Instance.transform);
            }
        }
        else if (command.Equals("/spawn"))
        {
            foreach (GameObject enemy in enemies)
            {
                Enemy enemyToSpawn = enemy.GetComponentInChildren<Enemy>();

                if (enemyToSpawn.enemyClass.Equals((Enemy.EnemyClass)System.Enum.Parse(typeof(Enemy.EnemyClass), words[1])))
                {
                    Instantiate(enemy, PlayerController.Instance.transform.position, Quaternion.identity).GetComponentInChildren<Enemy>().InitializeEnemy();
                    break;
                }
            }
        }
        else if (command.Equals("/tp"))
        {
            PlayerController.Instance.transform.position = FindObjectOfType<Door>().transform.position;
        }
        else if (command.Equals("/portal"))
        {
            PlayerController.Instance.portalController.ActivateAllPortals();
        }
        else if (command.Equals("/level"))
        {
            GameData.Instance.currentLevel = words[1];
            SceneLoader.Instance.LoadScene("Level2");
        }
        else if (command.Equals("/shop"))
        {
            PlayerController.Instance.transform.position = FindObjectOfType<Shopkeeper>().transform.position;
        }
        else if (command.Equals("/UI"))
        {
            int a = PlayerController.Instance.playerCanvas.GetComponent<CanvasGroup>().alpha == 0 ? 1 : 0;
            PlayerController.Instance.playerCanvas.GetComponent<CanvasGroup>().alpha = a;
        }
        else if (command.Equals("/kill"))
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                enemy.TakeTrueDamage(new BaseStats.Damage(10000000000));
            }
        }
        else if (command.Equals("/Q"))
        {
            SceneLoader.Instance.LoadScene("MainMenu");
            AudioManager.Instance.StopAllSounds();
        }
        //else if (command.Equals("/win"))
        //{
        //    LevelManager.Instance.LoadScene("WinScreen");
        //}
        else if (command.Equals("/health"))
        {
            PlayerController.Instance.health = int.Parse(words[1]);
        }

        SetConsole();
    }
}