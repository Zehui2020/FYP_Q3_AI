using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceButtonController : MonoBehaviour
{
    private PromptData promptData;

    [SerializeField] private WorldSpaceButton worldSpaceButtonPrefab;
    private List<WorldSpaceButton> buttons = new();

    [SerializeField] private ComfyUIManager uiManager;
    [SerializeField] private MenuBackground menuBackground;

    [SerializeField] private float minSpawnXRange;
    [SerializeField] private float maxSpawnXRange;
    [SerializeField] private float spawnY;

    public void InitController(PromptData promptData)
    {
        this.promptData = promptData;
    }

    public void SpawnButtons(PromptData.BGPrompt.Type currentBGType)
    {
        List<string> buttonPrompts = promptData.GetButtonPromptList(currentBGType);

        foreach (string prompt in buttonPrompts)
        {
            float randX = Random.Range(minSpawnXRange, maxSpawnXRange);
            WorldSpaceButton button = Instantiate(worldSpaceButtonPrefab, new Vector3(randX, spawnY, 0), Quaternion.identity);
            button.SetPrompt(prompt, uiManager);
            buttons.Add(button);
        }
    }

    public void ResetPrompts(bool resetBG)
    {
        if (resetBG)
            menuBackground.ResetBackground();
        uiManager.ResetPrompt();
        foreach (WorldSpaceButton button in buttons)
        {
            if (button.isActiveAndEnabled)
                continue;

            float randX = Random.Range(minSpawnXRange, maxSpawnXRange);
            button.transform.position = new Vector3(randX, spawnY, 0);
            button.transform.rotation = Quaternion.identity;
            button.gameObject.SetActive(true);
        }
    }
}