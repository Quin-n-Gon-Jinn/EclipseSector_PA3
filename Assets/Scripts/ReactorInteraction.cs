using UnityEngine;
using UnityEngine.InputSystem;

public class ReactorInteraction : MonoBehaviour
{
    public Transform player;
    public Renderer reactorRenderer;
    public Light reactorLight;
    public ParticleSystem[] reactorParticles;

    public float interactionDistance = 2.4f;
    public float inactiveEmission = 0.35f;
    public float activeEmission = 8f;
    public float inactiveLightIntensity = 0.4f;
    public float activeLightIntensity = 8f;

    private bool activated;
    private Material runtimeMaterial;

    private void Start()
    {
        if (reactorRenderer != null)
        {
            runtimeMaterial = reactorRenderer.material;
            runtimeMaterial.SetFloat("_EmissionIntensity", inactiveEmission);
        }

        if (reactorLight != null)
            reactorLight.intensity = inactiveLightIntensity;

        if (reactorParticles != null)
        {
            foreach (ParticleSystem ps in reactorParticles)
                if (ps != null)
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void Update()
    {
        if (activated || player == null || Keyboard.current == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && Keyboard.current.eKey.wasPressedThisFrame)
            ActivateReactor();
    }

    private void ActivateReactor()
    {
        activated = true;

        if (runtimeMaterial != null)
            runtimeMaterial.SetFloat("_EmissionIntensity", activeEmission);

        if (reactorLight != null)
            reactorLight.intensity = activeLightIntensity;

        if (reactorParticles != null)
        {
            foreach (ParticleSystem ps in reactorParticles)
                if (ps != null)
                    ps.Play();
        }

        Debug.Log("ECLIPSE: Reactor activado.");
    }

    private void OnGUI()
    {
        if (activated || player == null)
            return;

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance <= interactionDistance)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 22;
            style.normal.textColor = Color.white;

            GUI.Label(
                new Rect(Screen.width / 2f - 180f, Screen.height - 120f, 360f, 40f),
                "[E] ACTIVAR REACTOR",
                style
            );
        }
    }
}
