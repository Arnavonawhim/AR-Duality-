using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wheelRotationSpeed = 300f;
    public float jumpForce = 10f;

    public Transform[] wheels;
    public Transform bodyHolder;
    public ParticleSystem jetpackFire;

    Rigidbody rb;
    bool isGrounded;

    float moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (jetpackFire != null)
        {
            jetpackFire.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    void Update()
    {
        // Movement
        moveDirection = Input.GetAxis("Horizontal");
        transform.position += Vector3.forward * moveDirection * moveSpeed * Time.deltaTime;

        // Flip
        if (moveDirection > 0)
            bodyHolder.localRotation = Quaternion.Euler(0, 0, 0);
        else if (moveDirection < 0)
            bodyHolder.localRotation = Quaternion.Euler(0, 180, 0);

        // Wheel rotation
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right * moveDirection * wheelRotationSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            if (jetpackFire != null)
            {
                jetpackFire.Play(true);
            }
        }

        // Stop fire when falling
        if (rb.linearVelocity.y < 0)
        {
            if (jetpackFire != null)
                jetpackFire.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        // Better gravity
        if (rb.linearVelocity.y > 0)
            rb.AddForce(Vector3.down * 10f);
        else if (rb.linearVelocity.y < 0)
            rb.AddForce(Vector3.down * 20f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            if (jetpackFire != null)
            {
                jetpackFire.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}
