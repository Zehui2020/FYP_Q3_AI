//----------------------------------------------------------------------
// ItemEffect
//
// Class to manage effects related to items
//
// Date: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [Header("Item to apply the effect to")]
    public Transform m_itemTransform;
    [Header("Scale Amount")]
    public float m_scaleAmount = 1.2f;
    [Header("Time it takes to change")]
    public float m_changeTime = 0.2f;
    [Header("Original size")]
    private Vector3 m_originalScale;

    void Start()
    {
        // Save the original size
        m_originalScale = m_itemTransform.localScale;
    }

    // When the mouse cursor enters the object
    public void OnMouseEnter()
    {
        // Stop any ongoing size changes
        StopAllCoroutines();
        // Enlarge the item
        StartCoroutine(ScaleEffect(m_originalScale, m_originalScale * m_scaleAmount));
    }

    // When the mouse leaves the item
    public void OnMouseExit()
    {
        // Stop any ongoing size changes
        StopAllCoroutines();
        // Return the item to its original size
        StartCoroutine(ScaleEffect(m_itemTransform.localScale, m_originalScale));
    }

    // Stop the item effect
    public void StopEffect()
    {
        // Return the item to its original size
        m_itemTransform.localScale = m_originalScale;

        // Stop any ongoing size changes
        StopAllCoroutines();
    }

    // Method to change the item's size
    private IEnumerator ScaleEffect(Vector3 fromScale, Vector3 toScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime < m_changeTime)
        {
            // Gradually change the scale
            m_itemTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / m_changeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Set the item's size to the target value
        m_itemTransform.localScale = toScale;
    }
}
