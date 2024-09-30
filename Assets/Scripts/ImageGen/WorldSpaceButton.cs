using System.Collections;
using TMPro;
using UnityEngine;

public class WorldSpaceButton : MonoBehaviour
{
    [SerializeField] private TextMeshPro promptText;
    [SerializeField] private Rigidbody2D buttonRB;
    [SerializeField] private SimpleAnimation keycodeUI;

    private ComfyUIManager uiManager;

    private bool isLaunched = false;

    public void SetPrompt(string prompt, ComfyUIManager comfyUIManager)
    {
        promptText.text = prompt;
        uiManager = comfyUIManager;
    }

    public void PickupButton(Transform parent)
    {
        buttonRB.isKinematic = true;

        buttonRB.transform.SetParent(parent, false);
        buttonRB.transform.localPosition = Vector3.zero;
        buttonRB.transform.localRotation = Quaternion.identity;
        keycodeUI.Hide();
    }

    public void ReleaseButton()
    {
        buttonRB.isKinematic = false;
        buttonRB.transform.parent = null;
    }

    public void LaunchButton(Transform targetPoint)
    {
        if (isLaunched)
            return;

        isLaunched = true;
        transform.parent = null;
        buttonRB.isKinematic = true;
        Destroy(gameObject, 1f);
        transform.position = new Vector3(targetPoint.position.x, transform.position.y, 0);
        uiManager.AddPrompt(promptText.text);

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