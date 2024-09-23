//----------------------------------------------------------------------
// TextFadeAndMove
//
// Class for displaying the purchase amount
//
// Data: 17/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeAndMove : MonoBehaviour
{
    [Header("Fade and Move Duration")]
    public float m_duration = 2.0f;

    [Header("Move Distance")]
    public Vector3 m_moveDistance = new Vector3(0, -50, 0);

    // Item shop UI management class
    ItemShopUIHandler m_itemShopUIHandler;

    private Vector3 m_initialPosition;

    // Copy of the purchase amount display text
    private GameObject m_itemPurchaseDisplayPrefab;

    // Current copy of the purchase amount display text
    private GameObject m_currentItemPurchaseDisplayPrefab;

    void Start()
    {
        // Handle case when item shop UI management class is not set
        if (m_itemShopUIHandler == null)
        {
            // Set the item shop UI management class
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Create a copy of the purchase amount display text
        m_itemPurchaseDisplayPrefab = m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject;
        // Save the initial color and position
        m_initialPosition = m_itemShopUIHandler.m_itemPurchaseDisplay.rectTransform.anchoredPosition;

        // Hide the item purchase amount display
        m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject.SetActive(false);
    }

    // Fade the text
    public IEnumerator FadeMoveAndResetText(string text, float price, string itemUnit)
    {
        // Instantiate a new text object
        GameObject newPurchaseDisplay = Instantiate(m_itemPurchaseDisplayPrefab, m_itemShopUIHandler.m_itemPurchaseDisplay.transform.parent);
        var textComponent = newPurchaseDisplay.GetComponent<TextMeshProUGUI>();
        textComponent.text = text + price + itemUnit;
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);
        newPurchaseDisplay.SetActive(true);

        // Fade out and move
        yield return StartCoroutine(FadeAndMoveText(1, 0, newPurchaseDisplay.transform.position, newPurchaseDisplay.transform.position + m_moveDistance, newPurchaseDisplay));

        // Wait a moment
        yield return new WaitForSeconds(0.5f);

        // Destroy the created text
        Destroy(newPurchaseDisplay);
    }

    private IEnumerator FadeAndMoveText(float startAlpha, float endAlpha, Vector3 startPosition, Vector3 endPosition, GameObject textObject)
    {
        float elapsedTime = 0f;

        while (elapsedTime < m_duration)
        {
            float t = elapsedTime / m_duration;

            // Interpolate the position of the text
            if (textObject != null)
            {
                textObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);

                // Change the transparency of the text
                Color newColor = textObject.GetComponent<TextMeshProUGUI>().color;
                newColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
                textObject.GetComponent<TextMeshProUGUI>().color = newColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the final position and transparency
        if (textObject != null)
        {
            textObject.transform.position = endPosition;
            Color finalColor = textObject.GetComponent<TextMeshProUGUI>().color;
            finalColor.a = endAlpha;
            textObject.GetComponent<TextMeshProUGUI>().color = finalColor;
        }
    }

    // Reset the position and transparency
    private void ResetTextPositionAndAlpha()
    {
        // Reset the position
        m_itemShopUIHandler.m_itemPurchaseDisplay.rectTransform.anchoredPosition = m_initialPosition;

        // Reset the transparency
        Color resetColor = m_itemShopUIHandler.m_itemPurchaseDisplay.color;
        resetColor.a = 1f;
        m_itemShopUIHandler.m_itemPurchaseDisplay.color = resetColor;

        // Hide the purchase amount display
        m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject.SetActive(false);
    }
}
