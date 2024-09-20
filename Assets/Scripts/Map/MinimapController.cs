using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<Transform> transforms;
    [SerializeField] Camera cam;
    [SerializeField] int minimapSize;
    [SerializeField] int zoomSize;
    private int index = 0;

    private void ChangeView()
    {
        index++;
        if (index == transforms.Count)
        {
            index = 0;
            cam.orthographicSize = minimapSize;
        }
        else
        {
            cam.orthographicSize = zoomSize;
        }


        transform.SetParent(transforms[index]);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
}
