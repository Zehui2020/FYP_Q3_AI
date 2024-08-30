using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForceBuildLayout : MonoBehaviour
{
    [SerializeField] private List<RectTransform> layouts = new();

    private void Start()
    {
        RebuildLayout();
    }

    public void RebuildLayout()
    {
        foreach (var layout in layouts) 
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout);
        }
    }
}