using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float smoothTime = 0.25f;
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Follow Limits")]
    public float followThresholdX = 1.5f; // distance before camera starts moving

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        float cameraX = transform.position.x;
        float targetX = target.position.x + offset.x;

        // Only move camera if player goes beyond threshold
        if (Mathf.Abs(targetX - cameraX) > followThresholdX)
        {
            float newX = Mathf.Lerp(
                cameraX,
                targetX,
                Time.deltaTime / smoothTime
            );

            transform.position = new Vector3(
                newX,
                transform.position.y,
                offset.z
            );
        }
    }
}
