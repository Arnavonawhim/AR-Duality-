using UnityEngine;

// Library World (World 3) - History MCQ Questions
[CreateAssetMenu(fileName = "LibraryQuestions", menuName = "QuantumAcademy/Library Questions")]
public class LibraryQuestions : ScriptableObject
{
    public MCQQuestion[] GetQuestions() => new MCQQuestion[]
    {
        // Easy
        new MCQQuestion {
            questionText = "Who built the Taj Mahal?",
            options = new[] { "Akbar", "Shah Jahan", "Aurangzeb", "Humayun" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Library,
            points = 10
        },
        new MCQQuestion {
            questionText = "Which civilization built the pyramids?",
            options = new[] { "Roman", "Greek", "Egyptian", "Persian" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Library,
            points = 10
        },
        new MCQQuestion {
            questionText = "Who discovered America in 1492?",
            options = new[] { "Marco Polo", "Columbus", "Magellan", "Vasco da Gama" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Library,
            points = 10
        },
        new MCQQuestion {
            questionText = "What was the capital of ancient India's Maurya Empire?",
            options = new[] { "Delhi", "Ujjain", "Pataliputra", "Varanasi" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Library,
            points = 10
        },
        
        // Medium
        new MCQQuestion {
            questionText = "When did World War II end?",
            options = new[] { "1943", "1944", "1945", "1946" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Library,
            points = 20
        },
        new MCQQuestion {
            questionText = "Who was the first Emperor of India?",
            options = new[] { "Ashoka", "Chandragupta Maurya", "Akbar", "Bindusara" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Library,
            points = 20
        },
        new MCQQuestion {
            questionText = "In which year did India gain independence?",
            options = new[] { "1945", "1946", "1947", "1948" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Library,
            points = 20
        },
        new MCQQuestion {
            questionText = "Who invented the printing press?",
            options = new[] { "Edison", "Newton", "Gutenberg", "Da Vinci" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Library,
            points = 20
        },
        
        // Hard
        new MCQQuestion {
            questionText = "The Indus Valley Civilization was located in which present-day countries?",
            options = new[] { "India only", "Pakistan only", "India and Pakistan", "India, Pakistan and Afghanistan" },
            correctIndex = 3,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.Library,
            points = 30
        },
        new MCQQuestion {
            questionText = "Who wrote 'Arthashastra'?",
            options = new[] { "Kalidasa", "Chanakya", "Valmiki", "Tulsidas" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.Library,
            points = 30
        },
        new MCQQuestion {
            questionText = "Which treaty ended World War I?",
            options = new[] { "Treaty of Paris", "Treaty of Versailles", "Treaty of Vienna", "Treaty of Westphalia" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.Library,
            points = 30
        }
    };
}
