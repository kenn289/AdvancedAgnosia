using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TapCounter : MonoBehaviour
{
    public TextMeshProUGUI tapCountText;
    public Timer gameTimer;
    public Timer countDownTimer;
    private int tapCount;

    public TextMeshProUGUI infoText;

    bool gameStarted;

    void Start()
    {
        Initialise();
    }

    void Update()
    {

        if (countDownTimer.isRunning) return;
        else
        {
            if(!gameStarted)
            {
                gameStarted = true;
                gameTimer.StartTimer();
            }
           
        }

        // Check if the game time has not run out
        if (gameTimer.isRunning)
        {
            // Check for taps on the button
            if (Input.GetMouseButtonDown(0))
            {
                // Increase the tap count when the button is tapped
                tapCount++;
                UpdateUI();
            }
        }
        else
        {
            // Game time has run out, calculate and display average taps per second
            float averageTapsPerSecond = tapCount / gameTimer.duration;
            infoText.text = "Game Over! Taps: " + tapCount.ToString();
            
        }
    }

    void UpdateUI()
    {
        // Update the UI text to display the current tap count
        tapCountText.text = tapCount.ToString();
    }

    public void Initialise()
    {
        //close existing instance of gameTimer
        if (gameTimer.isRunning)
        {
            gameTimer.StopTimer();
        }
            

        tapCount = 0;
        gameStarted = false;
        infoText.text = string.Empty;
        UpdateUI();

        //When count down timer ends, Gametimer begins
        countDownTimer.StartTimer();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}