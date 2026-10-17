using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Invernadero7.Editor
{
    [InitializeOnLoad]
    public static class ProjectTools
    {
        const string ScenePath="Assets/Invernadero7/Scenes/Invernadero7.unity";
        static ProjectTools()
        {
            EditorApplication.delayCall += () =>
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.SessionState.GetBool("I7Ready",false)) return;
                UnityEditor.SessionState.SetBool("I7Ready",true);
                if(EditorSceneManager.GetActiveScene().path=="" && !EditorSceneManager.GetActiveScene().isDirty)
                    EditorSceneManager.OpenScene(ScenePath);
                Debug.Log("INVERNADERO 7: abre Assets/Invernadero7/Scenes/Invernadero7 y pulsa Play. Menu superior Invernadero 7 para generar Windows.");
            };
        }
        [MenuItem("Invernadero 7/1 - Abrir escena del juego")]
        public static void Open()
        {
            if(EditorApplication.isPlaying) return;
            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(ScenePath);
        }
        [MenuItem("Invernadero 7/2 - Generar ejecutable Windows")]
        public static void BuildWindows()
        {
            if(EditorApplication.isPlaying) { EditorUtility.DisplayDialog("Deten el juego","Desactiva Play antes de generar el ejecutable.","Aceptar"); return; }
            if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone,BuildTarget.StandaloneWindows64))
            { EditorUtility.DisplayDialog("Falta soporte de Windows","Unity Hub > Instalaciones > engranaje de tu version > Add modules. Instala Windows Build Support y vuelve a intentar.","Aceptar"); return; }
            if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            PlayerSettings.productName="Invernadero 7"; PlayerSettings.companyName="Proyecto Academico";
            PlayerSettings.defaultScreenWidth=1280; PlayerSettings.defaultScreenHeight=720;
            PlayerSettings.fullScreenMode=FullScreenMode.Windowed; PlayerSettings.resizableWindow=true;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
            string destination=Path.Combine(Directory.GetParent(Application.dataPath).FullName,"Builds/Windows/Invernadero7.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            BuildReport report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{ScenePath},target=BuildTarget.StandaloneWindows64,locationPathName=destination,options=BuildOptions.None});
            if(report.summary.result==BuildResult.Succeeded)
            {
                EditorUtility.RevealInFinder(destination);
                EditorUtility.DisplayDialog("Ejecutable generado","Abre Invernadero7.exe fuera de Unity y prueba una partida completa. Para compartir, comprime TODA la carpeta Windows.","Aceptar");
            }
            else EditorUtility.DisplayDialog("Revisa la consola","La generacion no termino correctamente. Revisa el primer error rojo de Console.","Aceptar");
        }
        [MenuItem("Invernadero 7/3 - Abrir instrucciones")]
        public static void Guide() { EditorUtility.OpenWithDefaultApp(Path.Combine(Directory.GetParent(Application.dataPath).FullName,"LEEME_PRIMERO.txt")); }
    }
    [CustomEditor(typeof(WorldBuilder))]
    public sealed class WorldBuilderInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("El escenario, el jugador, la IA, el audio y los puzzles se crean al pulsar PLAY. Es normal que esta escena solo tenga un objeto antes de ejecutarla. No necesitas arrastrar scripts ni descargar assets.",MessageType.Info);
            if(GUILayout.Button("Abrir instrucciones")) ProjectTools.Guide();
            if(GUILayout.Button("Generar Windows")) ProjectTools.BuildWindows();
        }
    }
}
