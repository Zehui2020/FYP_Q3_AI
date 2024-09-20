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
            else if (words[1].Equals("gold"))
            {
                PlayerController.Instance.gold += int.Parse(words[2]);
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