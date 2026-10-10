namespace Eclipse.Final
{
    public class ExtractionConsole : MissionInteractable
    {
        public override string Prompt => mission.ReactorOnline ? "[E] EXTRAER Y COMPLETAR MISIÓN" : "EXTRACCIÓN BLOQUEADA — estabiliza el reactor";
        public override void Use() { mission.Extract(); }
    }
}
