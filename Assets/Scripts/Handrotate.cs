using UnityEngine;

public class HandRotateSwing : MonoBehaviour
{
    public float rotateAmount = 10f;   // degrees of swing
    public float rotateSpeed = 6f;     // how fast it swings

    Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        if (Mathf.Abs(move) > 0.01f)
        {
            float angle = Mathf.Sin(Time.time * rotateSpeed) * rotateAmount;

            // ✅ Rotate around Z axis
            transform.localRotation =
                startRotation * Quaternion.Euler(0, angle, 0);
        }
        else
        {
            transform.localRotation =
                Quaternion.Lerp(transform.localRotation, startRotation, Time.deltaTime * 5f);
        }
    }
}
