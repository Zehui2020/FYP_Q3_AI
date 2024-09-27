//----------------------------------------------------------------------
// TextureCollision
//
// Class to handle texture-based hit detection (hit detection based on transparency)
//
// Date: 25/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class TextureCollision : MonoBehaviour
{
    // Ray required for hit detection
    public GraphicRaycaster m_raycaster;
    // Event system
    public EventSystem m_eventSystem;
    // Image to display
    public Image m_image;
    // AI system image to display
    private Texture2D m_texture;

    // Flag for hit detection
    private bool m_isTransparencyHitDetection = false;

    void Start()
    {
        // Get the texture from the sprite
        m_texture = m_image.sprite.texture;

        // Check if the texture is readable
        if (!m_texture.isReadable)
        {
            Debug.LogError("The texture is not readable");
        }
    }

    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position
            PointerEventData pointerEventData = new PointerEventData(m_eventSystem);
            pointerEventData.position = Input.mousePosition;

            // List to store the results of the raycast
            List<RaycastResult> results = new List<RaycastResult>();
            m_raycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult result in results)
            {
                // Check if the clicked object is the target Image
                if (result.gameObject == m_image.gameObject)
                {
                    Vector2 localPoint;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        m_image.rectTransform,
                        Input.mousePosition,
                        null,
                        out localPoint
                    );

                    // Convert local coordinates to pixel coordinates within the sprite
                    Rect rect = m_image.sprite.textureRect;
                    float x = (localPoint.x + m_image.rectTransform.rect.width * 0.5f) * rect.width / m_image.rectTransform.rect.width;
                    float y = (localPoint.y + m_image.rectTransform.rect.height * 0.5f) * rect.height / m_image.rectTransform.rect.height;

                    // Get the pixel color from the texture
                    x = Mathf.Clamp(x, 0, m_texture.width - 1);
                    y = Mathf.Clamp(y, 0, m_texture.height - 1);
                    Color pixelColor = m_texture.GetPixel((int)x, (int)y);

                    // Apply hit detection based on transparency
                    if (pixelColor.a > 0.1f)
                    {
                        Debug.Log("Hit an opaque part");
                        // Set to true if hit on an opaque (visible) part
                        m_isTransparencyHitDetection = true;
                    }
                    else
                    {
                        Debug.Log("Hit a transparent part");
                        // Set to false if hit on a transparent (invisible) part
                        m_isTransparencyHitDetection = false;
                    }
                }
            }
        }
    }

    // Reset the hit detection flag
    public void ResetHitDetection()
    {
        m_isTransparencyHitDetection = false;
    }

    // Get the hit detection flag
    public bool GetTransparencyHitDetectionFlag()
    {
        // Return the hit detection flag
        return m_isTransparencyHitDetection;
    }
}
