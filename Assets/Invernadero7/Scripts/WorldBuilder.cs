using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Invernadero7
{
    public sealed class WorldBuilder : MonoBehaviour
    {
        static readonly List<ParticleSystem> jets=new List<ParticleSystem>();
        static readonly List<Renderer> indicators=new List<Renderer>();
        readonly List<Material> materials=new List<Material>();
        Transform world;
        Material floor,metal,dark,glass,soil,leaf,trunk,white,water;
        Material[] sector;
        void Awake()
        {
            jets.Clear(); indicators.Clear();
            Application.targetFrameRate=60;
            QualitySettings.vSyncCount=1; QualitySettings.pixelLightCount=2;
            gameObject.AddComponent<SoundBank>();
            gameObject.AddComponent<GameSession>();
            Build();
        }
        Material Mat(string name,Color color,bool glow=false)
        {
            Material template=Resources.Load<Material>("Surface");
            Material m=template!=null?new Material(template):new Material(Shader.Find("Standard"));
            m.name=name; m.color=color;
            m.SetFloat("_Glossiness",.22f);
            if(glow) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor",color*1.4f); }
            materials.Add(m); return m;
        }
        GameObject Shape(string name,PrimitiveType primitive,Vector3 position,Vector3 scale,Material material,bool solid=true)
        {
            GameObject go=GameObject.CreatePrimitive(primitive); go.name=name; go.transform.SetParent(world,false);
            go.transform.position=position; go.transform.localScale=scale; go.GetComponent<Renderer>().sharedMaterial=material;
            if(!solid) { go.GetComponent<Collider>().enabled=false; Destroy(go.GetComponent<Collider>()); }
            else if(name!="Suelo") go.layer=8;
            return go;
        }
        void Cube(string name,Vector3 p,Vector3 scale,Material mat,bool solid=true) => Shape(name,PrimitiveType.Cube,p,scale,mat,solid);
        void Sign(string text,Vector3 position,float yaw,float size,Color color)
        {
            GameObject go=new GameObject("Rotulo - "+text); go.transform.SetParent(world,false); go.transform.position=position; go.transform.rotation=Quaternion.Euler(0,yaw,0);
            TextMesh mesh=go.AddComponent<TextMesh>(); mesh.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); mesh.GetComponent<MeshRenderer>().sharedMaterial=mesh.font.material; mesh.text=text; mesh.fontSize=64; mesh.characterSize=size; mesh.anchor=TextAnchor.MiddleCenter; mesh.alignment=TextAlignment.Center; mesh.color=color;
        }
        void Build()
        {
            world=new GameObject("INVERNADERO 7 - escenario generado").transform;
            floor=Mat("Baldosa",new Color(.13f,.22f,.21f)); metal=Mat("Estructura",new Color(.23f,.34f,.34f)); dark=Mat("Paneles",new Color(.035f,.07f,.08f));
            glass=Mat("Cubierta",new Color(.17f,.31f,.34f)); soil=Mat("Tierra",new Color(.12f,.075f,.045f)); leaf=Mat("Vegetacion",new Color(.16f,.48f,.24f)); trunk=Mat("Tallos",new Color(.21f,.24f,.10f));
            white=Mat("Senaletica",new Color(.74f,.96f,.85f),true); water=Mat("Agua",new Color(.25f,.7f,1),true);
            sector=new[]{Mat("Sector Ambar",new Color(1,.64f,.17f),true),Mat("Sector Azul",new Color(.17f,.7f,1),true),Mat("Sector Violeta",new Color(.72f,.39f,1),true)};
            RenderSettings.ambientMode=AmbientMode.Flat; RenderSettings.ambientLight=new Color(.43f,.55f,.56f); RenderSettings.fog=true; RenderSettings.fogColor=new Color(.055f,.12f,.14f); RenderSettings.fogDensity=.012f;
            CameraClearFlags clear=CameraClearFlags.SolidColor;
            Cube("Suelo",new Vector3(0,-.25f,0),new Vector3(36,.5f,48),floor);
            Cube("Muro norte",new Vector3(0,2.8f,24),new Vector3(36,5.6f,.35f),glass);
            Cube("Muro sur",new Vector3(0,2.8f,-24),new Vector3(36,5.6f,.35f),glass);
            Cube("Muro oeste",new Vector3(-18,2.8f,0),new Vector3(.35f,5.6f,48),glass);
            Cube("Muro este",new Vector3(18,2.8f,0),new Vector3(.35f,5.6f,48),glass);
            for(int z=-24;z<=24;z+=6)
            {
                Cube("Viga cubierta",new Vector3(0,5.4f,z),new Vector3(36,.16f,.16f),metal,false);
                for(int x=-18;x<=18;x+=36) Cube("Pilar",new Vector3(x,2.7f,z),new Vector3(.24f,5.4f,.24f),metal);
            }
            for(int x=-18;x<=18;x+=6) Cube("Cubierta longitudinal",new Vector3(x,5.4f,0),new Vector3(.13f,.13f,48),metal,false);
            // Floor markings and light strips orient the player; no per-strip lights.
            for(int z=-22;z<=22;z+=2) Cube("Guia central",new Vector3(0,.012f,z),new Vector3(.07f,.02f,.75f),white,false);
            for(int k=0;k<3;k++)
            {
                float z=k==0?-10:k==1?6:14;
                float x=k==1?15:-15;
                Cube("Ruta sector",new Vector3(x/2,.015f,z),new Vector3(Mathf.Abs(x),.025f,.12f),sector[k],false);
                Terminal(k,new Vector3(x,0,z),x<0?-90:90);
            }
            foreach(float x in new[]{-8f,8f}) foreach(float z in new[]{-17f,-2f,20f}) Bed(x,z);
            for(int i=0;i<2;i++)
            {
                GameObject lamp=new GameObject("Luz direccional "+i); lamp.transform.SetParent(world); lamp.transform.rotation=Quaternion.Euler(i==0?55:35,i==0?-25:145,0);
                Light l=lamp.AddComponent<Light>(); l.type=LightType.Directional; l.color=i==0?new Color(.77f,.94f,1):new Color(.48f,.8f,.66f); l.intensity=i==0?1.1f:.4f;
                l.shadows=i==0?LightShadows.Hard:LightShadows.None;
            }
            Terminal(-1,new Vector3(0,0,-22.5f),180);
            Sign("INVERNADERO 7",new Vector3(0,4,-23.7f),180,.17f,Color.white);
            Sign("SISTEMA DE RIEGO\nREPARAR 3 CIRCUITOS",new Vector3(0,3,23.7f),0,.12f,new Color(.7f,1,.85f));
            GameObject player=new GameObject("Jugador"); player.transform.SetParent(world); player.transform.position=new Vector3(0,.1f,-19);
            CharacterController controller=player.AddComponent<CharacterController>(); controller.height=1.8f; controller.center=new Vector3(0,.9f,0); controller.radius=.35f; controller.stepOffset=.3f;
            GameObject camera=new GameObject("Camara jugador"); camera.tag="MainCamera"; camera.transform.SetParent(player.transform,false); camera.transform.localPosition=new Vector3(0,1.6f,0);
            Camera cam=camera.AddComponent<Camera>(); cam.fieldOfView=75; cam.nearClipPlane=.05f; cam.farClipPlane=110; cam.clearFlags=clear; cam.backgroundColor=RenderSettings.fogColor; camera.AddComponent<AudioListener>();
            player.AddComponent<PlayerMotor>();
            Robot(player.transform);
        }
        void Bed(float x,float z)
        {
            Cube("Macetero / cobertura",new Vector3(x,.6f,z),new Vector3(6,1.2f,5),metal);
            Cube("Tierra",new Vector3(x,1.21f,z),new Vector3(5.7f,.1f,4.7f),soil,false);
            for(int a=-1;a<=1;a++) for(int b=-1;b<=1;b++)
            {
                Vector3 p=new Vector3(x+a*1.65f,1.8f,z+b*1.4f);
                Shape("Tallo",PrimitiveType.Cylinder,p,new Vector3(.12f,.6f,.12f),trunk,false);
                for(int j=0;j<3;j++)
                {
                    var plant=Shape("Hoja",PrimitiveType.Sphere,p+new Vector3(Mathf.Sin(j*2.1f)*.35f,j*.22f,Mathf.Cos(j*2.1f)*.35f),new Vector3(.9f,.22f,.42f),leaf,false);
                    plant.transform.rotation=Quaternion.Euler(15,j*120,25);
                }
            }
        }
        void Terminal(int index,Vector3 position,float yaw)
        {
            Material accent=index<0?white:sector[index];
            var housing=Shape(index<0?"Consola central":"Terminal "+(index+1),PrimitiveType.Cube,position+Vector3.up*.8f,new Vector3(1.8f,1.6f,1),dark);
            housing.transform.rotation=Quaternion.Euler(0,yaw,0);
            var terminal=housing.AddComponent<RepairTerminal>(); terminal.circuit=index;
            // Screen faces local -Z, towards the playable area.
            Vector3 forward=Quaternion.Euler(0,yaw,0)*Vector3.back;
            var screen=Shape("Pantalla de estado",PrimitiveType.Cube,position+Vector3.up*1.3f+forward*.52f,new Vector3(1.5f,.5f,.035f),accent,false);
            screen.transform.rotation=housing.transform.rotation;
            string label=index<0?"CONSOLA CENTRAL\nACTIVAR AL FINAL":new[]{"01 - AMBAR\nPRESION","02 - AZUL\nSECUENCIA","03 - VIOLETA\nCANALES"}[index];
            Sign(label,position+Vector3.up*2.5f,yaw,.085f,accent.color);
            if(index<0) return;
            indicators.Add(screen.GetComponent<Renderer>());
            var jet=new GameObject("Riego sector "+index); jet.transform.SetParent(world); jet.transform.position=position+Vector3.up*3.6f-forward*1.2f; jet.transform.rotation=Quaternion.Euler(90,0,0);
            var ps=jet.AddComponent<ParticleSystem>(); var main=ps.main; main.playOnAwake=false; main.loop=true; main.startLifetime=1.2f; main.startSpeed=2; main.startSize=.065f; main.startColor=new Color(.35f,.8f,1,.8f); main.maxParticles=90; main.simulationSpace=ParticleSystemSimulationSpace.World;
            var emission=ps.emission; emission.rateOverTime=45;
            var shape=ps.shape; shape.shapeType=ParticleSystemShapeType.Cone; shape.angle=30; shape.radius=.2f;
            var renderer=ps.GetComponent<ParticleSystemRenderer>(); renderer.sharedMaterial=water; ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear); jets.Add(ps);
        }
        void Robot(Transform player)
        {
            var root=new GameObject("Robot de mantenimiento / IA"); root.transform.SetParent(world); root.transform.position=new Vector3(0,0,12);
            var body=Shape("Cuerpo robot",PrimitiveType.Capsule,root.transform.position+Vector3.up*.85f,new Vector3(.85f,.65f,.85f),metal,false); body.transform.SetParent(root.transform,true);
            var eye=Shape("Sensor robot",PrimitiveType.Sphere,root.transform.position+new Vector3(0,1.25f,.38f),new Vector3(.35f,.22f,.22f),water,false); eye.transform.SetParent(root.transform,true);
            for(int i=-1;i<=1;i+=2)
            {
                var wheel=Shape("Rueda",PrimitiveType.Cylinder,root.transform.position+new Vector3(i*.48f,.3f,0),new Vector3(.5f,.12f,.5f),dark,false); wheel.transform.rotation=Quaternion.Euler(0,0,90); wheel.transform.SetParent(root.transform,true);
            }
            var ai=root.AddComponent<MaintenanceRobot>(); ai.player=player; ai.eye=eye.transform; ai.status=eye.GetComponent<Renderer>();
        }
        public static void Repaired(int index)
        {
            if(index<jets.Count) jets[index].Play();
            if(index<indicators.Count) { indicators[index].material.color=new Color(.3f,1,.5f); indicators[index].material.SetColor("_EmissionColor",new Color(.3f,1,.5f)*2); }
        }
        void OnDestroy()
        {
            foreach(Material m in materials) if(m!=null) Destroy(m);
            jets.Clear(); indicators.Clear();
        }
    }
}
