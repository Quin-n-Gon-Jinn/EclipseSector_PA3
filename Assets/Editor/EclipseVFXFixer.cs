using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.IO;

public static class EclipseVFXFixer
{
    private const string VFXFolder = "Assets/VFX";
    private const string TexturePath = "Assets/VFX/SoftParticle.png";
    private const string SmokeMatPath = "Assets/VFX/MAT_SmokeParticle.mat";
    private const string SparkMatPath = "Assets/VFX/MAT_SparkParticle.mat";

    [MenuItem("Tools/Eclipse Sector/Fix Pink VFX")]
    public static void FixPinkVFX()
    {
        EnsureFolder(VFXFolder);

        Texture2D softTex = CreateOrLoadSoftParticleTexture();
        Material smokeMat = CreateParticleMaterial(SmokeMatPath, softTex, false);
        Material sparkMat = CreateParticleMaterial(SparkMatPath, softTex, true);

        int smokeCount = 0;
        int sparkCount = 0;

        ParticleSystem[] systems = Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);

        foreach (ParticleSystem ps in systems)
        {
            ParticleSystemRenderer r = ps.GetComponent<ParticleSystemRenderer>();
            if (r == null) continue;

            if (ps.name.StartsWith("Smoke_"))
            {
                r.sharedMaterial = smokeMat;
                r.renderMode = ParticleSystemRenderMode.Billboard;
                r.sortMode = ParticleSystemSortMode.Distance;

                var velocity = ps.velocityOverLifetime;
                velocity.enabled = false;

                ps.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                smokeCount++;
            }
            else if (ps.name.StartsWith("Sparks_"))
            {
                r.sharedMaterial = sparkMat;
                r.renderMode = ParticleSystemRenderMode.Billboard;

                var velocity = ps.velocityOverLifetime;
                velocity.enabled = false;
                sparkCount++;
            }
        }

        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog(
            "Eclipse Sector",
            $"VFX corregidos.\n\nHumo: {smokeCount}\nChispas: {sparkCount}\n\nLos bloques rosados deberían desaparecer y el aviso de Particle Velocity también.",
            "OK"
        );
    }

    private static Texture2D CreateOrLoadSoftParticleTexture()
    {
        Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
        if (existing != null) return existing;

        const int size = 128;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.name = "SoftParticle";

        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), center) / radius;
                float alpha = Mathf.Clamp01(1f - d);
                alpha = alpha * alpha * (3f - 2f * alpha);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        tex.Apply();

        byte[] png = tex.EncodeToPNG();
        File.WriteAllBytes(TexturePath, png);
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(TexturePath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(TexturePath);
        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
    }

    private static Material CreateParticleMaterial(string path, Texture2D tex, bool additive)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
                shader = Shader.Find("Universal Render Pipeline/Unlit");

            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }

        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", tex);
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", Color.white);

        if (mat.HasProperty("_Surface"))
            mat.SetFloat("_Surface", 1f);
        if (mat.HasProperty("_Blend"))
            mat.SetFloat("_Blend", additive ? 1f : 0f);

        mat.SetOverrideTag("RenderType", "Transparent");
        mat.renderQueue = (int)RenderQueue.Transparent;

        if (mat.HasProperty("_SrcBlend"))
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        if (mat.HasProperty("_DstBlend"))
            mat.SetInt("_DstBlend", additive ? (int)BlendMode.One : (int)BlendMode.OneMinusSrcAlpha);
        if (mat.HasProperty("_ZWrite"))
            mat.SetInt("_ZWrite", 0);

        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (string.IsNullOrEmpty(parent)) parent = "Assets";
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);

        AssetDatabase.CreateFolder(parent, folder);
    }
}
