using UnityEngine;

namespace Invernadero7
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        CharacterController controller;
        Camera view;
        float pitch, fall, footstep;
        void Start() { controller = GetComponent<CharacterController>(); view = GetComponentInChildren<Camera>(); }
        void Update()
        {
            var session = GameSession.Instance;
            if (session == null || session.State != SessionState.Playing) return;
            session.Prompt = "";
            if (session.Puzzle >= 0) return;
            if (Cursor.lockState != CursorLockMode.Locked) return;
            transform.Rotate(0,Input.GetAxisRaw("Mouse X")*2.1f,0);
            pitch = Mathf.Clamp(pitch - Input.GetAxisRaw("Mouse Y")*2.1f,-78,78);
            view.transform.localRotation = Quaternion.Euler(pitch,0,0);
            float x = (Input.GetKey(KeyCode.D)?1:0) - (Input.GetKey(KeyCode.A)?1:0);
            float z = (Input.GetKey(KeyCode.W)?1:0) - (Input.GetKey(KeyCode.S)?1:0);
            Vector3 motion = Vector3.ClampMagnitude(transform.right*x+transform.forward*z,1);
            float speed = Input.GetKey(KeyCode.LeftShift)?6.4f:3.9f;
            fall = controller.isGrounded ? -2 : Mathf.Max(-25,fall - 20*Time.deltaTime);
            controller.Move((motion*speed + Vector3.up*fall)*Time.deltaTime);
            if (motion.sqrMagnitude > .1f && controller.isGrounded && Time.time > footstep)
            { SoundBank.Instance.Play("step",.24f); footstep = Time.time + (speed > 5 ? .27f:.43f); }
            RaycastHit hit;
            if (Physics.Raycast(view.transform.position,view.transform.forward,out hit,3.5f,~0,QueryTriggerInteraction.Ignore))
            {
                var terminal = hit.collider.GetComponentInParent<RepairTerminal>();
                if (terminal != null)
                {
                    session.Prompt = terminal.Caption;
                    if (Input.GetKeyDown(KeyCode.E)) terminal.Use();
                }
            }
            if (transform.position.y < -10) { controller.enabled=false; transform.position = new Vector3(0,1.2f,-20); controller.enabled=true; }
        }
    }
}
