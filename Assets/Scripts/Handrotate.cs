using UnityEngine;
using UnityEngine.InputSystem;

public class HandRotateSwing : MonoBehaviour
{
    public float rotateAmount = 10f;
    public float rotateSpeed = 6f;

    Quaternion startRotation;

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        float move = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                move = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                move = 1f;
        }

        if (Mathf.Abs(move) > 0.01f)
        {
            float angle = Mathf.Sin(Time.time * rotateSpeed) * rotateAmount;
            transform.localRotation = startRotation * Quaternion.Euler(0, angle, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                startRotation,
                Time.deltaTime * 5f
            );
        }
    }
}
