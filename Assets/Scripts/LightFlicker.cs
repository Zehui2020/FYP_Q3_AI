using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    public bool flickerOnStart;
    public float maximumDim;
    public float maximumBoost;
    public float speed;
    public float strength;

    private bool noFlicker;
    private Light2D source;
    private float initialIntensity;
    private Coroutine flickerRoutine;

    public void Reset()
    {
        maximumDim = 0.2f;
        maximumBoost = 0.2f;
        speed = 0.1f;
        strength = 250;
    }

    public void Start()
    {
        source = GetComponent<Light2D>();
        initialIntensity = source.intensity;
        source.intensity = 0;

        if (flickerOnStart)
            StartFlicker();
    }

    public void StartFlicker()
    {
        if (flickerRoutine == null)
        {
            noFlicker = false;
            flickerRoutine = StartCoroutine(Flicker());
        }
    }

    public void StopFlicker()
    {
        noFlicker = true;
        source.intensity = 0;
    }

    private IEnumerator Flicker()
    {
        while (!noFlicker)
        {
            source.intensity = Mathf.Lerp(source.intensity, Random.Range(initialIntensity - maximumDim, initialIntensity + maximumBoost), strength * Time.deltaTime);
            yield return new WaitForSeconds(speed);
        }

        flickerRoutine = null;
    }
}