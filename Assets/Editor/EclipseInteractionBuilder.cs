using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public static class EclipseInteractionBuilder
{
    [MenuItem("Tools/Eclipse Sector/Build Reactor Interaction")]
    public static void BuildInteraction()
    {
        GameObject interactionsRoot = GameObject.Find("Interactions");
        if (interactionsRoot == null)
            interactionsRoot = new GameObject("Interactions");

        Transform old = interactionsRoot.transform.Find("Reactor_Interaction");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        GameObject interactionGO = new GameObject("Reactor_Interaction");
        interactionGO.transform.SetParent(interactionsRoot.transform, false);
        interactionGO.transform.position = new Vector3(3.7f, 2.8f, 28.5f);

        ReactorInteraction script = interactionGO.AddComponent<ReactorInteraction>();

        GameObject player = GameObject.Find("FPS_Player");
        if (player != null)
            script.player = player.transform;

        GameObject core = GameObject.Find("Reactor_Core_PLACEHOLDER");
        if (core != null)
            script.reactorRenderer = core.GetComponent<Renderer>();

        GameObject reactorLightGO = GameObject.Find("Light_Reactor_Atmosphere_Realtime");
        if (reactorLightGO == null)
            reactorLightGO = GameObject.Find("Light_Reactor_Realtime");

        if (reactorLightGO != null)
            script.reactorLight = reactorLightGO.GetComponent<Light>();

        List<ParticleSystem> particles = new List<ParticleSystem>();
        foreach (ParticleSystem ps in Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
        {
            if (ps.name.Contains("Reactor"))
                particles.Add(ps);
        }

        script.reactorParticles = particles.ToArray();

        Selection.activeGameObject = interactionGO;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        EditorUtility.DisplayDialog(
            "Eclipse Sector",
            "Interacción creada. Acércate a la consola del reactor y presiona E.",
            "OK"
        );
    }
}
