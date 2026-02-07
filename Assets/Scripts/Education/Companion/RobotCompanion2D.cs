using UnityEngine;

public class RobotCompanion2D : MonoBehaviour
{
    [Header("Follow")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 followOffset = new Vector3(-1.5f, 1f, 0f);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float maxDistance = 8f;
    
    [Header("Hints")]
    [SerializeField] private GameObject hintBubble;
    [SerializeField] private TMPro.TextMeshProUGUI hintText;
    [SerializeField] private float hintDisplayTime = 3f;
    
    [Header("Portal Detection")]
    [SerializeField] private float portalDetectionRadius = 5f;
    [SerializeField] private LayerMask portalLayer;
    
    [Header("Visuals")]
    [SerializeField] private ParticleSystem pulseEffect;
    [SerializeField] private Renderer robotRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color nearPortalColor = new Color(0.5f, 0.8f, 1f);
    
    private bool isNearPortal;
    private float hintTimer;
    private MaterialPropertyBlock propBlock;
    
    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        if (hintBubble) hintBubble.SetActive(false);
        if (!player)
        {
            var p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
        }
    }
    
    void Update()
    {
        if (!player) return;
        FollowPlayer();
        CheckForPortals();
        UpdateHintTimer();
        UpdateVisuals();
    }
    
    void FollowPlayer()
    {
        Vector3 target = player.position + followOffset;
        float dist = Vector3.Distance(transform.position, target);
        transform.position = dist > maxDistance ? target : 
            Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
    }
    
    void CheckForPortals()
    {
        var portals = Physics.OverlapSphere(transform.position, portalDetectionRadius, portalLayer);
        bool wasNear = isNearPortal;
        isNearPortal = portals.Length > 0;
        
        if (isNearPortal && !wasNear)
        {
            ShowHint("Educational portal detected!");
            if (pulseEffect && !pulseEffect.isPlaying) pulseEffect.Play();
        }
        else if (!isNearPortal && wasNear && pulseEffect) pulseEffect.Stop();
    }
    
    void UpdateVisuals()
    {
        if (!robotRenderer) return;
        Color c = isNearPortal ? 
            Color.Lerp(normalColor, nearPortalColor, (Mathf.Sin(Time.time * 2f) + 1f) / 2f) : normalColor;
        robotRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", c);
        propBlock.SetColor("_BaseColor", c);
        robotRenderer.SetPropertyBlock(propBlock);
    }
    
    public void ShowHint(string message)
    {
        if (hintBubble && hintText)
        {
            hintText.text = message;
            hintBubble.SetActive(true);
            hintTimer = hintDisplayTime;
        }
    }
    
    void UpdateHintTimer()
    {
        if (hintTimer > 0)
        {
            hintTimer -= Time.deltaTime;
            if (hintTimer <= 0 && hintBubble) hintBubble.SetActive(false);
        }
    }
    
    public void OnEnterARMode() => gameObject.SetActive(false);
    public void OnExitARMode()
    {
        gameObject.SetActive(true);
        if (player) transform.position = player.position + followOffset;
    }
}
