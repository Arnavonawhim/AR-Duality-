using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public Vector3 ARMovementDelta { get; private set; }
    public float ARScaleMultiplier { get; private set; } = 1f;
    public float ScaleTimer { get; private set; }
    public int LightCollectibles { get; private set; }
    public int AntimatterCollectibles { get; private set; }
    public Vector3 PlayerPositionBeforeAR { get; private set; }
    public bool ReturningFromAR { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerPositionBeforeAR(Vector3 position)
    {
        PlayerPositionBeforeAR = position;
    }

    public void ExitAR(Vector3 movementDelta, float scaleMultiplier)
    {
        ARMovementDelta = movementDelta;
        ARScaleMultiplier = scaleMultiplier;
        ScaleTimer = 10f;
        ReturningFromAR = true;
    }

    public void AddLightCollectible()
    {
        LightCollectibles++;
    }

    public void AddAntimatterCollectible()
    {
        AntimatterCollectibles++;
    }

    public void ClearARData()
    {
        ReturningFromAR = false;
        ARMovementDelta = Vector3.zero;
    }

    public void UpdateScaleTimer(float deltaTime)
    {
        if (ScaleTimer > 0)
        {
            ScaleTimer -= deltaTime;
            if (ScaleTimer <= 0)
            {
                ARScaleMultiplier = 1f;
            }
        }
    }
}