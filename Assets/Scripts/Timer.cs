using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float duration;
    public float timeRemaining;
    public bool isRunning = false;

    public Image timerFillImage; // Reference to the Image component for the timer sprite

    public TextMeshProUGUI timeRemainingText;

    void Update()
    {
        if (isRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                // Update the fill amount of the timer sprite based on the remaining time
                if (timerFillImage != null)
                {
                   
                    timerFillImage.fillAmount = timeRemaining / duration;
                    
                }
                if (timeRemainingText)
                {
                    timeRemainingText.text = ((int)timeRemaining).ToString();
                }
            }
            else
            {
                StopTimer();
                if (timeRemainingText)
                {
                    timeRemainingText.text = string.Empty;
                    timeRemainingText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartTimer()
    {
        timeRemaining = duration + 1f;
        isRunning = true;
    }

    public void StopTimer()
    {
        timeRemaining = 0;
        isRunning = false;

        // Reset the fill amount when the timer stops
        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = 1;
        }
    }
}
