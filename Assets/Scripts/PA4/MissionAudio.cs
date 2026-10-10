using UnityEngine;
namespace Eclipse.Final
{
    public class MissionAudio : MonoBehaviour
    {
        public enum Signal { Repair, Reactor, Damage, Win, Lose }
        public AudioClip ambience, repair, reactor, damage, win, lose;
        public AudioSource ambientSource, cueSource;
        void Awake()
        {
            ambientSource.clip = ambience; ambientSource.loop = true; ambientSource.volume = 0.25f;
            if (ambience != null) ambientSource.Play();
        }
        public void SetPaused(bool paused) { if (paused) ambientSource.Pause(); else ambientSource.UnPause(); }
        public void Cue(Signal signal)
        {
            AudioClip clip = signal == Signal.Repair ? repair : signal == Signal.Reactor ? reactor : signal == Signal.Damage ? damage : signal == Signal.Win ? win : lose;
            if (clip != null) cueSource.PlayOneShot(clip, 0.55f);
        }
    }
}
