using System.Collections;
using TMPro;
using UnityEngine;

public class WorldSpaceButton : MonoBehaviour
{
    [SerializeField] private TextMeshPro promptText;
    [SerializeField] private Rigidbody2D buttonRB;
    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private FollowParent followParent;

    private ComfyUIManager uiManager;

    private bool isLaunched = false;

    public event System.Action<string> AddButton;

    public void SetPrompt(string prompt, ComfyUIManager comfyUIManager)
    {
        promptText.text = prompt;
        uiManager = comfyUIManager;
    }

    public void PickupButton(Transform parent)
    {
        AudioManager.Instance.PlayOneShot(Sound.SoundName.GenerationBlock);
        buttonRB.isKinematic = true;

        followParent.enabled = true;
        followParent.SetFollowTarget(parent);

        buttonRB.transform.localPosition = Vector3.zero;
        buttonRB.transform.localRotation = Quaternion.identity;
        keycodeUI.Hide();
    }

    public void ReleaseButton()
    {
        buttonRB.isKinematic = false;
        buttonRB.transform.parent = null;
        followParent.enabled = false;
    }

    public void LaunchButton(Transform targetPoint)
    {
        if (isLaunched)
            return;

        AudioManager.Instance.PlayOneShot(Sound.SoundName.GenerationPipe);
        followParent.enabled = false;
        isLaunched = true;
        transform.parent = null;
        buttonRB.isKinematic = true;
        Destroy(gameObject, 1f);
        transform.position = new Vector3(targetPoint.position.x, transform.position.y, 0);

        if (uiManager != null)
            uiManager.AddPrompt(promptText.text);

        AddButton?.Invoke(promptText.text);
        StartCoroutine(LaunchRoutine(targetPoint));
    }
    private IEnumerator LaunchRoutine(Transform targetPoint)
    {
        while (true)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, Time.deltaTime * 10f);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            keycodeUI.Show();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            keycodeUI.Hide();
    }
}