using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wheelRotationSpeed = 300f;
    public float jumpForce = 10f;

    public Transform[] wheels;
    public Transform bodyHolder;   // assign BodyHolder here

    Rigidbody rb;
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // ✅ Always move in world Z direction
        transform.position += Vector3.forward * move * moveSpeed * Time.deltaTime;

        // ✅ Flip visuals only
        if (move > 0)
            bodyHolder.localRotation = Quaternion.Euler(0, 0, 0);
        else if (move < 0)
            bodyHolder.localRotation = Quaternion.Euler(0, 180, 0);

        // Rotate wheels
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right * move * wheelRotationSpeed * Time.deltaTime);
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Better gravity
        if (rb.linearVelocity.y > 0)
            rb.AddForce(Vector3.down * 15f);
        else if (rb.linearVelocity.y < 0)
            rb.AddForce(Vector3.down * 30f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
