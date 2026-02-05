using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    
    public Vector3 sidescrollerPosition;
    public float scaleMultiplier = 1f;
    public int lightCollectibles;
    public int antimatterCollectibles;
    
    [HideInInspector] public Vector3 arMovementDelta;
    [HideInInspector] public bool returningFromAR;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterAR(Vector3 currentPosition)
    {
        sidescrollerPosition = currentPosition;
        arMovementDelta = Vector3.zero;
        returningFromAR = false;
    }

    public void ExitAR(Vector3 totalMovement, float newScale)
    {
        arMovementDelta = totalMovement;
        scaleMultiplier = newScale;
        returningFromAR = true;
    }
}