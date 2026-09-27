using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class EclipsePlayerBuilder
{
    [MenuItem("Tools/Eclipse Sector/Build FPS Player")]
    public static void BuildFPSPlayer()
    {
        GameObject playerRoot = GameObject.Find("Player");

        if (playerRoot == null)
        {
            playerRoot = new GameObject("Player");
            Undo.RegisterCreatedObjectUndo(playerRoot, "Create Player root");
        }

        // Remove previous generated player if this tool was already used.
        Transform old = playerRoot.transform.Find("FPS_Player");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        GameObject fps = new GameObject("FPS_Player");
        Undo.RegisterCreatedObjectUndo(fps, "Create FPS Player");
        fps.transform.SetParent(playerRoot.transform, false);
        fps.transform.position = new Vector3(0f, 1.05f, -2.5f);

        CharacterController cc = fps.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.35f;
        cc.center = new Vector3(0f, 0.9f, 0f);
        cc.stepOffset = 0.35f;
        cc.slopeLimit = 50f;

        // Reuse existing Main Camera.
        Camera cam = Camera.main;
        GameObject camGO;

        if (cam != null)
        {
            camGO = cam.gameObject;
        }
        else
        {
            camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";
            camGO.AddComponent<AudioListener>();
        }

        camGO.name = "Main Camera";
        camGO.tag = "MainCamera";
        camGO.transform.SetParent(fps.transform, false);
        camGO.transform.localPosition = new Vector3(0f, 1.62f, 0f);
        camGO.transform.localRotation = Quaternion.identity;

        // Disable any duplicate camera under the Player root.
        foreach (Camera c in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            if (c != cam && c.gameObject.activeSelf)
                c.gameObject.SetActive(false);
        }

        SimpleFPSController controller = fps.AddComponent<SimpleFPSController>();
        controller.playerCamera = cam;

        Selection.activeGameObject = fps;

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        EditorUtility.DisplayDialog(
            "Eclipse Sector",
            "Jugador FPS creado.\n\nWASD = mover\nShift = correr\nEspacio = saltar\nMouse = mirar\nEsc = liberar cursor\n\nGuarda con Ctrl+S.",
            "OK"
        );
    }
}
