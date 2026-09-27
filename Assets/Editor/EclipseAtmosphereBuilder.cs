using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class EclipseAtmosphereBuilder
{
    [MenuItem("Tools/Eclipse Sector/Build Atmosphere + VFX")]
    public static void BuildAtmosphere()
    {
        ConfigurePost();
        DimSun();

        GameObject effects = GetRoot("Effects");
        GameObject lighting = GetRoot("Lighting");
        ClearChild(effects.transform, "Generated_AtmosphereVFX");
        ClearChild(lighting.transform, "Generated_DynamicAtmosphere");

        Transform vfx = NewRoot("Generated_AtmosphereVFX", effects.transform);
        Transform dyn = NewRoot("Generated_DynamicAtmosphere", lighting.transform);

        Material red = GlowMat("Assets/Materials/MAT_EmergencyGlow.mat",
            new Color(0.22f,0.01f,0.01f), new Color(6f,0.03f,0.01f));
        Material cyan = GlowMat("Assets/Materials/MAT_ReactorGlow.mat",
            new Color(0.01f,0.08f,0.12f), new Color(0.02f,4f,7f));

        GlowBar("EmergencyBar_01", dyn, new Vector3(0,3.82f,8), red);
        GlowBar("EmergencyBar_02", dyn, new Vector3(0,3.82f,14), red);
        GlowBar("EmergencyBar_03", dyn, new Vector3(0,3.82f,18), red);

        var flickerGO = new GameObject("Light_Emergency_Realtime_Flicker");
        flickerGO.transform.SetParent(dyn); flickerGO.transform.position = new Vector3(0,3.55f,14);
        var fl = flickerGO.AddComponent<Light>();
        fl.type = LightType.Point; fl.color = new Color(1f,0.03f,0.01f);
        fl.range = 7f; fl.intensity = 5f; fl.shadows = LightShadows.Soft;
        fl.lightmapBakeType = LightmapBakeType.Realtime;
        var fx = flickerGO.AddComponent<EmergencyLightFlicker>();
        fx.baseIntensity = 5f; fx.variation = 2.5f; fx.speed = 5.5f;

        GlowBar("ReactorGlow_A", dyn, new Vector3(0,4.25f,30.2f), cyan, new Vector3(2.8f,.07f,.12f));
        GlowBar("ReactorGlow_B", dyn, new Vector3(0,4.25f,31.8f), cyan, new Vector3(2.8f,.07f,.12f));

        var rg = new GameObject("Light_Reactor_Atmosphere_Realtime");
        rg.transform.SetParent(dyn); rg.transform.position = new Vector3(0,4.3f,31);
        var rl = rg.AddComponent<Light>();
        rl.type = LightType.Point; rl.color = new Color(.03f,.55f,1f);
        rl.range = 9f; rl.intensity = 7f; rl.shadows = LightShadows.Soft;
        rl.lightmapBakeType = LightmapBakeType.Realtime;

        Smoke("Smoke_Corridor", vfx, new Vector3(-1.25f,.25f,17), .7f);
        Smoke("Smoke_Reactor", vfx, new Vector3(-3.8f,2.3f,32.5f), 1f);
        Sparks("Sparks_Reactor", vfx, new Vector3(0,4.2f,31), new Color(.05f,.8f,1f));
        Sparks("Sparks_DamagedWall", vfx, new Vector3(1.75f,2.6f,16.5f), new Color(1f,.12f,.02f));

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Eclipse Sector",
            "Atmósfera lista: Bloom, Vignette, Color Adjustments, luces emisivas, luz Realtime, humo y chispas.\n\nGuarda con Ctrl+S y prueba Play.",
            "OK");
    }

    static void ConfigurePost()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            AssetDatabase.CreateFolder("Assets","Settings");

        const string path = "Assets/Settings/Eclipse_PostProcess_Profile.asset";
        VolumeProfile p = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
        if (p == null) { p = ScriptableObject.CreateInstance<VolumeProfile>(); AssetDatabase.CreateAsset(p,path); }

        if (p.TryGet<Bloom>(out _)) p.Remove<Bloom>();
        if (p.TryGet<ColorAdjustments>(out _)) p.Remove<ColorAdjustments>();
        if (p.TryGet<Vignette>(out _)) p.Remove<Vignette>();
        if (p.TryGet<Tonemapping>(out _)) p.Remove<Tonemapping>();

        var b = p.Add<Bloom>(true); b.intensity.Override(1.15f); b.threshold.Override(.85f); b.scatter.Override(.65f);
        var c = p.Add<ColorAdjustments>(true); c.postExposure.Override(-.3f); c.contrast.Override(18f); c.saturation.Override(-12f);
        c.colorFilter.Override(new Color(.86f,.92f,1f,1f));
        var v = p.Add<Vignette>(true); v.intensity.Override(.28f); v.smoothness.Override(.55f);
        var t = p.Add<Tonemapping>(true); t.mode.Override(TonemappingMode.ACES);

        GameObject g = GameObject.Find("Global Volume");
        if (g == null) { g = new GameObject("Global Volume"); g.AddComponent<Volume>(); }
        var vol = g.GetComponent<Volume>(); vol.isGlobal = true; vol.priority = 10; vol.sharedProfile = p;
        EditorUtility.SetDirty(p);
    }

    static void DimSun()
    {
        var g = GameObject.Find("Directional Light");
        if (g == null) return;
        var l = g.GetComponent<Light>();
        if (l == null) return;
        l.color = new Color(.42f,.52f,.70f); l.intensity = .18f; l.shadows = LightShadows.Soft;
        g.transform.rotation = Quaternion.Euler(35,-30,0);
    }

    static GameObject GetRoot(string name)
    {
        var g = GameObject.Find(name);
        return g != null ? g : new GameObject(name);
    }

    static Transform NewRoot(string name, Transform parent)
    {
        var g = new GameObject(name); g.transform.SetParent(parent,false); return g.transform;
    }

    static void ClearChild(Transform parent, string name)
    {
        var t = parent.Find(name); if (t != null) Object.DestroyImmediate(t.gameObject);
    }

    static Material GlowMat(string path, Color baseColor, Color emission)
    {
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            var sh = Shader.Find("Universal Render Pipeline/Lit");
            m = new Material(sh); AssetDatabase.CreateAsset(m,path);
        }
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor",baseColor);
        if (m.HasProperty("_EmissionColor")) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor",emission); }
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness",.7f);
        EditorUtility.SetDirty(m);
        return m;
    }

    static void GlowBar(string name, Transform parent, Vector3 pos, Material mat, Vector3? scale = null)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = name; g.transform.SetParent(parent); g.transform.position = pos;
        g.transform.localScale = scale ?? new Vector3(1.25f,.08f,.22f);
        g.GetComponent<Renderer>().sharedMaterial = mat;
        Object.DestroyImmediate(g.GetComponent<Collider>());
    }

    static void Smoke(string name, Transform parent, Vector3 pos, float scale)
    {
        var g = new GameObject(name); g.transform.SetParent(parent); g.transform.position = pos; g.transform.localScale = Vector3.one*scale;
        var ps = g.AddComponent<ParticleSystem>();
        var main = ps.main; main.loop=true; main.startLifetime=new ParticleSystem.MinMaxCurve(3.5f,6f);
        main.startSpeed=new ParticleSystem.MinMaxCurve(.15f,.45f); main.startSize=new ParticleSystem.MinMaxCurve(.6f,1.5f);
        main.startColor=new ParticleSystem.MinMaxGradient(new Color(.12f,.12f,.14f,.1f),new Color(.28f,.3f,.34f,.32f));
        main.maxParticles=100;
        var em=ps.emission; em.rateOverTime=8;
        var sh=ps.shape; sh.shapeType=ParticleSystemShapeType.Cone; sh.angle=18; sh.radius=.45f;
        var vel=ps.velocityOverLifetime; vel.enabled=true; vel.y=new ParticleSystem.MinMaxCurve(.18f,.42f);
    }

    static void Sparks(string name, Transform parent, Vector3 pos, Color color)
    {
        var g = new GameObject(name); g.transform.SetParent(parent); g.transform.position = pos;
        var ps = g.AddComponent<ParticleSystem>();
        var main=ps.main; main.loop=true; main.startLifetime=new ParticleSystem.MinMaxCurve(.2f,.8f);
        main.startSpeed=new ParticleSystem.MinMaxCurve(.8f,2.8f); main.startSize=new ParticleSystem.MinMaxCurve(.02f,.08f);
        main.startColor=color; main.gravityModifier=.2f; main.maxParticles=120;
        var em=ps.emission; em.rateOverTime=12;
        var sh=ps.shape; sh.shapeType=ParticleSystemShapeType.Sphere; sh.radius=.8f;
    }
}
