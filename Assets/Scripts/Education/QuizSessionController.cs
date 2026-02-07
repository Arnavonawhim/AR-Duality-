using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizSessionController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject quizUI;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TextMeshProUGUI[] optionTexts;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button continueButton;
    
    [Header("Question Database")]
    [SerializeField] private QuestionDatabase questionDatabase;
    
    [Header("Settings")]
    [SerializeField] private int questionsToAsk = 3;
    [SerializeField] private int questionsToPass = 2;
    [SerializeField] private int pointsOnPass = 30;
    
    private MCQQuestion[] currentQuestions;
    private int currentQuestionIndex;
    private int correctAnswers;
    
    void Start()
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
        
        if (retryButton) retryButton.onClick.AddListener(StartQuizSession);
        if (continueButton) continueButton.onClick.AddListener(OnContinue);
        
        if (quizUI) quizUI.SetActive(false);
    }
    
    public void StartQuizSession()
    {
        if (quizUI) quizUI.SetActive(true);
        if (retryButton) retryButton.gameObject.SetActive(false);
        if (continueButton) continueButton.gameObject.SetActive(false);
        if (resultText) resultText.gameObject.SetActive(false);
        
        var difficulty = EducationSettings.Instance?.CurrentDifficulty ?? DifficultyLevel.Medium;
        
        if (questionDatabase)
            currentQuestions = questionDatabase.GetRandomQuestions(difficulty, questionsToAsk);
        
        if (currentQuestions == null || currentQuestions.Length == 0)
        {
            Debug.LogError("No questions found for quiz!");
            return;
        }
        
        currentQuestionIndex = 0;
        correctAnswers = 0;
        
        ShowQuestion();
    }
    
    void ShowQuestion()
    {
        if (currentQuestionIndex >= currentQuestions.Length)
        {
            ShowResults();
            return;
        }
        
        MCQQuestion q = currentQuestions[currentQuestionIndex];
        
        if (questionText) questionText.text = q.questionText;
        if (progressText) progressText.text = $"Question {currentQuestionIndex + 1} / {currentQuestions.Length}";
        
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < q.options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].interactable = true;
                optionButtons[i].GetComponent<Image>().color = Color.white;
                optionTexts[i].text = q.options[i];
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }
    
    void OnOptionSelected(int optionIndex)
    {
        MCQQuestion q = currentQuestions[currentQuestionIndex];
        
        // Disable buttons
        foreach (var btn in optionButtons) btn.interactable = false;
        
        // Check answer
        if (optionIndex == q.correctIndex)
        {
            correctAnswers++;
            optionButtons[optionIndex].GetComponent<Image>().color = Color.green;
        }
        else
        {
            optionButtons[optionIndex].GetComponent<Image>().color = Color.red;
            if (q.correctIndex < optionButtons.Length)
                optionButtons[q.correctIndex].GetComponent<Image>().color = Color.green;
        }
        
        Invoke(nameof(NextQuestion), 1.5f);
    }
    
    void NextQuestion()
    {
        currentQuestionIndex++;
        ShowQuestion();
    }
    
    void ShowResults()
    {
        foreach (var btn in optionButtons) btn.gameObject.SetActive(false);
        if (questionText) questionText.text = "";
        
        bool passed = correctAnswers >= questionsToPass;
        
        if (resultText)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = passed 
                ? $"PASSED!\n{correctAnswers}/{currentQuestions.Length} Correct"
                : $"FAILED\n{correctAnswers}/{currentQuestions.Length} Correct\nNeed {questionsToPass} to pass";
            resultText.color = passed ? Color.green : Color.red;
        }
        
        if (passed)
        {
            if (WorldManager.Instance != null)
            {
                WorldManager.Instance.CompleteWorld(WorldManager.Instance.CurrentWorld);
                WorldManager.Instance.AddKnowledgePoints(pointsOnPass);
            }
            if (continueButton) continueButton.gameObject.SetActive(true);
        }
        else
        {
            if (retryButton) retryButton.gameObject.SetActive(true);
        }
    }
    
    void OnContinue()
    {
        ARSessionManager.Instance?.GoToNextWorld();
    }
}
