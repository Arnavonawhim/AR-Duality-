using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float smoothTime = 0.25f;
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Follow only X axis like Hollow Knight
        Vector3 targetPos = new Vector3(
            target.position.x + offset.x,
            transform.position.y,
            offset.z
        );

        // Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
