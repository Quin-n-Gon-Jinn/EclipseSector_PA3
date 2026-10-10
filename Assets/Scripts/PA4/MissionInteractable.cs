using UnityEngine;
namespace Eclipse.Final
{
    public abstract class MissionInteractable : MonoBehaviour
    {
        public MissionController mission;
        public abstract string Prompt { get; }
        public abstract void Use();
        public virtual void TickFocused(float dt, bool held) { }
        public virtual void LostFocus() { }
    }
    // Concrete components live in their own files so Unity can serialize them.
}
