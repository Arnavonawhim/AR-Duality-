using UnityEngine;
using UnityEngine.Events;

public class AREducationController : MonoBehaviour
{
    [Header("World Settings")]
    [SerializeField] private WorldType currentWorldType;
    [SerializeField] private QuestionDatabase questionDatabase;
    
    [Header("AR Character")]
    [SerializeField] private GameObject tutorPrefab;
    [SerializeField] private Transform tutorSpawnPoint;
    [SerializeField] private float spawnDelay = 1f;
    
    [Header("Session Settings")]
    [SerializeField] private int questionsPerSession = 3;
    [SerializeField] private int pointsPerCorrect = 10;
    [SerializeField] private int bonusCompletion = 20;
    
    [Header("UI")]
    [SerializeField] private GameObject educationUI;
    [SerializeField] private TMPro.TextMeshProUGUI questionText;
    [SerializeField] private TMPro.TextMeshProUGUI pointsText;
    [SerializeField] private TMPro.TextMeshProUGUI progressText;
    [SerializeField] private GameObject powerUnlockPanel;
    [SerializeField] private TMPro.TextMeshProUGUI powerNameText;
    
    [Header("Events")]
    public UnityEvent OnSessionStarted;
    public UnityEvent OnCorrectAnswer;
    public UnityEvent OnSessionCompleted;
    public UnityEvent<PowerType> OnPowerUnlocked;
    
    private int currentQuestionIndex, correctAnswers, sessionPoints;
    private Question currentQuestion;
    private Question[] sessionQuestions;
    private bool sessionActive;
    private GameObject spawnedTutor;
    
    public bool IsSessionActive => sessionActive;
    
    void Start()
    {
        if (educationUI) educationUI.SetActive(false);
        if (powerUnlockPanel) powerUnlockPanel.SetActive(false);
        if (WorldManager.Instance != null) currentWorldType = WorldManager.Instance.CurrentWorld;
        Invoke(nameof(StartEducationSession), spawnDelay);
    }
    
    public void StartEducationSession()
    {
        if (questionDatabase == null) return;
        
        var difficulty = EducationSettings.Instance?.CurrentDifficulty ?? DifficultyLevel.Medium;
        var allQuestions = questionDatabase.GetQuestionsForDifficulty(difficulty);
        if (allQuestions.Length == 0) allQuestions = questionDatabase.questions;
        
        sessionQuestions = SelectRandomQuestions(allQuestions, questionsPerSession);
        if (sessionQuestions.Length == 0) return;
        
        currentQuestionIndex = correctAnswers = sessionPoints = 0;
        sessionActive = true;
        
        if (tutorPrefab && tutorSpawnPoint)
            spawnedTutor = Instantiate(tutorPrefab, tutorSpawnPoint.position, tutorSpawnPoint.rotation);
        
        if (educationUI) educationUI.SetActive(true);
        OnSessionStarted?.Invoke();
        AskCurrentQuestion();
    }
    
    private Question[] SelectRandomQuestions(Question[] source, int count)
    {
        count = Mathf.Min(count, source.Length);
        var selected = new Question[count];
        var shuffled = (Question[])source.Clone();
        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(i, shuffled.Length);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            selected[i] = shuffled[i];
        }
        return selected;
    }
    
    public void AskCurrentQuestion()
    {
        if (currentQuestionIndex >= sessionQuestions.Length) { CompleteSession(); return; }
        currentQuestion = sessionQuestions[currentQuestionIndex];
        if (questionText) questionText.text = currentQuestion.questionText;
        if (progressText) progressText.text = $"Question {currentQuestionIndex + 1}/{sessionQuestions.Length}";
    }
    
    public void ValidateAnswer(string studentAnswer)
    {
        if (!sessionActive || currentQuestion == null) return;
        
        bool correct = false;
        string lower = studentAnswer.ToLower();
        foreach (var kw in currentQuestion.acceptableKeywords)
            if (lower.Contains(kw.ToLower())) { correct = true; break; }
        
        if (correct)
        {
            correctAnswers++;
            sessionPoints += currentQuestion.pointValue;
            OnCorrectAnswer?.Invoke();
        }
        
        if (pointsText) pointsText.text = $"Points: {sessionPoints}";
        currentQuestionIndex++;
        
        if (currentQuestionIndex < sessionQuestions.Length)
            Invoke(nameof(AskCurrentQuestion), 2f);
        else
            Invoke(nameof(CompleteSession), 2f);
    }
    
    public void CompleteSession()
    {
        sessionActive = false;
        if (correctAnswers == sessionQuestions.Length) sessionPoints += bonusCompletion;
        
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.AddKnowledgePoints(sessionPoints, currentWorldType);
            if ((float)correctAnswers / sessionQuestions.Length >= 0.6f)
            {
                WorldManager.Instance.CompleteWorld(currentWorldType);
                ShowPowerUnlock();
            }
        }
        
        if (pointsText) pointsText.text = $"Points: {sessionPoints}";
        OnSessionCompleted?.Invoke();
    }
    
    private void ShowPowerUnlock()
    {
        var worldData = WorldManager.Instance?.GetWorldData(currentWorldType);
        if (worldData == null || worldData.powerReward == PowerType.None) return;
        if (powerUnlockPanel) powerUnlockPanel.SetActive(true);
        if (powerNameText) powerNameText.text = $"Power Unlocked: {worldData.powerName}!";
        OnPowerUnlocked?.Invoke(worldData.powerReward);
    }
    
    public string GetCurrentHint() => currentQuestion?.hint ?? "Think about what you've learned...";
    
    public void SkipQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < sessionQuestions.Length) AskCurrentQuestion();
        else CompleteSession();
    }
    
    void OnDestroy() { if (spawnedTutor) Destroy(spawnedTutor); }
}
