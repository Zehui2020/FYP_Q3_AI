using System.Collections;
using TMPro;
using UnityEngine;

public class ShopkeeperUIManager : MonoBehaviour
{
    [SerializeField] private Canvas shopkeeperCanvas;

    //For conversing with the Shopkeeper
    [SerializeField] private TMP_InputField user_Input;
    [SerializeField] private TypewriterEffect shopkeeperOutput;

    public bool isInteracting = false;

    public void ShowUI()
    {
        shopkeeperCanvas.enabled = true;
    }

    public void HideUI()
    {
        shopkeeperCanvas.enabled = false;
    }

    public void SetShopkeeperOutput(string output)
    {
        StartCoroutine(SetOutputRoutine(output));
    }

    private IEnumerator SetOutputRoutine(string output)
    {
        while (!isInteracting)
            yield return null;

        shopkeeperOutput.gameObject.SetActive(true);
        shopkeeperOutput.ShowMessage("Constance", output);
    }

    public void OnLeaveShopkeeper()
    {
        shopkeeperOutput.gameObject.SetActive(false);
    }

    public string GetUserInput()
    {
        return user_Input.text;
    }
}