using UnityEngine;
using UnityEngine.AI;
namespace Eclipse.Final
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SentinelAI : MonoBehaviour
    {
        public enum Behaviour { Patrol, Chase, Search }
        public MissionController mission;
        public Transform player;
        public Transform[] waypoints;
        public Renderer indicator;
        public float detectionRange = 9f;
        public float decisionInterval = 0.2f;
        public int Decisions { get; private set; }
        public Behaviour State { get; private set; }
        NavMeshAgent agent;
        int waypoint;
        float nextDecision, lastSeenTime;
        Vector3 lastSeen;
        MaterialPropertyBlock block;
        void Awake() { agent = GetComponent<NavMeshAgent>(); block = new MaterialPropertyBlock(); }
        void Update()
        {
            if (!agent.isOnNavMesh) return;
            bool active = mission.State == MissionState.Playing;
            agent.isStopped = !active;
            if (!active || Time.time < nextDecision) return;
            nextDecision = Time.time + Mathf.Max(0.05f, decisionInterval); Decisions++;
            Vector3 eye = transform.position + Vector3.up * 1.4f;
            Vector3 target = player.position + Vector3.up * 1.1f;
            float distance = Vector3.Distance(eye, target);
            bool visible = false;
            if (distance < detectionRange && Physics.Raycast(eye, (target-eye).normalized, out RaycastHit hit, distance + 0.2f, ~0, QueryTriggerInteraction.Ignore))
                visible = hit.transform == player || hit.transform.IsChildOf(player);
            if (visible)
            {
                State = Behaviour.Chase; lastSeen = player.position; lastSeenTime = Time.time;
                agent.speed = 3.8f; SetDestination(lastSeen);
                if (distance < 1.65f) mission.Damage(20);
            }
            else if (State != Behaviour.Patrol && Time.time - lastSeenTime < 3f)
            { State = Behaviour.Search; agent.speed = 2.6f; SetDestination(lastSeen); }
            else
            {
                State = Behaviour.Patrol; agent.speed = 2.2f;
                if (waypoints != null && waypoints.Length > 0)
                {
                    if (!agent.pathPending && agent.hasPath && agent.remainingDistance < 0.7f) waypoint = (waypoint + 1) % waypoints.Length;
                    SetDestination(waypoints[waypoint].position);
                }
            }
            if (indicator != null)
            {
                Color c = State == Behaviour.Chase ? Color.red : State == Behaviour.Search ? Color.yellow : Color.cyan;
                indicator.GetPropertyBlock(block); block.SetColor("_EmissionColor", c * 2f); indicator.SetPropertyBlock(block);
            }
        }
        void SetDestination(Vector3 target)
        {
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 2f, NavMesh.AllAreas) && (!agent.hasPath || (agent.destination-hit.position).sqrMagnitude > 0.25f))
                agent.SetDestination(hit.position);
        }
    }
}
