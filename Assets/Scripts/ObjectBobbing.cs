using UnityEngine;

public class ObjectBobbing : MonoBehaviour
{
    [SerializeField] private float bobIntensity = 0.5f;
    [SerializeField] private float bobSpeed = 2f;
    private Vector3 startPos;

    public void InitBobbing()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobIntensity;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}