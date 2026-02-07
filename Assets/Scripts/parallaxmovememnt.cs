using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax Speed")]
    public float parallaxSpeed = 0.1f;

    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        // Camera movement delta
        Vector3 delta = cam.position - lastCamPos;

        // Move layer slower than camera
        transform.position += new Vector3(delta.x * parallaxSpeed, 0f, 0f);

        // Update last position
        lastCamPos = cam.position;
    }
}
