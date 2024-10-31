using DesignPatterns.ObjectPool;
using Unity.VisualScripting;
using UnityEngine;

public class BossLaser : PooledObject
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer warningLR;

    public override void Init()
    {
        base.Init();
        warningLR.enabled = true;
        lineRenderer.enabled = false;
    }

    public void SetupLaser(Vector3 startPos, Vector3 endPos)
    {
        warningLR.SetPosition(0, startPos);
        warningLR.SetPosition(1, endPos);
    }

    public void UpdateLaser(Vector3 startPos, Vector3 endPos)
    {
        warningLR.enabled = false;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    public void ReleaseLaser()
    {
        Release();
        gameObject.SetActive(false);
        warningLR.enabled = true;
        lineRenderer.enabled = false;
    }
}