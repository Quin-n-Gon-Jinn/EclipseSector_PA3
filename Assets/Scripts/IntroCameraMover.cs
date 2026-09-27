using UnityEngine;

public class IntroCameraMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0f, 1.8f, -3.5f);
    public Vector3 endPosition   = new Vector3(0f, 1.8f, 3.5f);

    public Vector3 startEuler = new Vector3(0f, 0f, 0f);
    public Vector3 endEuler   = new Vector3(0f, 0f, 0f);

    public float duration = 7f;

    private float elapsed;

    private void OnEnable()
    {
        elapsed = 0f;
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startEuler);
    }

    private void Update()
    {
        if (duration <= 0f) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Smooth movement so the shot starts/ends gently.
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);
        transform.rotation = Quaternion.Slerp(
            Quaternion.Euler(startEuler),
            Quaternion.Euler(endEuler),
            smoothT
        );
    }
}
