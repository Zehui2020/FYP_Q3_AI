//----------------------------------------------------------------------
// ItemInteraction
//
// Class to manage interactions between the item and the user
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
        // If the item effect is not set
        if (m_itemEffect == null)
        {
            // Get the item effect component
            m_itemEffect = this.GetComponent<ItemEffect>();
        }
        // If the item purchase display class is not set
        if (m_itemPurchaseDisplay == null)
        {
            // Set the item purchase display class
            m_itemPurchaseDisplay = FindAnyObjectByType<ItemPurchaseDisplay>();
        }
    }

    // When the pointer enters the item
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Trigger the scale effect
        m_itemEffect.OnMouseEnter();

        // If the item purchase popup is being displayed
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // Do not trigger the effect
            m_itemEffect.StopEffect();
        }
    }

    // When the pointer exits the item
    public void OnPointerExit(PointerEventData eventData)
    {
        // Trigger the scale effect
        m_itemEffect.OnMouseExit();
        // If the item purchase popup is being displayed
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // Do not trigger the effect
            m_itemEffect.StopEffect();
        }
    }

    // When the pointer clicks on the item
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Get the ItemDisplay of the clicked item
            ItemDisplay clickedItemDisplay = eventData.pointerPress.GetComponent<ItemDisplay>();

            if (clickedItemDisplay != null)
            {
                // Trigger the item effect
                m_itemEffect.OnMouseEnter();

                // Display the popup with the clicked item's data
                m_itemPurchaseDisplay.OnItemClicked(clickedItemDisplay.GetCurrentItemData(), clickedItemDisplay.GetCurrentSpriteDictionary());
                // If the item purchase popup is being displayed
                if (m_itemPurchaseDisplay.GetPopupFlag() == true)
                {
                    // Do not trigger the effect
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
