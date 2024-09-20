using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<Transform> transforms;
    [SerializeField] Camera cam;
    [SerializeField] int minimapSize;
    [SerializeField] int zoomSize;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeView(1);
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            ChangeView(0);
        }
    }

    private void ChangeView(int i)
    {
        if (i == 0)
        {
            cam.orthographicSize = minimapSize;
        }
        else
        {
            cam.orthographicSize = zoomSize;
        }

        transform.SetParent(transforms[i]);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
}
