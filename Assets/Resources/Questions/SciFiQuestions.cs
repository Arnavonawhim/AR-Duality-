using UnityEngine;

// SciFi World (World 1) - Space/Astronomy MCQ Questions
[CreateAssetMenu(fileName = "SciFiQuestions", menuName = "QuantumAcademy/SciFi Questions")]
public class SciFiQuestions : ScriptableObject
{
    public MCQQuestion[] GetQuestions() => new MCQQuestion[]
    {
        // Easy
        new MCQQuestion {
            questionText = "What is the closest planet to the Sun?",
            options = new[] { "Venus", "Mercury", "Earth", "Mars" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.SciFi,
            points = 10
        },
        new MCQQuestion {
            questionText = "How many planets are in our solar system?",
            options = new[] { "7", "8", "9", "10" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.SciFi,
            points = 10
        },
        new MCQQuestion {
            questionText = "What is the name of Earth's natural satellite?",
            options = new[] { "Titan", "Europa", "Moon", "Phobos" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.SciFi,
            points = 10
        },
        new MCQQuestion {
            questionText = "What does the Sun mainly consist of?",
            options = new[] { "Rock", "Water", "Gas (Hydrogen)", "Metal" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Easy,
            world = WorldType.SciFi,
            points = 10
        },
        
        // Medium
        new MCQQuestion {
            questionText = "What is the largest planet in our solar system?",
            options = new[] { "Saturn", "Neptune", "Jupiter", "Uranus" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.SciFi,
            points = 20
        },
        new MCQQuestion {
            questionText = "Which planet is known as the 'Red Planet'?",
            options = new[] { "Venus", "Mars", "Jupiter", "Mercury" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.SciFi,
            points = 20
        },
        new MCQQuestion {
            questionText = "What is a 'light year' a measure of?",
            options = new[] { "Time", "Distance", "Speed", "Brightness" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.SciFi,
            points = 20
        },
        new MCQQuestion {
            questionText = "Which planet has the most moons?",
            options = new[] { "Jupiter", "Saturn", "Uranus", "Neptune" },
            correctIndex = 1,
            difficulty = DifficultyLevel.Medium,
            world = WorldType.SciFi,
            points = 20
        },
        
        // Hard
        new MCQQuestion {
            questionText = "What is the name of the boundary where our solar system ends?",
            options = new[] { "Kuiper Belt", "Oort Cloud", "Heliopause", "Asteroid Belt" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.SciFi,
            points = 30
        },
        new MCQQuestion {
            questionText = "What type of galaxy is the Milky Way?",
            options = new[] { "Elliptical", "Irregular", "Spiral", "Lenticular" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.SciFi,
            points = 30
        },
        new MCQQuestion {
            questionText = "What is the escape velocity from Earth's surface?",
            options = new[] { "5.2 km/s", "7.9 km/s", "11.2 km/s", "15.1 km/s" },
            correctIndex = 2,
            difficulty = DifficultyLevel.Hard,
            world = WorldType.SciFi,
            points = 30
        }
    };
}
