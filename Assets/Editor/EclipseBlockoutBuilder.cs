using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class EclipseBlockoutBuilder
{
    private const string MaterialsFolder = "Assets/Materials";

    [MenuItem("Tools/Eclipse Sector/Build Blockout")]
    public static void BuildBlockout()
    {
        EnsureFolder(MaterialsFolder);

        GameObject environment = FindOrCreateRoot("Environment");
        GameObject props = FindOrCreateRoot("Props");
        GameObject lighting = FindOrCreateRoot("Lighting");

        // Clean previous generated content so the tool can be run again safely.
        DeleteChildIfExists(environment.transform, "Generated_Blockout");
        DeleteChildIfExists(props.transform, "Generated_Props");
        DeleteChildIfExists(lighting.transform, "Generated_Lighting");

        // Remove the manual starter floor if it exists outside the generated root.
        GameObject oldFloor = GameObject.Find("Floor_StartRoom");
        if (oldFloor != null)
            Object.DestroyImmediate(oldFloor);

        Transform blockoutRoot = NewEmpty("Generated_Blockout", environment.transform).transform;
        Transform propsRoot = NewEmpty("Generated_Props", props.transform).transform;
        Transform lightingRoot = NewEmpty("Generated_Lighting", lighting.transform).transform;

        Material matFloor = GetOrCreateMaterial("MAT_Floor", new Color(0.16f, 0.17f, 0.19f, 1f), 0.15f, 0.55f);
        Material matWall = GetOrCreateMaterial("MAT_Wall", new Color(0.28f, 0.30f, 0.33f, 1f), 0.05f, 0.35f);
        Material matMetal = GetOrCreateMaterial("MAT_Metal", new Color(0.10f, 0.12f, 0.14f, 1f), 0.75f, 0.60f);
        Material matWarning = GetOrCreateMaterial("MAT_Warning", new Color(0.45f, 0.06f, 0.04f, 1f), 0.15f, 0.45f);
        Material matReactor = GetOrCreateEmissionMaterial(
            "MAT_Reactor_Placeholder",
            new Color(0.04f, 0.25f, 0.33f, 1f),
            new Color(0.0f, 2.2f, 3.2f, 1f)
        );

        // =========================================================
        // START ROOM (10 x 10)
        // =========================================================
        CreateCube("Floor_StartRoom", blockoutRoot, new Vector3(0f, 0f, 0f), new Vector3(10f, 0.2f, 10f), matFloor);
        CreateCube("Wall_Start_Left", blockoutRoot, new Vector3(-5.1f, 2f, 0f), new Vector3(0.2f, 4f, 10.2f), matWall);
        CreateCube("Wall_Start_Right", blockoutRoot, new Vector3(5.1f, 2f, 0f), new Vector3(0.2f, 4f, 10.2f), matWall);
        CreateCube("Wall_Start_Back", blockoutRoot, new Vector3(0f, 2f, -5.1f), new Vector3(10.2f, 4f, 0.2f), matWall);

        // Front wall with a 3-unit doorway.
        CreateCube("Wall_Start_Front_L", blockoutRoot, new Vector3(-3.25f, 2f, 5.1f), new Vector3(3.5f, 4f, 0.2f), matWall);
        CreateCube("Wall_Start_Front_R", blockoutRoot, new Vector3(3.25f, 2f, 5.1f), new Vector3(3.5f, 4f, 0.2f), matWall);
        CreateCube("Wall_Start_DoorLintel", blockoutRoot, new Vector3(0f, 3.4f, 5.1f), new Vector3(3f, 1.2f, 0.2f), matWall);
        CreateCube("Ceiling_StartRoom", blockoutRoot, new Vector3(0f, 4.1f, 0f), new Vector3(10f, 0.2f, 10f), matMetal);

        // =========================================================
        // CORRIDOR (z 5 -> 19)
        // =========================================================
        CreateCube("Floor_Corridor", blockoutRoot, new Vector3(0f, 0f, 12f), new Vector3(4f, 0.2f, 14f), matFloor);
        CreateCube("Wall_Corridor_Left", blockoutRoot, new Vector3(-2.1f, 2f, 12f), new Vector3(0.2f, 4f, 14f), matWall);
        CreateCube("Wall_Corridor_Right", blockoutRoot, new Vector3(2.1f, 2f, 12f), new Vector3(0.2f, 4f, 14f), matWall);
        CreateCube("Ceiling_Corridor", blockoutRoot, new Vector3(0f, 4.1f, 12f), new Vector3(4f, 0.2f, 14f), matMetal);

        // =========================================================
        // RAMP (z 19 -> 25, rise 2)
        // =========================================================
        float rampAngle = -18.435f;
        float rampLength = 6.325f;
        CreateCube(
            "Ramp_ToReactor",
            blockoutRoot,
            new Vector3(0f, 1.1f, 22f),
            new Vector3(4f, 0.3f, rampLength),
            matFloor,
            new Vector3(rampAngle, 0f, 0f)
        );

        // Simple side rails/walls following the ramp.
        CreateCube(
            "Ramp_Wall_Left",
            blockoutRoot,
            new Vector3(-2.1f, 2.6f, 22f),
            new Vector3(0.2f, 3.0f, rampLength),
            matWall,
            new Vector3(rampAngle, 0f, 0f)
        );
        CreateCube(
            "Ramp_Wall_Right",
            blockoutRoot,
            new Vector3(2.1f, 2.6f, 22f),
            new Vector3(0.2f, 3.0f, rampLength),
            matWall,
            new Vector3(rampAngle, 0f, 0f)
        );

        // =========================================================
        // REACTOR ROOM (12 x 10) elevated 2 units
        // =========================================================
        CreateCube("Floor_ReactorRoom", blockoutRoot, new Vector3(0f, 2f, 30f), new Vector3(12f, 0.2f, 10f), matFloor);
        CreateCube("Wall_Reactor_Left", blockoutRoot, new Vector3(-6.1f, 4f, 30f), new Vector3(0.2f, 4f, 10.2f), matWall);
        CreateCube("Wall_Reactor_Right", blockoutRoot, new Vector3(6.1f, 4f, 30f), new Vector3(0.2f, 4f, 10.2f), matWall);
        CreateCube("Wall_Reactor_Back", blockoutRoot, new Vector3(0f, 4f, 35.1f), new Vector3(12.2f, 4f, 0.2f), matWall);

        // Entrance wall with 4-unit opening.
        CreateCube("Wall_Reactor_Front_L", blockoutRoot, new Vector3(-4f, 4f, 24.9f), new Vector3(4f, 4f, 0.2f), matWall);
        CreateCube("Wall_Reactor_Front_R", blockoutRoot, new Vector3(4f, 4f, 24.9f), new Vector3(4f, 4f, 0.2f), matWall);
        CreateCube("Wall_Reactor_DoorLintel", blockoutRoot, new Vector3(0f, 5.4f, 24.9f), new Vector3(4f, 1.2f, 0.2f), matWall);
        CreateCube("Ceiling_ReactorRoom", blockoutRoot, new Vector3(0f, 6.1f, 30f), new Vector3(12f, 0.2f, 10f), matMetal);

        // =========================================================
        // DECORATIVE / STORY PROPS (>= 5)
        // =========================================================
        CreateCube("Crate_A", propsRoot, new Vector3(-3.6f, 0.55f, -2.8f), new Vector3(1.1f, 1.1f, 1.1f), matMetal);
        CreateCube("Crate_B", propsRoot, new Vector3(-3.2f, 0.45f, -1.5f), new Vector3(0.9f, 0.9f, 0.9f), matMetal, new Vector3(0f, 24f, 0f));
        CreateCube("Console_Start", propsRoot, new Vector3(3.9f, 0.75f, 2.3f), new Vector3(1.3f, 1.5f, 0.55f), matMetal);

        CreateCylinder("Barrel_Corridor_A", propsRoot, new Vector3(-1.15f, 0.65f, 9.0f), new Vector3(0.65f, 0.65f, 0.65f), matWarning);
        CreateCylinder("Barrel_Corridor_B", propsRoot, new Vector3(1.15f, 0.65f, 15.5f), new Vector3(0.65f, 0.65f, 0.65f), matWarning, new Vector3(0f, 0f, 82f));

        CreateCylinder("Pipe_Start_A", propsRoot, new Vector3(-4.55f, 2.5f, 1.5f), new Vector3(0.22f, 1.6f, 0.22f), matMetal, new Vector3(0f, 0f, 90f));
        CreateCylinder("Pipe_Start_B", propsRoot, new Vector3(-4.55f, 3.0f, 1.5f), new Vector3(0.18f, 1.3f, 0.18f), matMetal, new Vector3(0f, 0f, 90f));

        // Reactor pedestal and placeholder core.
        CreateCylinder("Reactor_Base", propsRoot, new Vector3(0f, 2.55f, 31f), new Vector3(1.8f, 0.55f, 1.8f), matMetal);
        CreateCylinder("Reactor_Column", propsRoot, new Vector3(0f, 3.55f, 31f), new Vector3(0.85f, 1.0f, 0.85f), matMetal);
        CreateSphere("Reactor_Core_PLACEHOLDER", propsRoot, new Vector3(0f, 4.25f, 31f), new Vector3(1.15f, 1.15f, 1.15f), matReactor);
        CreateCube("Console_Reactor", propsRoot, new Vector3(3.7f, 2.8f, 28.5f), new Vector3(1.4f, 1.4f, 0.6f), matMetal, new Vector3(0f, -20f, 0f));

        // =========================================================
        // LIGHTING
        // =========================================================
        CreatePointLight("Light_Start_Baked", lightingRoot, new Vector3(0f, 3.45f, -1.5f),
            new Color(0.62f, 0.76f, 1.0f), 5f, 9f, LightmapBakeType.Baked);

        CreatePointLight("Light_Corridor_Baked_01", lightingRoot, new Vector3(0f, 3.4f, 8f),
            new Color(1.0f, 0.08f, 0.04f), 6f, 7f, LightmapBakeType.Baked);
        CreatePointLight("Light_Corridor_Baked_02", lightingRoot, new Vector3(0f, 3.4f, 14f),
            new Color(1.0f, 0.08f, 0.04f), 6f, 7f, LightmapBakeType.Baked);
        CreatePointLight("Light_Corridor_Baked_03", lightingRoot, new Vector3(0f, 3.4f, 18f),
            new Color(1.0f, 0.08f, 0.04f), 5f, 6f, LightmapBakeType.Baked);

        CreatePointLight("Light_Reactor_Realtime", lightingRoot, new Vector3(0f, 4.4f, 31f),
            new Color(0.05f, 0.65f, 1.0f), 8f, 11f, LightmapBakeType.Realtime);

        // Mark static geometry for GI/batching.
        MarkChildrenStatic(blockoutRoot);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = environment;
        SceneView.lastActiveSceneView?.FrameSelected();

        Debug.Log("ECLIPSE: Blockout generated successfully. Save the scene with Ctrl+S.");
        EditorUtility.DisplayDialog(
            "Eclipse Sector",
            "Blockout generado.\n\nAhora guarda la escena con Ctrl+S y prueba el recorrido visual en Scene.",
            "OK"
        );
    }

    private static GameObject FindOrCreateRoot(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            go.transform.SetParent(null);
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }

        go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    private static GameObject NewEmpty(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        return go;
    }

    private static void DeleteChildIfExists(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);
        if (child != null)
            Object.DestroyImmediate(child.gameObject);
    }

    private static GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale,
        Material material, Vector3? euler = null)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent, true);
        go.transform.position = position;
        go.transform.eulerAngles = euler ?? Vector3.zero;
        go.transform.localScale = scale;
        AssignMaterial(go, material);
        return go;
    }

    private static GameObject CreateCylinder(string name, Transform parent, Vector3 position, Vector3 scale,
        Material material, Vector3? euler = null)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = name;
        go.transform.SetParent(parent, true);
        go.transform.position = position;
        go.transform.eulerAngles = euler ?? Vector3.zero;
        go.transform.localScale = scale;
        AssignMaterial(go, material);
        return go;
    }

    private static GameObject CreateSphere(string name, Transform parent, Vector3 position, Vector3 scale,
        Material material)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.SetParent(parent, true);
        go.transform.position = position;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = scale;
        AssignMaterial(go, material);
        return go;
    }

    private static void AssignMaterial(GameObject go, Material material)
    {
        Renderer r = go.GetComponent<Renderer>();
        if (r != null)
            r.sharedMaterial = material;
    }

    private static void CreatePointLight(string name, Transform parent, Vector3 position,
        Color color, float intensity, float range, LightmapBakeType bakeType)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, true);
        go.transform.position = position;

        Light light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.Soft;
        light.lightmapBakeType = bakeType;

        if (bakeType == LightmapBakeType.Baked)
            GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.ContributeGI);
    }

    private static void MarkChildrenStatic(Transform root)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            GameObjectUtility.SetStaticEditorFlags(
                t.gameObject,
                StaticEditorFlags.ContributeGI | StaticEditorFlags.BatchingStatic
            );
        }
    }

    private static Material GetOrCreateMaterial(string name, Color baseColor, float metallic, float smoothness)
    {
        string path = $"{MaterialsFolder}/{name}.mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            ApplyLitProperties(existing, baseColor, metallic, smoothness);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.name = name;
        ApplyLitProperties(mat, baseColor, metallic, smoothness);
        AssetDatabase.CreateAsset(mat, path);
        AssetDatabase.SaveAssets();
        return mat;
    }

    private static Material GetOrCreateEmissionMaterial(string name, Color baseColor, Color emission)
    {
        Material mat = GetOrCreateMaterial(name, baseColor, 0.25f, 0.65f);
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emission);
        }
        EditorUtility.SetDirty(mat);
        AssetDatabase.SaveAssets();
        return mat;
    }

    private static void ApplyLitProperties(Material mat, Color baseColor, float metallic, float smoothness)
    {
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", baseColor);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", baseColor);

        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", metallic);

        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", smoothness);
        else if (mat.HasProperty("_Glossiness"))
            mat.SetFloat("_Glossiness", smoothness);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (string.IsNullOrEmpty(parent))
            parent = "Assets";

        if (!AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folder);
    }
}
