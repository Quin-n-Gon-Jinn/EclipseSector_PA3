using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using Unity.AI.Navigation;
using Eclipse.Final;

public static class EclipseFinalBuilder
{
    const string Source = "Assets/Scenes/EclipseSector.unity";
    public const string Final = "Assets/Scenes/EclipseFinal.unity";
    const string Folder = "Assets/PA4Generated";
    [MenuItem("Tools/Eclipse Final/1 Preparar misión PA4")]
    public static void Prepare()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Final) != null)
        {
            EditorSceneManager.OpenScene(Final);
            Validate();
            Debug.Log("PA4 ya preparado. Se conserva la escena existente."); return;
        }
        if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets", "PA4Generated");
        if (!AssetDatabase.CopyAsset(Source, Final)) throw new Exception("No se pudo copiar la escena PA3.");
        EditorSceneManager.OpenScene(Final);
        var player = UnityEngine.Object.FindFirstObjectByType<SimpleFPSController>();
        var reactor = UnityEngine.Object.FindFirstObjectByType<ReactorInteraction>();
        if (player == null || reactor == null) throw new Exception("Falta jugador o reactor del PA3.");
        // The final starts at a briefing. The PA3 cinematic remains in the original scene.
        foreach (var intro in UnityEngine.Object.FindObjectsByType<IntroCinematicController>(FindObjectsSortMode.None))
        { intro.enabled = false; if (intro.introCameraObject != null) intro.introCameraObject.SetActive(false); }
        foreach (var director in UnityEngine.Object.FindObjectsByType<PlayableDirector>(FindObjectsSortMode.None)) { director.playOnAwake = false; director.enabled = false; }
        foreach (var mover in UnityEngine.Object.FindObjectsByType<IntroCameraMover>(FindObjectsSortMode.None)) mover.gameObject.SetActive(false);
        GameObject cmIntro = GameObject.Find("CM_IntroCamera"); if (cmIntro != null) cmIntro.SetActive(false);
        // Ensure the player camera is not driven by the intro's Cinemachine brain.
        foreach (var behaviour in player.playerCamera.GetComponents<Behaviour>())
            if (behaviour.GetType().Name == "CinemachineBrain") behaviour.enabled = false;
        player.transform.position = new Vector3(0f, 0.15f, -2.5f);
        player.playerCamera.transform.SetParent(player.transform, false);
        player.playerCamera.transform.localPosition = new Vector3(0f,1.62f,0f);
        player.playerCamera.transform.localRotation = Quaternion.identity;
        player.playerCamera.gameObject.SetActive(true); player.playerCamera.enabled = true;
        foreach (var listener in UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None)) listener.enabled = listener.gameObject == player.playerCamera.gameObject;
        if (player.playerCamera.GetComponent<AudioListener>() == null) player.playerCamera.gameObject.AddComponent<AudioListener>();
        player.missionControlsCursor = true; player.enabled = false;
        reactor.externallyControlled = true;
        var root = new GameObject("PA4_Mission");
        var mission = root.AddComponent<MissionController>(); mission.player = player;
        var audio = root.AddComponent<MissionAudio>(); mission.audioSystem = audio;
        audio.ambientSource = root.AddComponent<AudioSource>(); audio.cueSource = root.AddComponent<AudioSource>();
        audio.ambience = Clip("ambience"); audio.repair = Clip("repair"); audio.reactor = Clip("reactor");
        audio.damage = Clip("damage"); audio.win = Clip("win"); audio.lose = Clip("lose");
        var interactor = player.gameObject.AddComponent<PlayerInteractor>(); interactor.mission = mission; interactor.view = player.playerCamera;
        var amber = Material("TerminalAmber", new Color(1f,0.55f,0.08f));
        var green = Material("ExtractionGreen", new Color(0.1f,1f,0.4f));
        var blue = Material("ReactorBlue", new Color(0.08f,0.7f,1f));
        Terminal(root.transform, mission, amber, "A — Energía", new Vector3(3.9f,1.5f,1.9f));
        Terminal(root.transform, mission, amber, "B — Refrigeración", new Vector3(-1.75f,1.35f,12f));
        Terminal(root.transform, mission, amber, "C — Contención", new Vector3(-4.7f,3.4f,32.5f));
        var console = Panel(root.transform, "Consola Reactor", new Vector3(3.7f,3.55f,28.2f), blue);
        var reactorConsole = console.AddComponent<ReactorConsole>(); reactorConsole.mission = mission; reactorConsole.reactor = reactor;
        var exit = Panel(root.transform, "EXTRACCIÓN", new Vector3(0f,1.5f,-4.75f), green);
        var extraction = exit.AddComponent<ExtractionConsole>(); extraction.mission = mission;
        // Bake collider geometry before adding the enemy, with no runtime navigation rebuilding.
        var navRoot = new GameObject("PA4_Navigation");
        var surface = navRoot.AddComponent<NavMeshSurface>(); surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        var controller = player.GetComponent<CharacterController>();
        bool controllerEnabled = controller.enabled;
        controller.enabled = false;
        try { surface.BuildNavMesh(); } finally { controller.enabled = controllerEnabled; }
        if (surface.navMeshData == null) throw new Exception("No se generó NavMesh.");
        AssetDatabase.CreateAsset(surface.navMeshData, Folder + "/FinalNavMesh.asset");
        var sentinelGO = new GameObject("Centinela");
        sentinelGO.transform.SetParent(root.transform);
        // Character center is one unit above its feet; ray origin lies above the capsule.
        var capsule = sentinelGO.AddComponent<CapsuleCollider>(); capsule.center = Vector3.up; capsule.height = 1.8f; capsule.radius = 0.32f;
        var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule); visual.name = "Centinela Visual";
        visual.transform.SetParent(sentinelGO.transform, false); visual.transform.localPosition = Vector3.up;
        UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());
        visual.GetComponent<Renderer>().sharedMaterial = Material("Sentinel", new Color(0.3f,0.35f,0.4f));
        var agent = sentinelGO.AddComponent<NavMeshAgent>(); agent.radius = 0.32f; agent.height = 1.8f; agent.stoppingDistance = 0.9f;
        if (!NavMesh.SamplePosition(new Vector3(-3f,2.2f,29f), out NavMeshHit spawn, 2f, NavMesh.AllAreas)) throw new Exception("No hay NavMesh en spawn del centinela.");
        sentinelGO.transform.position = spawn.position;
        var ai = sentinelGO.AddComponent<SentinelAI>(); ai.mission = mission; ai.player = player.transform; ai.indicator = visual.GetComponent<Renderer>();
        ai.waypoints = new[] { Waypoint(root.transform,new Vector3(-3f,2.2f,29f)), Waypoint(root.transform,new Vector3(3f,2.2f,33f)), Waypoint(root.transform,new Vector3(0f,0.2f,10f)) };
        var hud = root.AddComponent<MissionHUD>(); hud.mission = mission; hud.interactor = interactor; hud.sentinel = ai;
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Final,true) };
        PlayerSettings.productName = "Eclipse Sector PA4";
        PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
        AssetDatabase.SaveAssets(); EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); Validate();
        Debug.Log("PA4 preparado. Abre Game, pulsa Play e inicia la misión. Luego genera y prueba el build.");
    }
    static AudioClip Clip(string name)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/PA4/" + name + ".wav");
        if (clip == null) throw new Exception("Falta audio: " + name); return clip;
    }
    static Material Material(string name, Color color)
    {
        string path = Folder + "/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path); if (material != null) return material;
        var shader = Shader.Find("Universal Render Pipeline/Lit"); if (shader == null) throw new Exception("No se encuentra shader URP Lit.");
        material = new Material(shader); material.SetColor("_BaseColor",color); material.EnableKeyword("_EMISSION"); material.SetColor("_EmissionColor",color*2f);
        AssetDatabase.CreateAsset(material,path); return material;
    }
    static GameObject Panel(Transform parent, string name, Vector3 position, Material material)
    {
        var panel = GameObject.CreatePrimitive(PrimitiveType.Cube); panel.name = name; panel.transform.SetParent(parent);
        panel.transform.position = position; panel.transform.localScale = new Vector3(0.65f,0.5f,0.22f); panel.GetComponent<Renderer>().sharedMaterial = material;
        // Small shadowless light makes the panel readable in dark areas without extra shadow maps.
        var light = new GameObject("Indicador").AddComponent<Light>(); light.transform.SetParent(panel.transform,false);
        light.transform.localPosition = new Vector3(0,0,-2f); light.type = LightType.Point; light.color = material.GetColor("_BaseColor"); light.range = 2.5f; light.intensity = 1f; light.shadows = LightShadows.None;
        return panel;
    }
    static void Terminal(Transform parent, MissionController mission, Material material, string name, Vector3 position)
    {
        var panel = Panel(parent,"Terminal " + name,position,material);
        var terminal = panel.AddComponent<RepairTerminal>(); terminal.mission = mission; terminal.terminalName = name; terminal.indicator = panel.GetComponent<Renderer>();
        var effect = new GameObject("Reparación VFX").AddComponent<ParticleSystem>(); effect.transform.SetParent(panel.transform,false); effect.transform.localPosition = new Vector3(0,0,-1f);
        var main = effect.main; main.playOnAwake = false; main.loop = false; main.duration = 0.6f; main.startLifetime = 0.6f; main.startSpeed = 1.2f; main.startSize = 0.08f; main.startColor = Color.green; main.maxParticles = 32;
        var emission = effect.emission; emission.rateOverTime = 0; emission.SetBursts(new[] {new ParticleSystem.Burst(0f,24)});
        var renderer = effect.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/VFX/MAT_SparkParticle.mat");
        terminal.repairedEffect = effect;
    }
    static Transform Waypoint(Transform parent, Vector3 position)
    { var go = new GameObject("Patrulla"); go.transform.SetParent(parent); go.transform.position = position; return go.transform; }
    [MenuItem("Tools/Eclipse Final/2 Validar configuración")]
    public static void Validate()
    {
        var mission = UnityEngine.Object.FindFirstObjectByType<MissionController>();
        var terminals = UnityEngine.Object.FindObjectsByType<RepairTerminal>(FindObjectsSortMode.None);
        var ai = UnityEngine.Object.FindFirstObjectByType<SentinelAI>();
        var nav = UnityEngine.Object.FindFirstObjectByType<NavMeshSurface>();
        if (mission == null || mission.player == null || mission.audioSystem == null || terminals.Length != mission.requiredTerminals || ai == null || nav == null || nav.navMeshData == null)
            throw new Exception("Configuración PA4 incompleta. Revisa la escena final y referencias.");
        foreach (var terminal in terminals) if (terminal.mission != mission || terminal.repairedEffect == null) throw new Exception("Terminal incompleto.");
        foreach (var point in ai.waypoints) if (!NavMesh.SamplePosition(point.position,out _,2f,NavMesh.AllAreas)) throw new Exception("Waypoint fuera de NavMesh.");
        if (UnityEngine.Object.FindFirstObjectByType<ReactorConsole>() == null || UnityEngine.Object.FindFirstObjectByType<ExtractionConsole>() == null) throw new Exception("Falta consola.");
        Debug.Log("Configuración PA4 válida. Esto NO sustituye pruebas de gameplay ni del ejecutable.");
    }
    [MenuItem("Tools/Eclipse Final/3 Generar Windows x64")]
    public static void BuildWindows()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Final) == null) Prepare();
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Final) == null) throw new Exception("Preparación cancelada.");
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(Final); Validate();
        if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64)) throw new Exception("Instala Windows Build Support desde Unity Hub.");
        Directory.CreateDirectory("Builds/Windows");
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] {Final}, locationPathName = "Builds/Windows/EclipseSector.exe", target = BuildTarget.StandaloneWindows64, options = BuildOptions.None });
        File.WriteAllText("Builds/Windows/BuildResult.txt", "Resultado: " + report.summary.result + "\nErrores: " + report.summary.totalErrors + "\nFecha UTC: " + DateTime.UtcNow.ToString("O"));
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Build fallido. Revisa Console.");
        Debug.Log("Build generado en Builds/Windows. Ejecuta EclipseSector.exe y prueba victoria, derrota y reinicio.");
    }
}
