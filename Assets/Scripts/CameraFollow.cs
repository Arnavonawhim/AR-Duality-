using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Position")]
    [SerializeField] private Vector3 baseOffset = new Vector3(0f, 2f, -20f);
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSpeed = 2f;
    
    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.25f;
    [SerializeField] private float lookAheadSmoothTime = 0.5f;
    
    [Header("Bounds (Optional)")]
    [SerializeField] private bool useBounds;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    
    private Vector3 velocity = Vector3.zero;
    private float currentLookAhead;
    private float lookAheadVelocity;
    private PlayerController playerController;

    void Start()
    {
        if (target != null)
        {
            playerController = target.GetComponent<PlayerController>();
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        float targetLookAhead = 0f;
        if (playerController != null)
        {
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
            {
                targetLookAhead = Mathf.Sign(rb.linearVelocity.x) * lookAheadDistance;
            }
        }
        
        currentLookAhead = Mathf.SmoothDamp(currentLookAhead, targetLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);
        
        Vector3 targetPosition = target.position + baseOffset + new Vector3(currentLookAhead, 0, 0);
        
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothTime);
    }
}
