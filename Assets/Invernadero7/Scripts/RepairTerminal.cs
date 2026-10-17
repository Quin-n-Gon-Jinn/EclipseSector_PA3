using UnityEngine;

namespace Invernadero7
{
    public sealed class RepairTerminal : MonoBehaviour
    {
        public int circuit = -1;
        public string Caption => circuit < 0 ? "[E] ACTIVAR CONSOLA CENTRAL" : "[E] REPARAR CIRCUITO " + (circuit+1);
        public void Use()
        {
            SoundBank.Instance.Play("click");
            if (circuit < 0) GameSession.Instance.ActivateCore();
            else GameSession.Instance.OpenPuzzle(circuit);
        }
    }
}
