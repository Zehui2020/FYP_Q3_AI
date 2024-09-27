using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField] List<Transform> transforms;
    [SerializeField] List<GameObject> cam;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeView(1, 0);
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            ChangeView(0, 1);
        }
    }

    private void ChangeView(int i, int j)
    {
        cam[i].SetActive(true);
        cam[j].SetActive(false);

        transform.SetParent(transforms[i]);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
}
