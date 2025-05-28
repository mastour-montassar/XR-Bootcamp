using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class TimerManager : MonoBehaviour
{
    public float gameDuration = 60f; // Game duration in seconds
    public Text timerText; // UI text to display the timer
    public GameObject gameOverPanel; // Panel to display when the game ends
    public GameObject Panel;
    public GameObject openingPanel; // The opening menu panel
    public GameObject[] targets; // Array of targets to activate
    public Text finalScoreText; // UI text to display the final score
    public Text winLoseText; // UI text to display win/lose message
    private float timeRemaining;
    private bool isGameOver;
    private bool isGameStarted = false; // To track if the game has started
    private ScoreManager scoreManager;
    public XRInteractorLineVisual leftControllerLine;
    public XRInteractorLineVisual rightControllerLine;
    private int scoreMultiplier = 0; // Default score multiplier for "Normal"
    private float additionalTime = 0f; // Default additional time
    private int limitscore = 10;

    private void Start()
    {
        timeRemaining = gameDuration;
        isGameOver = false;
        isGameStarted = false; // Set the game as not started
        gameOverPanel.SetActive(false);
        Panel.SetActive(false);
        openingPanel.SetActive(true);
        scoreManager = FindObjectOfType<ScoreManager>();
        rightControllerLine.enabled = true;
        leftControllerLine.enabled = true;

        // Ensure all targets are inactive at the start
        foreach (GameObject target in targets)
        {
            target.SetActive(false);
        }
    }

    private void Update()
    {
        if (isGameOver || !isGameStarted) return; // Only update timer if the game has started

        // Update the timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            EndGame();
        }

        // Display the timer in minutes and seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        if (timeRemaining <= 10)
        {
            timerText.color = Color.red;
        }

        timerText.text = $"Time: {minutes:D2}:{seconds:D2}";
    }

    private void EndGame()
    {
        isGameOver = true;
        
        gameOverPanel.SetActive(true);
        Panel.SetActive(false);
        if (leftControllerLine != null)
        {
            leftControllerLine.enabled = true;
        }
        if (rightControllerLine != null)
        {
            rightControllerLine.enabled = true;
        }
        foreach (GameObject target in targets)
        {
            target.SetActive(false);
        }
        // Display final score
        finalScoreText.text = "Final Score: " + scoreManager.GetScore();

        // Check if player wins or loses (customize this condition as needed)
        if (scoreManager.GetScore() >= limitscore) // Example: Winning if score is 100 or more
        {
            winLoseText.text = "You Win!";
        }
        else
        {
            winLoseText.text = "You Lose!";
        }

        // Pause the game
        Time.timeScale = 0f;
    }

    public void ReplayLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current level
    }

    // Call this method for the Normal difficulty
    public void StartNormalGame()
    {
        additionalTime = 0f;
        BeginGame();
    }

    // Call this method for the Medium difficulty
    public void StartMediumGame()
    {
        additionalTime = 10f; // Add 10 extra seconds
        limitscore += 20; // Add 50 to the score
        BeginGame();
    }

    // Call this method for the Hard difficulty
    public void StartHardGame()
    {
        additionalTime = 10f; // Add 20 extra seconds
        limitscore += 100; // Add 100 to the score
        BeginGame();
    }

    public void BeginGame()
    {
        rightControllerLine.enabled = false;
        leftControllerLine.enabled = false;

        // Adjust the game settings based on the selected difficulty
        timeRemaining = gameDuration + additionalTime;
        scoreManager.AddScore(scoreMultiplier); // Assuming ScoreManager has a method to add score

        // Mark the game as started
        isGameStarted = true;

        // Hide the opening panel and start the game
        openingPanel.SetActive(false);
        Panel.SetActive(true);

        // Activate all targets
        foreach (GameObject target in targets)
        {
            target.SetActive(true);

            // Optional: Trigger the target activation logic if needed
            TargetBehavior targetBehavior = target.GetComponent<TargetBehavior>();
            if (targetBehavior != null)
            {
                targetBehavior.LightUp();
            }
        }

        // Resume the game
        Time.timeScale = 1f;
    }
}
