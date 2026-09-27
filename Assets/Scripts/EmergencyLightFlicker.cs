using UnityEngine;

[RequireComponent(typeof(Light))]
public class EmergencyLightFlicker : MonoBehaviour
{
    public float baseIntensity = 5f;
    public float variation = 2f;
    public float speed = 7f;

    private Light targetLight;
    private float seed;

    private void Awake()
    {
        targetLight = GetComponent<Light>();
        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float n = Mathf.PerlinNoise(seed, Time.time * speed);
        targetLight.intensity = Mathf.Max(0f, baseIntensity + (n - 0.5f) * 2f * variation);
    }
}
