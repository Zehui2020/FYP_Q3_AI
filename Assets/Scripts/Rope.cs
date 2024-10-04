using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<PlatformEffector2D> platforms;
    [SerializeField] private LayerMask playerLayer;

    private bool isGrappling = false;
    private float maxRopeHeight;
    private float minRopeHeight;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxRopeHeight = transform.position.y + transform.localScale.y / 2f;
        minRopeHeight = transform.position.y - transform.localScale.y / 2f;
    }

    public bool CheckCannotGrappleUp(Transform grappler)
    {
        if (grappler == null)
            return false;

        return grappler.position.y - 0.5f >= maxRopeHeight;
    }

    public bool CheckCannotGrappleDown(Transform grappler)
    {
        if (grappler == null)
            return false;

        return grappler.position.y + 0.5f < minRopeHeight;
    }
    
    public void GrappleStart()
    {
        if (isGrappling)
            return;
        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i] != null)
            {
                platforms[i].colliderMask -= playerLayer;
            }
        }
        isGrappling = true;
    }

    public void GrappleEnd()
    {
        if (!isGrappling)
            return;
        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i] != null)
            {
                platforms[i].colliderMask += playerLayer;
            }
        }
        isGrappling = false;
    }
}