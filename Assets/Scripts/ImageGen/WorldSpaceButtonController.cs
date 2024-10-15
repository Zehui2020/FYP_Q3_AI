using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceButtonController : MonoBehaviour
{
    private PromptData promptData;

    [SerializeField] private WorldSpaceButton worldSpaceButtonPrefab;
    [SerializeField] private List<WorldSpaceButton> buttons = new();

    [SerializeField] private ComfyUIManager uiManager;

    [SerializeField] private float minSpawnXRange;
    [SerializeField] private float maxSpawnXRange;
    [SerializeField] private float spawnY;

    public void InitController(PromptData promptData)
    {
        this.promptData = promptData;
    }

    public void SpawnForegroundButtons()
    {
        SpawnButtons();
    }

    public void SpawnButtons()
    {
        List<string> buttonPrompts = promptData.GetButtonPromptList();

        foreach (string prompt in buttonPrompts)
        {
            float randX = Random.Range(minSpawnXRange, maxSpawnXRange);
            WorldSpaceButton button = Instantiate(worldSpaceButtonPrefab, new Vector3(randX, spawnY, 0), Quaternion.identity);
            button.SetPrompt(prompt, uiManager);
            buttons.Add(button);
        }
    }

    public void ResetPrompts()
    {
        if (uiManager.CheckAdditionalPrompts())
            return;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null)
                continue;

            Destroy(buttons[i].gameObject);
        }

        buttons.Clear();
        uiManager.ResetPrompt();
        SpawnButtons();
    }
}