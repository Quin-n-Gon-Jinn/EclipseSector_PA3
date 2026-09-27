using UnityEngine;
using UnityEngine.Playables;

public class IntroCinematicController : MonoBehaviour
{
    public PlayableDirector director;
    public SimpleFPSController playerController;
    public GameObject introCameraObject;
    public Camera mainCamera;

    private void Start()
    {
        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        if (director != null)
        {
            director.stopped += OnDirectorStopped;
            director.Play();
        }
    }

    private void OnDirectorStopped(PlayableDirector d)
    {
        FinishIntro();
    }

    private void FinishIntro()
    {
        if (introCameraObject != null)
            introCameraObject.SetActive(false);

        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = new Vector3(0f, 1.62f, 0f);
            mainCamera.transform.localRotation = Quaternion.identity;
        }

        if (playerController != null)
            playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnDirectorStopped;
    }
}
