using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }
    
    [Header("Durations")]
    [SerializeField] private float gravityDuration = 10f;
    [SerializeField] private float timeDuration = 8f;
    [SerializeField] private float floatDuration = 10f;
    
    [Header("Gravity Power")]
    [SerializeField] private float heavyScale = 2f;
    [SerializeField] private float lightScale = 0.5f;
    [SerializeField] private float lightJumpMultiplier = 1.5f;
    
    [Header("Time Power")]
    [SerializeField] private float slowMotionScale = 0.3f;
    
    [Header("Float Power")]
    [SerializeField] private float floatGravityScale = 0.2f;
    
    private PowerType activePower = PowerType.None;
    private float powerTimer = 0f;
    private bool isHeavy = false;
    private PlayerController playerController;
    private Rigidbody playerRb;
    private Vector3 playerBaseScale;
    
    public System.Action<PowerType> OnPowerActivated;
    public System.Action<PowerType> OnPowerDeactivated;
    public System.Action<float, float> OnPowerTimerUpdate;
    
    public PowerType ActivePower => activePower;
    public float PowerTimeRemaining => powerTimer;
    public bool IsHeavy => isHeavy;
    
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start() => FindPlayer();
    
    void Update() { if (activePower != PowerType.None) UpdatePowerTimer(); }
    
    public void FindPlayer()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            playerRb = player.GetComponent<Rigidbody>();
            playerBaseScale = player.transform.localScale;
        }
    }
    
    public bool CanActivatePower(PowerType power)
    {
        if (power == PowerType.None || activePower != PowerType.None) return false;
        return WorldManager.Instance != null && WorldManager.Instance.IsPowerUnlocked(power);
    }
    
    public bool ActivatePower(PowerType power)
    {
        if (!CanActivatePower(power)) return false;
        
        activePower = power;
        switch (power)
        {
            case PowerType.GravityManipulation:
                powerTimer = gravityDuration;
                ToggleGravityManipulation();
                break;
            case PowerType.TimeGlimpse:
                powerTimer = timeDuration;
                Time.timeScale = slowMotionScale;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                break;
            case PowerType.ZeroGFloat:
                powerTimer = floatDuration;
                Physics.gravity = new Vector3(0, -9.81f * floatGravityScale, 0);
                break;
        }
        OnPowerActivated?.Invoke(power);
        return true;
    }
    
    public void DeactivatePower()
    {
        if (activePower == PowerType.None) return;
        PowerType wasActive = activePower;
        
        switch (activePower)
        {
            case PowerType.GravityManipulation:
                isHeavy = false;
                if (playerController != null) playerController.transform.localScale = playerBaseScale;
                break;
            case PowerType.TimeGlimpse:
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                break;
            case PowerType.ZeroGFloat:
                Physics.gravity = new Vector3(0, -9.81f, 0);
                break;
        }
        
        activePower = PowerType.None;
        powerTimer = 0f;
        OnPowerDeactivated?.Invoke(wasActive);
    }
    
    public void ToggleGravityManipulation()
    {
        if (activePower != PowerType.GravityManipulation) return;
        isHeavy = !isHeavy;
        if (playerController != null)
            playerController.transform.localScale = playerBaseScale * (isHeavy ? heavyScale : lightScale);
    }
    
    public float GetJumpMultiplier() => 
        (activePower == PowerType.GravityManipulation && !isHeavy) ? lightJumpMultiplier : 1f;
    
    private void UpdatePowerTimer()
    {
        float dt = activePower == PowerType.TimeGlimpse ? Time.unscaledDeltaTime : Time.deltaTime;
        powerTimer -= dt;
        
        float max = activePower switch
        {
            PowerType.GravityManipulation => gravityDuration,
            PowerType.TimeGlimpse => timeDuration,
            PowerType.ZeroGFloat => floatDuration,
            _ => 1f
        };
        OnPowerTimerUpdate?.Invoke(powerTimer, max);
        if (powerTimer <= 0) DeactivatePower();
    }
    
    public string GetPowerName(PowerType power) => power switch
    {
        PowerType.GravityManipulation => "Gravity Manipulation",
        PowerType.TimeGlimpse => "Time Glimpse",
        PowerType.ZeroGFloat => "Zero-G Float",
        _ => "None"
    };
}
