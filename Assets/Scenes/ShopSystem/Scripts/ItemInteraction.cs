//----------------------------------------------------------------------
// ItemInteraction
//
// Class that manages interactions between items and the user
//
// Data: 28/8/2024
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
        // Process when item effect is not set
        if (m_itemEffect == null)
        {
            // Get the item effect
            m_itemEffect = this.GetComponent<ItemEffect>();
        }
        // Process when item purchase display class is not set
        if (m_itemPurchaseDisplay == null)
        {
            // Set the item purchase display class
            m_itemPurchaseDisplay = FindAnyObjectByType<ItemPurchaseDisplay>();
        }
    }

    // Process when pointer enters the item
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Activate scale effect
        m_itemEffect.OnMouseEnter();

        // Process when item purchase screen is displayed
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // Do not apply effect
            m_itemEffect.StopEffect();
        }
    }

    // Process when pointer exits the item
    public void OnPointerExit(PointerEventData eventData)
    {
        // Activate scale effect
        m_itemEffect.OnMouseExit();
        // Process when item purchase screen is displayed
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // Do not apply effect
            m_itemEffect.StopEffect();
        }
    }

    // Process when pointer clicks on the item
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Get the ItemDisplay of the clicked item
            ItemDisplay clickedItemDisplay = eventData.pointerPress.GetComponent<ItemDisplay>();

            if (clickedItemDisplay != null)
            {
                // Start item effect
                m_itemEffect.OnMouseEnter();

                // Display popup with clicked item's data
                m_itemPurchaseDisplay.OnItemClicked(clickedItemDisplay.GetCurrentItemData(), clickedItemDisplay.GetCurrentSpriteDictionary());
                // Process when item purchase screen is displayed
                if (m_itemPurchaseDisplay.GetPopupFlag() == true)
                {
                    // Do not apply effect
                    m_itemEffect.StopEffect();
                }
            }
        }
        else
        {
            // End item effect
            m_itemEffect.OnMouseExit();
        }
    }
}
