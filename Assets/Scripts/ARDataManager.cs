using UnityEngine;

public class ARDataManager : MonoBehaviour
{
    public static ARDataManager Instance { get; private set; }
    
    public Vector3 ArMovementDelta { get; private set; }
    public float ArScale { get; private set; }
    public Vector3 VirtualStartPosition { get; private set; }
    public bool HasARData { get; private set; }
    
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
    
    public void SetVirtualStartPosition(Vector3 position)
    {
        VirtualStartPosition = position;
    }
    
    public void StoreARData(Vector3 movementDelta, float scale)
    {
        ArMovementDelta = movementDelta;
        ArScale = scale;
        HasARData = true;
    }
    
    public void ClearARData()
    {
        ArMovementDelta = Vector3.zero;
        ArScale = 1f;
        HasARData = false;
    }
}