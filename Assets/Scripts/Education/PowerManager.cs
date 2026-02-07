using UnityEngine;
using System.Collections;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }
    
    [Header("Power Settings")]
    [SerializeField] private float jetpackDuration = 5f;
    [SerializeField] private float jetpackForce = 15f;
    [SerializeField] private float gravityPullDuration = 3f;
    [SerializeField] private float gravityPullRange = 5f;
    [SerializeField] private float gravityPullForce = 10f;
    
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private LayerMask pullableObjects;
    
    private PowerType activePower = PowerType.None;
    private float powerTimer;
    private bool isPowerActive;
    
    public System.Action<PowerType, float> OnPowerActivated;
    public System.Action OnPowerDeactivated;
    public System.Action<float> OnPowerTimerUpdate;
    
    public bool IsPowerActive => isPowerActive;
    public PowerType ActivePower => activePower;
    
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    
    void Start()
    {
        if (!player) player = FindObjectOfType<PlayerController>();
    }
    
    void Update()
    {
        if (isPowerActive)
        {
            powerTimer -= Time.deltaTime;
            OnPowerTimerUpdate?.Invoke(powerTimer);
            
            if (activePower == PowerType.Jetpack) ApplyJetpack();
            else if (activePower == PowerType.GravityPull) ApplyGravityPull();
            
            if (powerTimer <= 0) DeactivatePower();
        }
    }
    
    public bool CanUsePower(PowerType powerType)
    {
        return WorldManager.Instance?.IsPowerUnlocked(powerType) == true && !isPowerActive;
    }
    
    public void ActivatePower(PowerType powerType)
    {
        if (!CanUsePower(powerType)) return;
        
        activePower = powerType;
        isPowerActive = true;
        
        powerTimer = powerType switch
        {
            PowerType.Jetpack => jetpackDuration,
            PowerType.GravityPull => gravityPullDuration,
            _ => 0f
        };
        
        OnPowerActivated?.Invoke(powerType, powerTimer);
    }
    
    void ApplyJetpack()
    {
        if (player?.TryGetComponent<Rigidbody>(out Rigidbody rb) == true)
        {
            rb.AddForce(Vector3.up * jetpackForce, ForceMode.Force);
        }
    }
    
    void ApplyGravityPull()
    {
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, gravityPullRange, pullableObjects);
        foreach (var col in hitColliders)
        {
            if (col.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 direction = (player.transform.position - col.transform.position).normalized;
                rb.AddForce(direction * gravityPullForce, ForceMode.Force);
            }
        }
    }
    
    public void DeactivatePower()
    {
        isPowerActive = false;
        activePower = PowerType.None;
        OnPowerDeactivated?.Invoke();
    }
    
    public bool ShouldDisableJump()
    {
        return WorldManager.Instance?.CurrentWorld == WorldType.SciFi;
    }
}
