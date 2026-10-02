using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireVisuals : MonoBehaviour
{
    public bool startLit = false;

    [Header("Visuals")]
    public GameObject visuals;       // flame sprite / particles
    public Light2D light2D;          // the 2D light of this fire

    [Header("Flicker")]
    public float flickerStrength = 0.15f;
    public float flickerSpeed = 8f;

    public bool IsLit { get; private set; }
    float baseIntensity;
    Vector3 visualsBaseScale;

    void Awake()
    {
        if (light2D != null) baseIntensity = light2D.intensity;
        if (visuals != null) visualsBaseScale = visuals.transform.localScale;
        SetLit(startLit);
    }

    public void SetLit(bool lit)
    {
        IsLit = lit;
        if (visuals != null) visuals.SetActive(lit);
        if (light2D != null) light2D.enabled = lit;
    }

    void Update()
    {
        if (!IsLit) return;
        float n = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) - 0.5f;
        if (light2D != null)
            light2D.intensity = baseIntensity * (1f + n * flickerStrength * 2f);
        if (visuals != null)
            visuals.transform.localScale = visualsBaseScale * (1f + n * 0.2f);
    }
}