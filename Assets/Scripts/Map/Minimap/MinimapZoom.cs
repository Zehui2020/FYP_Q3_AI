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
    private Vector2 startScale;

    private void Start()
    {
        startScale = map.transform.localScale;
    }

    private void Update()
    {
        float zoom = Input.mouseScrollDelta.y * scrollSensitivity;
        map.localScale += new Vector3(zoom * startScale.x, zoom * startScale.y, 0);
        map.localScale = new Vector3(Mathf.Clamp(map.localScale.x, startScale.x, maxZoom * startScale.x), Mathf.Clamp(map.localScale.y, startScale.y, maxZoom * startScale.y), 0);
        float percentage = 100 - (map.localScale.x - 1) / maxZoom * 100;
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].localScale = new Vector3(Mathf.Clamp(percentage / 100, minbuttonZoom, 1), Mathf.Clamp(percentage / 100, minbuttonZoom, 1), 0);
        buttonParent.localPosition = new Vector3(-buttons[0].localScale.x * halfButtonSize.x, -buttons[0].localScale.x * halfButtonSize.y, 0);
    }
}
