using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldData", menuName = "QuantumAcademy/World Data")]
public class WorldData : ScriptableObject
{
    [Header("World Identity")]
    public WorldType worldType;
    public string worldName;
    [TextArea(2, 4)] public string worldDescription;
    
    [Header("AR Tutor")]
    public string tutorName;
    [TextArea(2, 4)] public string tutorIntroduction;
    public string convaiCharacterId;
    
    [Header("Visuals")]
    public Sprite worldIcon;
    public Color worldThemeColor = Color.white;
    
    [Header("Progression")]
    public int knowledgePointsRequired;
    public PowerType powerReward;
    public string powerName;
    [TextArea(2, 4)] public string powerDescription;
    
    [Header("Educational Content")]
    public string[] topicsCovered;
    
    [Header("Scene References")]
    public string arSceneName;
    public string platformerSceneName;
}
