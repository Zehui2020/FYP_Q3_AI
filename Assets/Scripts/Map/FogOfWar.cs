using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public List<GameObject> fogOfWarPlane;
    public LayerMask fogLayer;
    public float radius = 5f;
    private float radiusSqr {  get { return radius; } }

    private List<Mesh> mesh = new List<Mesh>();
    private List<Vector3[]> vertices = new List<Vector3[]>();
    private List<Color[]> colors = new List<Color[]>();

    private bool isInitiated;

    private void FixedUpdate()
    {
        if (!isInitiated)
            return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, PlayerController.Instance.transform.position - transform.position, fogLayer);
        if (hit)
        {
            for (int i = 0; i < fogOfWarPlane.Count; i++)
            {
                for (int j = 0; j < vertices[i].Length; j++)
                {
                    Vector3 v = fogOfWarPlane[i].transform.TransformPoint(vertices[i][j]);
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

    public void Initialize()
    {
        for (int i = 0; i < fogOfWarPlane.Count; i++)
        {
            mesh.Add(fogOfWarPlane[i].GetComponent<MeshFilter>().mesh);
            vertices.Add(mesh[i].vertices);
            colors.Add(new Color[vertices[i].Length]);
            for (int j = 0; j < colors[i].Length; j++)
                colors[i][j] = Color.black;
            UpdateColor(i);
        }
        isInitiated = true;
    }

    private void UpdateColor(int i)
    {
        mesh[i].colors = colors[i];
    }
}
