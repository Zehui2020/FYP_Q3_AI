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
        {
            if (GameData.Instance.levelCount >= GameData.Instance.maxLevels)
                SceneLoader.Instance.LoadScene("BossLevel");
            else
                SceneLoader.Instance.LoadScene(baseSceneName + GameData.Instance.levelCount);
        }
    }
}