using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldData", menuName = "QuantumAcademy/World Data")]
public class WorldData : ScriptableObject
{
    [Header("World Identity")]
    public WorldType worldType;
    public string worldName;
    [TextArea(2, 4)] public string worldDescription;
    
    [Header("Convai Character")]
    public string characterName;
    public string convaiCharacterId;
    public GameObject characterPrefab;
    
    [Header("Teaching Content")]
    [TextArea(3, 6)] public string teachingScript;
    public float teachingDuration = 90f;
    
    [Header("Progression")]
    public int knowledgePointsRequired;
    public PowerType powerReward;
    public string powerName;
    [TextArea(2, 4)] public string powerDescription;
    public string nextSceneName;
    
    [Header("World Rules")]
    public bool disableJump;
    
    [Header("Visuals")]
    public Sprite worldIcon;
    public Color themeColor = Color.white;
}
