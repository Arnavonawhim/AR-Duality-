using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestionDatabase", menuName = "QuantumAcademy/Question Database")]
public class QuestionDatabase : ScriptableObject
{
    public WorldType worldType;
    public string subject;
    public Question[] questions;
    
    public Question[] GetQuestionsForDifficulty(DifficultyLevel difficulty)
    {
        var filtered = new System.Collections.Generic.List<Question>();
        foreach (var q in questions)
            if (q.difficulty == difficulty) filtered.Add(q);
        return filtered.ToArray();
    }
    
    public Question GetRandomQuestion(DifficultyLevel difficulty)
    {
        var filtered = GetQuestionsForDifficulty(difficulty);
        return filtered.Length > 0 ? filtered[Random.Range(0, filtered.Length)] : null;
    }
}

[System.Serializable]
public class Question
{
    [TextArea(2, 4)] public string questionText;
    public string[] acceptableKeywords;
    [TextArea(2, 4)] public string expectedAnswer;
    public DifficultyLevel difficulty;
    public int pointValue = 10;
    public string hint;
    [TextArea(1, 2)] public string followUpInfo;
}
