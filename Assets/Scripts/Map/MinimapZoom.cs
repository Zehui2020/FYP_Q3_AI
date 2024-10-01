using UnityEngine;

public class MinimapZoom : MonoBehaviour
{
    [SerializeField] private Transform map;
    [SerializeField] private float scrollSensitivity;
    [SerializeField] private int maxZoom;

    private void Update()
    {
        float zoom = Input.mouseScrollDelta.y * scrollSensitivity;
        map.localScale += new Vector3(zoom, zoom, 0);
        map.localScale = new Vector3(Mathf.Clamp(map.localScale.x, 1, maxZoom), Mathf.Clamp(map.localScale.y, 1, maxZoom), 0);
    }
}
