using UnityEngine;

[System.Serializable]
public class MCQQuestion
{
    [TextArea(2, 3)] public string questionText;
    public string[] options;
    public int correctIndex;
    public DifficultyLevel difficulty;
    public WorldType world;
    public int points = 10;
}

[CreateAssetMenu(fileName = "NewQuestionDatabase", menuName = "QuantumAcademy/Question Database")]
public class QuestionDatabase : ScriptableObject
{
    public WorldType worldType;
    public string subject;
    public MCQQuestion[] mcqQuestions;
    
    public MCQQuestion[] GetQuestionsForDifficulty(DifficultyLevel difficulty)
    {
        var filtered = new System.Collections.Generic.List<MCQQuestion>();
        foreach (var q in mcqQuestions)
            if (q.difficulty == difficulty && q.world == worldType) 
                filtered.Add(q);
        return filtered.ToArray();
    }
    
    public MCQQuestion[] GetRandomQuestions(DifficultyLevel difficulty, int count)
    {
        var available = GetQuestionsForDifficulty(difficulty);
        if (available.Length <= count) return available;
        
        var selected = new MCQQuestion[count];
        var indices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < available.Length; i++) indices.Add(i);
        
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, indices.Count);
            selected[i] = available[indices[idx]];
            indices.RemoveAt(idx);
        }
        return selected;
    }
}
