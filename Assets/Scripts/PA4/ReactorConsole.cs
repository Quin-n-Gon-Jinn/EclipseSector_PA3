using UnityEngine;
namespace Eclipse.Final
{
    public class ReactorConsole : MissionInteractable
    {
        public ReactorInteraction reactor;
        public override string Prompt => mission.ReactorOnline ? "REACTOR ESTABLE — vuelve a extracción" : mission.Repaired < mission.requiredTerminals ? "[E] Consola bloqueada — repara los tres terminales" : "[E] ESTABILIZAR REACTOR";
        public override void Use() { if (mission.ActivateReactor()) reactor.ActivateReactor(); }
    }
}
