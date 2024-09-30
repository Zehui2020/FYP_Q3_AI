using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private string sceneName;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(triggerTag))
            SceneLoader.Instance.LoadScene(sceneName);
    }
}