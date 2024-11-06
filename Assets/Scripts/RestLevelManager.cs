using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static PromptData;

public class RestLevelManager : MonoBehaviour
{
    [SerializeField] private List<WorldSpaceButton> buttons = new();
    [SerializeField] private PromptData setPromptData;
    [SerializeField] private WorldSpaceButton worldSpaceButtonPrefab;

    [SerializeField] private TextMeshProUGUI nextDestination;

    [SerializeField] private float minSpawnXRange;
    [SerializeField] private float maxSpawnXRange;
    [SerializeField] private float spawnY;

    public UnityEvent<string> OnRecieveButton;

    private void Start()
    {
        SpawnRestLevelButtons();
    }

    private void SpawnRestLevelButtons()
    {
        DestroyAllButtons();

        bool foundPrompt = false;

        foreach (string prompt in setPromptData.themePrompts)
        {
            if (!Utility.StringExistsInString(prompt, GameData.Instance.levelThemes) || Utility.StringExistsInString(prompt, GameData.Instance.choseThemes))
                continue;

            SpawnButton(prompt);
            foundPrompt = true;
        }

        if (!foundPrompt)
            SpawnButton("Boss");
    }

    private void SpawnButton(string prompt)
    {
        float randX = Random.Range(minSpawnXRange, maxSpawnXRange);
        WorldSpaceButton button = Instantiate(worldSpaceButtonPrefab, new Vector3(randX, spawnY, 0), Quaternion.identity);
        button.SetPrompt(prompt, null);
        button.AddButton += RecieveButton;
        buttons.Add(button);
    }

    public void RecieveButton(string prompt)
    {
        if (prompt == "Boss")
        {
            GameData.Instance.currentLevel = "Cave";
            nextDestination.text = nextDestination.text + " " + prompt;
            OnRecieveButton?.Invoke(prompt);
            DestroyAllButtons();
            return;
        }

        GameData.Instance.choseThemes += prompt;
        GameData.Instance.currentLevel = prompt;
        nextDestination.text = nextDestination.text + " " + prompt;
        OnRecieveButton?.Invoke(prompt);
        DestroyAllButtons();
    }

    public void DestroyAllButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null)
                continue;

            Destroy(buttons[i].gameObject);
        }
    }
}