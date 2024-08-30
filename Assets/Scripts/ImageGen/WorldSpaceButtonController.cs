using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceButtonController : MonoBehaviour
{
    [SerializeField] private List<string> prompts = new();
    private List<WorldSpaceButton> buttons = new();
    [SerializeField] private WorldSpaceButton worldSpaceButtonPrefab;
    [SerializeField] private ComfyPromptCtr promptCtr;
    [SerializeField] private MenuBackground menuBackground;

    [SerializeField] private float minSpawnXRange;
    [SerializeField] private float maxSpawnXRange;
    [SerializeField] private float spawnY;

    private void Start()
    {
        SpawnButtons();
    }

    public void SpawnButtons()
    {
        foreach (string prompt in prompts)
        {
            float randX = Random.Range(minSpawnXRange, maxSpawnXRange);
            WorldSpaceButton button = Instantiate(worldSpaceButtonPrefab, new Vector3(randX, spawnY, 0), Quaternion.identity);
            button.SetPrompt(prompt, promptCtr);
            buttons.Add(button);
        }
    }

    public void ResetPrompts(bool resetBG)
    {
        if (resetBG)
            menuBackground.ResetBackground();
        promptCtr.ResetPrompts();
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