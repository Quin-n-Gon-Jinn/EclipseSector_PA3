using UnityEngine;
namespace Eclipse.Final
{
    public class MissionHUD : MonoBehaviour
    {
        public MissionController mission;
        public PlayerInteractor interactor;
        public SentinelAI sentinel;
        GUIStyle title, text, small, center;
        bool diagnostics;
        void Styles()
        {
            if (title != null) return;
            title = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, wordWrap = true };
            text = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
            small = new GUIStyle(text) { fontSize = 14 };
            center = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter };
            foreach (var s in new[] {title,text,small,center}) s.normal.textColor = Color.white;
        }
        void OnGUI()
        {
            Styles();
            float scale = Mathf.Max(0.5f, Mathf.Min(Screen.width / 1280f, Screen.height / 720f));
            Matrix4x4 original = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * scale);
            float w = Screen.width / scale, h = Screen.height / scale;
            GUI.Box(new Rect(18,18,430,140), GUIContent.none);
            GUI.Label(new Rect(32,25,405,38), "ECLIPSE SECTOR · PROTOCOLO DE SALIDA", text);
            GUI.Label(new Rect(32,65,405,70), "Integridad " + mission.Health + "%   |   Tiempo " + Mathf.FloorToInt(mission.Remaining / 60f).ToString("00") + ":" + Mathf.FloorToInt(mission.Remaining % 60f).ToString("00") + "\nTerminales " + mission.Repaired + "/" + mission.requiredTerminals + "   |   Reactor " + (mission.ReactorOnline ? "ESTABLE" : "INACTIVO"), text);
            GUI.Label(new Rect(20,h-48,w-40,35), "WASD mover · Mouse mirar · Shift correr · Espacio saltar · E interactuar · Esc pausa", small);
            if (mission.State == MissionState.Playing)
            {
                GUI.Label(new Rect(w/2-15,h/2-15,30,30), "+", center);
                GUI.Label(new Rect(w/2-360,h-135,720,65), interactor.Focus != null ? interactor.Focus.Prompt : mission.ReactorOnline ? "Regresa a la sala inicial y busca EXTRACCIÓN." : mission.Repaired < mission.requiredTerminals ? "Busca los terminales ámbar. Mira el panel y mantén E." : "Busca la consola del reactor.", center);
                GUI.Label(new Rect(w/2-360,175,720,65), mission.Message, center);
            }
            else
            {
                GUI.Box(new Rect(w/2-325,h/2-165,650,330), GUIContent.none);
                bool briefing = mission.State == MissionState.Briefing;
                bool paused = mission.State == MissionState.Paused;
                GUI.Label(new Rect(w/2-290,h/2-140,580,50), briefing ? "PROTOCOLO DE SALIDA" : paused ? "MISIÓN EN PAUSA" : mission.State == MissionState.Won ? "MISIÓN COMPLETADA" : "MISIÓN FALLIDA", title);
                string body = briefing ? "La instalación ha perdido el control del reactor.\n1. Repara los tres terminales ámbar manteniendo E.\n2. Activa la consola azul del reactor.\n3. Regresa al panel verde de extracción.\nEvita al centinela: patrulla, detecta y persigue. Tienes 5 minutos." : paused ? "El tiempo está detenido. Puedes continuar o reiniciar la misión.\n\nEvita al centinela y usa las cajas para cortar su línea de visión." : mission.State == MissionState.Won ? "Reactor estabilizado y operador evacuado.\nIntegridad final: " + mission.Health + "%\nTiempo restante: " + Mathf.RoundToInt(mission.Remaining) + " segundos." : mission.Message;
                GUI.Label(new Rect(w/2-290,h/2-85,580,180), body, text);
                if (GUI.Button(new Rect(w/2-290,h/2+105,270,42), briefing ? "INICIAR MISIÓN" : paused ? "CONTINUAR" : "VOLVER A JUGAR"))
                { if (briefing) mission.Begin(); else if (paused) mission.Resume(); else mission.Restart(); }
                if (GUI.Button(new Rect(w/2+20,h/2+105,270,42), paused ? "REINICIAR" : "SALIR"))
                { if (paused) mission.Restart(); else Application.Quit(); }
            }
            diagnostics = GUI.Toggle(new Rect(w-205,20,185,25), diagnostics, "Diagnóstico de IA");
            if (diagnostics && sentinel != null) GUI.Label(new Rect(w-340,55,320,95), "IA: " + sentinel.State + "\nDecisiones: " + sentinel.Decisions + "\nIntervalo: " + sentinel.decisionInterval + " s (aprox. 5/s)", small);
            GUI.matrix = original;
        }
    }
}
