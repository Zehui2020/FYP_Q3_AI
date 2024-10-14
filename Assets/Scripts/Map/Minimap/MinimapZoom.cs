using System.Collections.Generic;
using UnityEngine;

public class MinimapZoom : MonoBehaviour
{
    [SerializeField] private Transform map;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private int maxZoom;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private List<Transform> buttons;
    [SerializeField] private float minbuttonZoom;
    [SerializeField] private Vector2 halfButtonSize;

    private void Update()
    {
        float zoom = Input.mouseScrollDelta.y * scrollSensitivity;
        map.localScale += new Vector3(zoom, zoom, 0);
        map.localScale = new Vector3(Mathf.Clamp(map.localScale.x, 1, maxZoom), Mathf.Clamp(map.localScale.y, 1, maxZoom), 0);
        float percentage = 100 - (map.localScale.x - 1) / maxZoom * 100;
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].localScale = new Vector3(Mathf.Clamp(percentage / 100, minbuttonZoom, 1), Mathf.Clamp(percentage / 100, minbuttonZoom, 1), 0);
        buttonParent.localPosition = new Vector3(-buttons[0].localScale.x * halfButtonSize.x, -buttons[0].localScale.x * halfButtonSize.y, 0);
    }
}
