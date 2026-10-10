using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Eclipse.Final
{
    public enum MissionState { Briefing, Playing, Paused, Won, Lost }
    [DefaultExecutionOrder(-100)]
    public class MissionController : MonoBehaviour
    {
        public SimpleFPSController player;
        public MissionAudio audioSystem;
        public float timeLimit = 300f;
        public int requiredTerminals = 3;
        public MissionState State { get; private set; } = MissionState.Briefing;
        public int Repaired { get; private set; }
        public bool ReactorOnline { get; private set; }
        public float Remaining { get; private set; }
        public int Health { get; private set; } = 100;
        public string Message { get; private set; } = "Repara los tres terminales para estabilizar el reactor.";
        public event Action Changed;
        float nextDamage, messageUntil;
        void Awake() { Time.timeScale = 1f; Remaining = timeLimit; }
        void Start() { SetState(MissionState.Briefing); }
        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (State == MissionState.Playing) SetState(MissionState.Paused);
                else if (State == MissionState.Paused) SetState(MissionState.Playing);
            }
            if (State != MissionState.Playing) return;
            Remaining = Mathf.Max(0f, Remaining - Time.deltaTime);
            if (Remaining <= 0f) Lose("Tiempo agotado. El reactor ha colapsado.");
            else if (player.transform.position.y < -8f) Lose("Has caído fuera de la instalación.");
            if (messageUntil > 0f && Time.time > messageUntil) { Message = ""; messageUntil = 0f; Changed?.Invoke(); }
        }
        public void Begin() { if (State == MissionState.Briefing) SetState(MissionState.Playing); }
        public void Resume() { if (State == MissionState.Paused) SetState(MissionState.Playing); }
        void SetState(MissionState state)
        {
            State = state;
            bool playing = state == MissionState.Playing;
            player.enabled = playing;
            Time.timeScale = playing ? 1f : 0f;
            Cursor.lockState = playing ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !playing;
            if (audioSystem != null) audioSystem.SetPaused(!playing);
            Changed?.Invoke();
        }
        public void Notify(string message)
        { Message = message; messageUntil = Time.time + 4f; Changed?.Invoke(); }
        public void RepairTerminal()
        {
            if (State != MissionState.Playing || Repaired >= requiredTerminals) return;
            Repaired++; audioSystem.Cue(MissionAudio.Signal.Repair);
            Notify(Repaired == requiredTerminals ? "Terminales listos. Activa la consola del reactor." : "Terminal reparado. Busca el siguiente.");
        }
        public bool ActivateReactor()
        {
            if (State != MissionState.Playing || ReactorOnline) return false;
            if (Repaired < requiredTerminals) { Notify("Faltan terminales por reparar."); return false; }
            ReactorOnline = true; audioSystem.Cue(MissionAudio.Signal.Reactor);
            Notify("Reactor estable. Regresa al punto de extracción de la sala inicial."); return true;
        }
        public void Extract()
        {
            if (State != MissionState.Playing) return;
            if (!ReactorOnline) { Notify("Primero estabiliza el reactor."); return; }
            audioSystem.Cue(MissionAudio.Signal.Win); SetState(MissionState.Won);
        }
        public void Damage(int amount)
        {
            if (State != MissionState.Playing || Time.time < nextDamage) return;
            nextDamage = Time.time + 1.5f; Health = Mathf.Max(0, Health - amount);
            audioSystem.Cue(MissionAudio.Signal.Damage);
            Notify("El centinela te ha alcanzado. Aléjate y corta su línea de visión.");
            if (Health <= 0) Lose("El centinela ha neutralizado al operador.");
        }
        public void Lose(string reason)
        { if (State != MissionState.Playing) return; Message = reason; audioSystem.Cue(MissionAudio.Signal.Lose); SetState(MissionState.Lost); }
        public void Restart() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        void OnDestroy() { Time.timeScale = 1f; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }
}
