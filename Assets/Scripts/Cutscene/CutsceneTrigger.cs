using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private CutsceneGroup cutsceneGroup;
    private bool isPlayed = false;

    public void StartCutscene()
    {
        timeline.Play();
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