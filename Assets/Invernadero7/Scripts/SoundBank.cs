using System.Collections.Generic;
using UnityEngine;

namespace Invernadero7
{
    // All sources and clips are created once. Sound events reuse a single source.
    public sealed class SoundBank : MonoBehaviour
    {
        public static SoundBank Instance { get; private set; }
        readonly Dictionary<string,AudioClip> clips = new Dictionary<string,AudioClip>();
        AudioSource effects, ambient;
        void Awake()
        {
            Instance=this;
            effects=gameObject.AddComponent<AudioSource>(); effects.spatialBlend=0;
            ambient=gameObject.AddComponent<AudioSource>(); ambient.loop=true; ambient.volume=.22f;
            clips["click"]=Tone("click",660,.12f);
            clips["wrong"]=Tone("wrong",150,.28f);
            clips["success"]=Tone("success",520,.65f);
            clips["damage"]=Tone("damage",80,.4f);
            clips["step"]=Tone("step",92,.065f);
            clips["alert"]=Tone("alert",880,.25f);
            int rate=22050; float[] data=new float[rate*8];
            var rng=new System.Random(7);
            for(int i=0;i<data.Length;i++)
            {
                float t=i/(float)rate;
                data[i]=.12f*Mathf.Sin(2*Mathf.PI*110*t)+.07f*Mathf.Sin(2*Mathf.PI*165*t)+.04f*Mathf.Sin(2*Mathf.PI*220*t)+((float)rng.NextDouble()-.5f)*.035f;
            }
            ambient.clip=AudioClip.Create("Ambiente generado - ventilacion y armonia",data.Length,1,rate,false); ambient.clip.SetData(data,0); ambient.Play();
        }
        AudioClip Tone(string name,float hz,float duration)
        {
            int rate=22050; float[] data=new float[(int)(rate*duration)];
            for(int i=0;i<data.Length;i++)
            {
                float t=i/(float)rate, envelope=Mathf.Sin(Mathf.PI*i/(data.Length-1));
                float frequency=name=="success"?hz*(1+Mathf.Floor(t/.16f)*.25f):hz;
                data[i]=Mathf.Sin(2*Mathf.PI*frequency*t)*envelope*.28f;
            }
            AudioClip clip=AudioClip.Create(name,data.Length,1,rate,false); clip.SetData(data,0); return clip;
        }
        public void Play(string id,float volume=1) { if(effects!=null && clips.ContainsKey(id)) effects.PlayOneShot(clips[id],volume); }
        void OnDestroy() { foreach(var clip in clips.Values) Destroy(clip); if(ambient!=null && ambient.clip!=null) Destroy(ambient.clip); if(Instance==this) Instance=null; }
    }
}
