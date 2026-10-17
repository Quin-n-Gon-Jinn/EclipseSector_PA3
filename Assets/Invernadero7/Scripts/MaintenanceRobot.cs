using System.Collections.Generic;
using UnityEngine;

namespace Invernadero7
{
    public enum RobotState { Patrol, Chase, Search }
    public sealed class MaintenanceRobot : MonoBehaviour
    {
        public Transform player, eye;
        public RobotState State { get; private set; }
        public Renderer status;
        GridNavigator navigator;
        List<Vector3> route=new List<Vector3>();
        readonly Vector3[] patrol={new Vector3(0,0,18),new Vector3(-12,0,14),new Vector3(-12,0,-10),new Vector3(0,0,-14),new Vector3(12,0,6),new Vector3(0,0,6)};
        int waypoint,pathIndex;
        float detectionAt,repathAt,lastSeenAt,searchUntil;
        Vector3 lastSeen;
        Material statusMaterial;
        void Start() { navigator=new GridNavigator(); statusMaterial=status.material; }
        void Update()
        {
            var session=GameSession.Instance;
            if(session==null || session.State!=SessionState.Playing || player==null) return;
            if(Time.time>=detectionAt)
            {
                detectionAt=Time.time+.2f;
                Vector3 delta=(player.position+Vector3.up)-eye.position;
                bool visible=delta.sqrMagnitude<144 && !Physics.Raycast(eye.position,delta.normalized,delta.magnitude,1<<8,QueryTriggerInteraction.Ignore);
                if(visible)
                {
                    if(State!=RobotState.Chase) { SoundBank.Instance.Play("alert",.6f); session.Notice("Robot: objetivo detectado. Usa los maceteros como cobertura.",2); }
                    State=RobotState.Chase; lastSeen=player.position; lastSeenAt=Time.time;
                }
                else if(State==RobotState.Chase && Time.time-lastSeenAt>2)
                { State=RobotState.Search; searchUntil=Time.time+4; }
                else if(State==RobotState.Search && Time.time>searchUntil)
                { State=RobotState.Patrol; }
                statusMaterial.color=State==RobotState.Chase?new Color(1,.2f,.12f):State==RobotState.Search?Color.yellow:new Color(.35f,.9f,1);
                statusMaterial.SetColor("_EmissionColor",statusMaterial.color*2);
            }
            Vector3 target=State==RobotState.Patrol?patrol[waypoint]:lastSeen;
            if(State==RobotState.Patrol && Vector3.Distance(new Vector3(transform.position.x,0,transform.position.z),target)<.6f)
            { waypoint=(waypoint+1)%patrol.Length; target=patrol[waypoint]; repathAt=0; }
            if(Time.time>=repathAt) { route=navigator.Find(transform.position,target); pathIndex=0; repathAt=Time.time+.5f; }
            if(pathIndex<route.Count)
            {
                Vector3 destination=route[pathIndex]; destination.y=transform.position.y;
                Vector3 delta=destination-transform.position;
                if(delta.magnitude<.15f) pathIndex++;
                else
                {
                    float speed=State==RobotState.Chase?3.35f:2f;
                    transform.position=Vector3.MoveTowards(transform.position,destination,speed*Time.deltaTime);
                    transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(delta),Time.deltaTime*7);
                }
            }
            Vector3 separation=player.position-transform.position; separation.y=0;
            if(separation.sqrMagnitude<2f) session.Damage(25);
            if(eye!=null) eye.localPosition=new Vector3(0,1.25f+Mathf.Sin(Time.time*3)*.04f,.38f);
        }
        void OnDestroy() { if(statusMaterial!=null) Destroy(statusMaterial); }
    }
}
