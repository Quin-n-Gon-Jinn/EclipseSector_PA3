using UnityEngine;
namespace Eclipse.Final
{
    public class RepairTerminal : MissionInteractable
    {
        public string terminalName = "Terminal";
        public float repairSeconds = 10f;
        public Renderer indicator;
        public ParticleSystem repairedEffect;
        public bool Repaired { get; private set; }
        public float Progress { get; private set; }
        MaterialPropertyBlock block;
        public override string Prompt => Repaired ? terminalName + " — REPARADO" : "Mantén [E] para reparar " + terminalName + " — " + Mathf.RoundToInt(100f * Progress / repairSeconds) + "%";
        public override void Use() { }
        public override void TickFocused(float dt, bool held)
        {
            if (Repaired || !held || mission.State != MissionState.Playing) return;
            Progress = Mathf.Min(repairSeconds, Progress + dt);
            if (Progress < repairSeconds) return;
            Repaired = true; mission.RepairTerminal();
            if (indicator != null)
            {
                block = new MaterialPropertyBlock(); indicator.GetPropertyBlock(block);
                block.SetColor("_BaseColor", Color.green); block.SetColor("_EmissionColor", Color.green * 3f);
                indicator.SetPropertyBlock(block);
            }
            if (repairedEffect != null) repairedEffect.Play();
        }
    }
}
