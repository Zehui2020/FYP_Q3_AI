using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    [SerializeField] private LayerMask fogLayer;
    [SerializeField] private float radius = 50f;
    [SerializeField] private float checkRadius = 100f;
    [SerializeField] private float checkInterval = 0.5f;

    private float radiusSqr {  get { return radius; } }

    private List<GameObject> fogOfWarPlanes;
    private List<Mesh> mesh = new List<Mesh>();
    private List<Vector3[]> vertices = new List<Vector3[]>();
    private List<Color[]> colors = new List<Color[]>();

    private void UpdateFog()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, PlayerController.Instance.transform.position - transform.position, fogLayer);
        if (hit)
        {
            for (int i = 0; i < fogOfWarPlanes.Count; i++)
            {
                if (checkRadius < Vector3.Distance(fogOfWarPlanes[i].transform.position, PlayerController.Instance.transform.position))
                    continue;
                for (int j = 0; j < vertices[i].Length; j++)
                {
                    Vector3 v = fogOfWarPlanes[i].transform.TransformPoint(vertices[i][j]);
                    float dist = Vector3.SqrMagnitude(v - new Vector3(hit.point.x, hit.point.y, 0));
                    if (dist < radiusSqr)
                    {
                        float alpha = Mathf.Min(colors[i][j].a, dist / radiusSqr);
                        colors[i][j].a = alpha;
                    }
                }
                UpdateColor(i);
            }
        }
    }

    private IEnumerator CheckFog()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            UpdateFog();
        }
    }

    public void Initialize(List<GameObject> fog)
    {
        fogOfWarPlanes = fog;
        for (int i = 0; i < fogOfWarPlanes.Count; i++)
        {
            mesh.Add(fogOfWarPlanes[i].GetComponent<MeshFilter>().mesh);
            vertices.Add(mesh[i].vertices);
            colors.Add(new Color[vertices[i].Length]);
            for (int j = 0; j < colors[i].Length; j++)
                colors[i][j] = Color.black;
            UpdateColor(i);
        }
        StartCoroutine(CheckFog());
    }

    private void UpdateColor(int i)
    {
        mesh[i].colors = colors[i];
    }
}
