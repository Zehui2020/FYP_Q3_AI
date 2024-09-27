//----------------------------------------------------------------------
// ItemInteraction
//
// Class to manage interaction between items and the user
//
// Date: 28/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine.EventSystems;
using UnityEngine;

public class ItemInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Item effect
    private ItemEffect m_itemEffect;
    // Item purchase display class
    public ItemPurchaseDisplay m_itemPurchaseDisplay;

    void Start()
    {
        // Handle case where the item effect is not set
        if (m_itemEffect == null)
        {
            // Retrieve the item effect
            m_itemEffect = this.GetComponent<ItemEffect>();
        }
        // Handle case where the item purchase display class is not set
        if (m_itemPurchaseDisplay == null)
        {
            // Set the item purchase display class
            m_itemPurchaseDisplay = FindAnyObjectByType<ItemPurchaseDisplay>();
        }
    }

    // Process when the pointer enters the item
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Trigger the scale effect
        m_itemEffect.OnMouseEnter();

        // Handle case where the item purchase screen is displayed
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // Stop the effect
            m_itemEffect.StopEffect();
        }
    }

    // Process when the pointer exits the item
    public void OnPointerExit(PointerEventData eventData)
    {
        // Trigger the scale effect
        m_itemEffect.OnMouseExit();

        // Handle case where the item purchase screen is displayed
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // Stop the effect
            m_itemEffect.StopEffect();
        }
    }

    // Process when the pointer clicks on the item
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Get the ItemDisplay of the clicked item
            ItemDisplay clickedItemDisplay = eventData.pointerPress.GetComponent<ItemDisplay>();

            if (clickedItemDisplay != null)
            {
                // Start the item effect
                m_itemEffect.OnMouseEnter();

                // Display popup with the data of the clicked item
                m_itemPurchaseDisplay.OnItemClicked(clickedItemDisplay.GetCurrentItemData(), clickedItemDisplay.GetCurrentSpriteDictionary());

                // Handle case where the item purchase screen is displayed
                if (m_itemPurchaseDisplay.GetPopupFlag() == true)
                {
                    // Stop the effect
                    m_itemEffect.StopEffect();
                }
            }
        }
        else
        {
            // End the item effect
            m_itemEffect.OnMouseExit();
        }
    }
}
