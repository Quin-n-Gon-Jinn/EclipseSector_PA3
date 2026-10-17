using UnityEngine;
using UnityEngine.SceneManagement;

namespace Invernadero7
{
    public enum SessionState { Menu, Playing, Paused, Won, Lost }

    public sealed class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        public SessionState State { get; private set; } = SessionState.Menu;
        public float Energy { get; private set; } = 100f;
        public float Seconds { get; private set; } = 360f;
        public int Circuits { get; private set; }
        public string Message { get; private set; }
        public string Ending { get; private set; }
        public string Prompt { get; set; }
        public int Puzzle { get; private set; } = -1;
        public bool[] Completed = new bool[3];
        public int Pressure { get; private set; }
        public int SequenceStep { get; private set; }
        public int[] Channels = { 2, 0, 1 };
        public readonly int[] Sequence = { 1, 3, 0, 2 };
        float messageUntil, damageUntil;
        bool help;
        GUIStyle title, heading, body, small, button;
        Texture2D flat;
        readonly Color mint = new Color(.4f, .94f, .72f);

        void Awake() { Instance = this; Time.timeScale = 1; }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Puzzle >= 0) ClosePuzzle();
                else if (State == SessionState.Playing) SetState(SessionState.Paused);
                else if (State == SessionState.Paused) SetState(SessionState.Playing);
            }
            if (State != SessionState.Playing) return;
            Seconds = Mathf.Max(0, Seconds - Time.deltaTime);
            if (Seconds <= 0) Finish(false, "El sistema de soporte se quedo sin energia.");
        }
        public void SetState(SessionState state)
        {
            State = state;
            bool locked = state == SessionState.Playing && Puzzle < 0;
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
            Time.timeScale = state == SessionState.Paused ? 0 : 1;
        }
        public void Notice(string message, float duration = 4)
        { Message = message; messageUntil = Time.unscaledTime + duration; }
        public void OpenPuzzle(int index)
        {
            if (Completed[index]) { Notice("Este circuito ya esta reparado."); return; }
            Puzzle = index; Prompt = ""; SetState(SessionState.Playing);
        }
        public void ClosePuzzle() { Puzzle = -1; SetState(SessionState.Playing); }
        public void Damage(float amount)
        {
            if (State != SessionState.Playing || Time.time < damageUntil) return;
            damageUntil = Time.time + 2.5f;
            Energy = Mathf.Max(0, Energy - amount);
            SoundBank.Instance.Play("damage");
            Notice("Contacto con el robot: -" + amount + " de energia. Alejate y rompe su linea de vision.");
            if (Energy <= 0) Finish(false, "Tu equipo se quedo sin energia.");
        }
        public void Complete()
        {
            int index = Puzzle;
            if (index < 0 || Completed[index]) return;
            Completed[index] = true; Circuits++;
            WorldBuilder.Repaired(index);
            SoundBank.Instance.Play("success");
            ClosePuzzle();
            Notice(Circuits == 3 ? "Tres circuitos restaurados. Vuelve a la consola de entrada." : "Circuito restaurado. Busca el siguiente sector.", 6);
        }
        public void ActivateCore()
        {
            if (Circuits < 3) { Notice("Faltan " + (3 - Circuits) + " circuitos por reparar."); return; }
            Finish(true, "El riego funciona. Invernadero 7 vuelve a respirar.");
        }
        void Finish(bool won, string text)
        {
            Puzzle = -1; Ending = text;
            SetState(won ? SessionState.Won : SessionState.Lost);
            SoundBank.Instance.Play(won ? "success" : "damage");
        }
        void Restart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        void Styles()
        {
            if (flat != null) return;
            flat = new Texture2D(1, 1); flat.SetPixel(0, 0, Color.white); flat.Apply();
            title = new GUIStyle(GUI.skin.label) { fontSize = 55, fontStyle = FontStyle.Bold, wordWrap = true };
            title.normal.textColor = Color.white;
            heading = new GUIStyle(title) { fontSize = 27 };
            body = new GUIStyle(GUI.skin.label) { fontSize = 20, wordWrap = true };
            body.normal.textColor = new Color(.88f,.94f,.92f);
            small = new GUIStyle(body) { fontSize = 15 };
            button = new GUIStyle(GUI.skin.button) { fontSize = 20, padding = new RectOffset(15,15,12,12) };
        }
        void Box(Rect rect, Color color) { GUI.color = color; GUI.DrawTexture(rect, flat); GUI.color = Color.white; }
        bool Button(float x, float y, float w, string label) => GUI.Button(new Rect(x,y,w,46),label,button);
        void Label(float x,float y,float w,float h,string label, GUIStyle style) => GUI.Label(new Rect(x,y,w,h),label,style);
        void OnGUI()
        {
            Styles(); GUI.matrix = Matrix4x4.Scale(new Vector3(Screen.width / 1280f, Screen.height / 720f, 1));
            if (State == SessionState.Playing)
            {
                Box(new Rect(20,20,345,92),new Color(.025f,.075f,.065f,.92f));
                Label(35,29,320,30,"INVERNADERO 7  /  RESTAURACION",small);
                Label(35,59,315,40,"CIRCUITOS   " + Circuits + " / 3",heading);
                Box(new Rect(972,20,288,92),new Color(.025f,.075f,.065f,.92f));
                Label(987,28,265,30,"TIEMPO  " + Mathf.FloorToInt(Seconds / 60).ToString("00") + ":" + Mathf.FloorToInt(Seconds % 60).ToString("00"),heading);
                Label(987,66,255,25,"ENERGIA  " + Mathf.CeilToInt(Energy) + "%",small);
                Box(new Rect(1110,75,132,7),new Color(.15f,.25f,.22f)); Box(new Rect(1110,75,132*Energy/100,7),mint);
                if (Puzzle < 0)
                {
                    Box(new Rect(637,355,6,10),mint); Box(new Rect(635,357,10,6),mint);
                    if (!string.IsNullOrEmpty(Prompt)) { Box(new Rect(280,558,720,52),new Color(0,0,0,.8f)); Label(300,568,680,35,Prompt,body); }
                }
                Label(24,685,1200,25,"WASD mover   |   Raton mirar   |   E interactuar   |   Shift correr   |   Esc pausa",small);
                if (Time.unscaledTime < messageUntil)
                {
                    Box(new Rect(250,125,780,70),new Color(.03f,.1f,.08f,.96f)); Label(270,137,740,55,Message,body);
                }
                if (Puzzle >= 0) DrawPuzzle();
                return;
            }
            Box(new Rect(0,0,1280,720),new Color(.012f,.035f,.03f,.88f));
            Box(new Rect(75,75,7,560),mint);
            if (State == SessionState.Menu)
            {
                Label(110,85,1020,30,"UNA INSTALACION. TRES CIRCUITOS. SEIS MINUTOS.",small);
                Label(105,140,1040,85,"INVERNADERO 7",title);
                Label(110,240,910,100,"El riego ha fallado y un robot de mantenimiento perdio el control. Repara los tres circuitos y activa la consola central antes de que se agote el tiempo.",body);
                Label(110,355,930,85,"Explora los sectores AMBAR, AZUL y VIOLETA. Lee las pistas de sus paneles. El robot seguira activo mientras reparas: busca un momento seguro.",body);
                if (Button(110,480,260,"COMENZAR")) { SetState(SessionState.Playing); Notice("Busca los tres paneles de colores. La consola central esta junto a la entrada.",7); }
                if (Button(390,480,250,"CONTROLES / AYUDA")) help = !help;
                if (Button(660,480,180,"SALIR")) Application.Quit();
                if (help) Label(110,550,1000,110,"WASD: movimiento. Raton: mirar. Shift: correr. E: usar panel. Esc: cerrar panel o pausar. Los paneles se resuelven con el raton. Puedes repetirlos sin penalizacion. El robot no entra en las bahias de reparacion, pero puede verte desde el pasillo.",body);
            }
            else if (State == SessionState.Paused)
            {
                Label(110,165,1040,90,"PAUSA",title);
                Label(110,270,950,70,"Repara los tres circuitos y vuelve a la consola central.",body);
                if (Button(110,390,260,"CONTINUAR")) SetState(SessionState.Playing);
                if (Button(390,390,260,"REINICIAR")) Restart();
            }
            else
            {
                Label(110,150,1080,100,State == SessionState.Won ? "VIDA RESTAURADA" : "SISTEMA DETENIDO",title);
                Label(110,280,980,80,Ending,heading);
                Label(110,370,1000,60,"Circuitos: " + Circuits + "/3   |   Energia: " + Mathf.CeilToInt(Energy) + "%   |   Tiempo restante: " + Mathf.CeilToInt(Seconds) + " s",body);
                if (Button(110,480,285,"VOLVER A INTENTAR")) Restart();
                if (Button(415,480,200,"SALIR")) Application.Quit();
            }
        }
        void DrawPuzzle()
        {
            Box(new Rect(0,200,1280,520),new Color(.015f,.035f,.03f,.96f));
            string[] names = { "01 / SECTOR AMBAR - PRESION", "02 / SECTOR AZUL - SECUENCIA", "03 / SECTOR VIOLETA - CANALES" };
            Label(180,225,1000,45,names[Puzzle],heading);
            Label(180,278,930,60,"El tiempo sigue corriendo. Usa el raton para reparar el circuito.",small);
            if (Puzzle == 0)
            {
                Label(180,320,920,55,"Pista: la presion correcta es el numero de lados de un hexagono.",body);
                Label(500,395,300,60,"PRESION: " + Pressure,heading);
                if (Button(300,390,140,"- 1")) { Pressure = Mathf.Max(0,Pressure-1); SoundBank.Instance.Play("click"); }
                if (Button(820,390,140,"+ 1")) { Pressure = Mathf.Min(9,Pressure+1); SoundBank.Instance.Play("click"); }
                if (Button(420,485,440,"CONFIRMAR PRESION"))
                { if (Pressure == 6) Complete(); else { SoundBank.Instance.Play("wrong"); Notice("Presion incorrecta. Piensa en un hexagono."); } }
            }
            else if (Puzzle == 1)
            {
                Label(180,320,940,60,"Orden de arranque: HOJA > SOL > GOTA > FLOR",heading);
                string[] symbols = { "GOTA", "HOJA", "FLOR", "SOL" };
                for (int i = 0; i < 4; i++) if (Button(180+i*235,415,215,symbols[i]))
                {
                    if (i == Sequence[SequenceStep]) { SequenceStep++; SoundBank.Instance.Play("click"); if (SequenceStep == 4) Complete(); }
                    else { SequenceStep = 0; SoundBank.Instance.Play("wrong"); Notice("Orden incorrecto. La secuencia vuelve al inicio."); }
                    break;
                }
                Label(180,490,950,45,"Pasos correctos: " + SequenceStep + " / 4",body);
            }
            else if (Puzzle == 2)
            {
                Label(180,320,960,65,"Alinea cada salida con la entrada indicada: A = GOTA, B = HOJA, C = SOL.",body);
                string[] channels = { "GOTA", "HOJA", "SOL" };
                for (int i = 0; i < 3; i++) if (Button(200+i*300,415,270,((char)('A'+i)) + "  /  " + channels[Channels[i]]))
                { Channels[i] = (Channels[i] + 1) % 3; SoundBank.Instance.Play("click"); }
                if (Button(420,490,440,"CONECTAR CANALES"))
                { if (Channels[0] == 0 && Channels[1] == 1 && Channels[2] == 2) Complete(); else { SoundBank.Instance.Play("wrong"); Notice("Los canales todavia no coinciden con las entradas."); } }
            }
            if (Button(440,590,400,"CERRAR PANEL [ESC]")) ClosePuzzle();
        }
        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (flat != null) Destroy(flat);
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }
    }
}
