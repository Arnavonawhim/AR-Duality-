using UnityEngine;

// Earth World (World 2) - Physics MCQ Questions
[CreateAssetMenu(fileName = "EarthQuestions", menuName = "QuantumAcademy/Earth Questions")]
public class EarthQuestions : ScriptableObject
{
    public MCQQuestion[] GetQuestions() => new MCQQuestion[]
    {
        // Easy
        new MCQQuestion {
            questionText = "What force pulls objects toward Earth?",
            options = new[] { "Magnetism", "Friction", "Gravity", "Electricity" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Earth,
            points = 10
        },
        new MCQQuestion {
            questionText = "What unit is used to measure force?",
            options = new[] { "Kilogram", "Newton", "Meter", "Joule" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Earth,
            points = 10
        },
        new MCQQuestion {
            questionText = "If you push a box, what happens?",
            options = new[] { "It disappears", "It moves", "It grows", "Nothing" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Earth,
            points = 10
        },
        new MCQQuestion {
            questionText = "What slows down a moving object?",
            options = new[] { "Gravity", "Friction", "Light", "Sound" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.Earth,
            points = 10
        },
        
        // Medium
        new MCQQuestion {
            questionText = "Newton's First Law is also called the Law of...",
            options = new[] { "Motion", "Gravity", "Inertia", "Energy" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Earth,
            points = 20
        },
        new MCQQuestion {
            questionText = "What is the formula for Force?",
            options = new[] { "F = m/a", "F = m + a", "F = m × a", "F = a/m" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Earth,
            points = 20
        },
        new MCQQuestion {
            questionText = "What type of energy does a moving car have?",
            options = new[] { "Potential", "Kinetic", "Thermal", "Chemical" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Earth,
            points = 20
        },
        new MCQQuestion {
            questionText = "What happens to speed if you double the force on an object?",
            options = new[] { "Halves", "Stays same", "Doubles", "Triples" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.Earth,
            points = 20
        },
        
        // Hard
        new MCQQuestion {
            questionText = "What is the acceleration due to gravity on Earth?",
            options = new[] { "5.8 m/s²", "9.8 m/s²", "12.5 m/s²", "15.2 m/s²" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.Earth,
            points = 30
        },
        new MCQQuestion {
            questionText = "Newton's Third Law states that every action has...",
            options = new[] { "A delayed reaction", "No effect", "An equal and opposite reaction", "Multiple reactions" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.Earth,
            points = 30
        },
        new MCQQuestion {
            questionText = "What is the unit of work and energy?",
            options = new[] { "Newton", "Watt", "Joule", "Pascal" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.Earth,
            points = 30
        }
    };
}
