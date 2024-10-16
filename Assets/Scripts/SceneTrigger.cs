using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string triggerTag;
    [SerializeField] private string baseSceneName;
    [SerializeField] private bool activeOnStart;

    public void Start()
    {
        if (activeOnStart)
            GetComponent<Animator>().SetTrigger("activate");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(triggerTag))
            SceneLoader.Instance.LoadScene(baseSceneName + GameData.Instance.currentLevel);
    }
}