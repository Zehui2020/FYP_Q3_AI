using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private CutsceneGroup cutsceneGroup;
    private bool isPlayed = false;

    private void Start()
    {
        isPlayed = false;
    }

    public void StartCutscene()
    {
        cutsceneGroup.EnterCutscene();
        isPlayed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isPlayed)
            return;

        StartCutscene();
    }
}