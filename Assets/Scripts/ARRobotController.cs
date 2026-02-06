using UnityEngine;
using UnityEngine.SceneManagement;

public class ARRobotController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float maxMoveDistance = 3f;

    [Header("Scale Settings")]
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 0.5f;

    private Vector3 spawnPosition;
    private Vector3 initialScale;
    private float currentScaleMultiplier = 1f;
    private int moveDirection = 0;
    private Rigidbody rb;

    public void Initialize(Vector3 position)
    {
        spawnPosition = position;
        initialScale = transform.localScale;
        
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        transform.position = position;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, spawnPosition.y, transform.position.z);
        
        if (moveDirection != 0)
        {
            Vector3 movement = transform.forward * moveDirection * moveSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + movement;
            newPosition.y = spawnPosition.y;

            float distanceFromSpawn = Vector3.Distance(
                new Vector3(newPosition.x, 0, newPosition.z), 
                new Vector3(spawnPosition.x, 0, spawnPosition.z));
            
            if (distanceFromSpawn <= maxMoveDistance)
            {
                transform.position = newPosition;
            }
        }
    }

    public void MoveLeft()
    {
        moveDirection = 1;
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    public void MoveRight()
    {
        moveDirection = 1;
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void StopMoving()
    {
        moveDirection = 0;
    }

    public void SetScale(float sliderValue)
    {
        currentScaleMultiplier = Mathf.Lerp(minScale / initialScale.x, maxScale / initialScale.x, sliderValue);
        transform.localScale = initialScale * currentScaleMultiplier;
    }

    public Vector3 GetTotalMovement()
    {
        return transform.position - spawnPosition;
    }

    public float GetScaleMultiplier()
    {
        return currentScaleMultiplier;
    }

    public void ExitARWorld()
    {
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.ExitAR(GetTotalMovement(), currentScaleMultiplier);
        }
        SceneManager.LoadScene("MainGame");
    }
}

