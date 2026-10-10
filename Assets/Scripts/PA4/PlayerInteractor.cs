using UnityEngine;
using UnityEngine.InputSystem;
namespace Eclipse.Final
{
    public class PlayerInteractor : MonoBehaviour
    {
        public MissionController mission;
        public Camera view;
        public float reach = 2.8f;
        public MissionInteractable Focus { get; private set; }
        void Update()
        {
            MissionInteractable target = null;
            if (mission.State == MissionState.Playing && Physics.Raycast(view.transform.position, view.transform.forward, out RaycastHit hit, reach, ~0, QueryTriggerInteraction.Ignore))
                target = hit.collider.GetComponentInParent<MissionInteractable>();
            if (target != Focus) { if (Focus != null) Focus.LostFocus(); Focus = target; }
            if (Focus == null || Keyboard.current == null) return;
            if (Keyboard.current.eKey.wasPressedThisFrame) Focus.Use();
            Focus.TickFocused(Time.deltaTime, Keyboard.current.eKey.isPressed);
        }
    }
}
