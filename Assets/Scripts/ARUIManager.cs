using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARSessionController sessionController;

    [Header("Control Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button exitButton;

    [Header("Scale Control")]
    [SerializeField] private Slider scaleSlider;

    [Header("Display")]
    [SerializeField] private Text lightCollectibleText;
    [SerializeField] private Text antimatterCollectibleText;

    private ARRobotController robotController;

    void Start()
    {
        SetupButtonEvents(leftButton, OnLeftDown, OnButtonUp);
        SetupButtonEvents(rightButton, OnRightDown, OnButtonUp);

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitPressed);
        }

        if (scaleSlider != null)
        {
            scaleSlider.value = 0.5f;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
    }

    void Update()
    {
        if (robotController == null && sessionController != null && sessionController.IsRobotPlaced())
        {
            robotController = sessionController.GetRobotController();
        }

        UpdateCollectibleDisplay();
    }

    void SetupButtonEvents(Button button, System.Action onDown, System.Action onUp)
    {
        if (button == null) return;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers.Clear();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => onDown());
        trigger.triggers.Add(pointerDown);

        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => onUp());
        trigger.triggers.Add(pointerUp);
    }

    void OnLeftDown()
    {
        if (robotController != null) robotController.MoveLeft();
    }

    void OnRightDown()
    {
        if (robotController != null) robotController.MoveRight();
    }

    void OnButtonUp()
    {
        if (robotController != null) robotController.StopMoving();
    }

    void OnScaleChanged(float value)
    {
        if (robotController != null) robotController.SetScale(value);
    }

    void OnExitPressed()
    {
        if (robotController != null) robotController.ExitARWorld();
    }

    void UpdateCollectibleDisplay()
    {
        if (GameDataManager.Instance == null) return;

        if (lightCollectibleText != null)
        {
            lightCollectibleText.text = "Light: " + GameDataManager.Instance.LightCollectibles;
        }
        if (antimatterCollectibleText != null)
        {
            antimatterCollectibleText.text = "Antimatter: " + GameDataManager.Instance.AntimatterCollectibles;
        }
    }
}
