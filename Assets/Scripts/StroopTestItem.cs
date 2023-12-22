using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BrainCheck;
using TMPro;

public class StroopTestItem : MonoBehaviour
{

    public string Answer;
    public TextMeshProUGUI answerText;

    public Button btnComponent;

    public bool isCorrectGuess = false;

    public Image answerIndicatorImage;

    public Sprite correctSprite;
    public Sprite incorrectSprite;

    public bool isAttempted;

    int numberOfAttempts;

    public Image highlightImage;

    private void Awake()
    {

        //Subscribe to Mic Trigger event
        if(btnComponent)
        {
            btnComponent.onClick.AddListener(FindObjectOfType<MicrophoneHandler>().SpeechToTextInHiddenModeWithSound); 
        }
    }

    private void Start()
    {
        if(answerText)
        {
            if (answerText.color == Color.red)
                Answer = "red";
            else if (answerText.color == Color.green)
                Answer = "green";
            else if (answerText.color == Color.blue)
                Answer = "blue";
            else
                Answer = "yellow";

        }
        SetHighlightState(false);
    }

    public void SetAnswerStatusUI(bool isCorrect)
    {
        answerIndicatorImage.gameObject.SetActive(true);

        if(isCorrect)
        {
            answerIndicatorImage.sprite = correctSprite;
        }
        else
        {
            answerIndicatorImage.sprite = incorrectSprite;
        }
    }


    public void SetPresentSelectedItem()
    {
        if(StroopTestBase.presentSelectedItem!=null)
        {
            //Clear previously selected one
            StroopTestBase.presentSelectedItem.SetHighlightState(false);
            StroopTestBase.presentSelectedItem = null;
        }
       
        //set newly selected one
        StroopTestBase.presentSelectedItem = this;
        this.SetHighlightState(true);
    }

    public void UnSubscribeAllButtonEvents()
    {
        if (btnComponent)
        {
            btnComponent.onClick.RemoveAllListeners();
        }
    }

    public void SetHighlightState(bool state)
    {
        highlightImage.gameObject.SetActive(state);
    }

}
