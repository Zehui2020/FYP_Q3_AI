using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlatformEffector2D platform;
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

    public bool CheckCannotGrapple(Transform grappler)
    {
        if (grappler == null)
            return false;

        return grappler.position.y >= maxRopeHeight || grappler.position.y < minRopeHeight;
    }
    
    public void GrappleStart()
    {
        if (isGrappling)
            return;

        platform.colliderMask -= playerLayer;
        isGrappling = true;
    }

    public void GrappleEnd()
    {
        if (!isGrappling)
            return;

        platform.colliderMask += playerLayer;
        isGrappling = false;
    }
}