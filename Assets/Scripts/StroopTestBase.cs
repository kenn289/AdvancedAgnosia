using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StroopTestBase : MonoBehaviour
{
    public TextMeshProUGUI colorText;
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cellPrefab;

    public string[] colorNames;
    public Color[] inkColors;

    public int totalRows = 16;
    public int totalColumns = 11;

    public int startRow = 0;
    public int endRow = 5;
    public int startColumn = 0;
    public int endColumn = 5;

    public static StroopTestItem presentSelectedItem;

    public List<StroopTestItem> stroopTestItems;

    public List<StroopTestItem> presentDisplayedSegment;

    public Button[] navButtons;


    private void Start()
    {

      

        for (int i = 0; i < inkColors.Length; i++)
        {
            inkColors[i].a = 1;
        }

        InitialiseStroopTest();

        gridLayoutGroup.constraintCount = endColumn + 1;
        DisplayGridSegment();

       
    }

    private void Update()
    {
        //HandleInput();
    }



    IEnumerator StroopTask()
    {
        for (int i = 0; i < colorNames.Length; i++)
        {
            // Display the word in the Text UI element
            colorText.text = colorNames[i];

            // Set the ink color of the word
            colorText.color = inkColors[i];

            // Wait for a short duration (adjust as needed)
            yield return new WaitForSeconds(2f);

            // Clear the text
            colorText.text = "";

            // Wait for a short duration before the next word (adjust as needed)
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("Stroop Test completed!");
    }


    //Initialise Everything
    public void InitialiseStroopTest()
    {
        stroopTestItems = new List<StroopTestItem>();

        for (int row = 0; row < colorNames.Length; row++)
        {

                // Create a new cell
                GameObject cell = Instantiate(cellPrefab, gridLayoutGroup.transform);

                TextMeshProUGUI textMeshProUGUI = cell.GetComponentInChildren<TextMeshProUGUI>();

                // Calculate the index for the arrays
                 int arrayIndex = row;

                // Customize the cell based on Stroop test data
                textMeshProUGUI.text = colorNames[arrayIndex];
                textMeshProUGUI.color = inkColors[arrayIndex];

                StroopTestItem item = cell.GetComponent<StroopTestItem>();
                stroopTestItems.Add(item);
                cell.SetActive(false);
               
        }
    }

    void DisplayGridSegment()
    {
        //To prevent double clicks
        SetNavButtonsInteractableState(false);


        // Calculate the actual number of columns to display in this segment
        int columnsToDisplay = endColumn - startColumn + 1;

        // Set the constraint count of the GridLayoutGroup
        gridLayoutGroup.constraintCount = columnsToDisplay;

        // Calculate the total number of cells in the grid
        int totalCells = (endRow - startRow + 1) * (endColumn - startColumn + 1);

        //clear all
        if(presentDisplayedSegment.Count>0)
        {
            foreach(StroopTestItem item in presentDisplayedSegment)
            {
                item.gameObject.SetActive(false);
            }
            presentDisplayedSegment.Clear();
        }

        // Loop through the specified grid segment
        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startColumn; col <= endColumn; col++)
            {
                // Calculate the index for the arrays
                int arrayIndex = row * totalColumns + col;

                stroopTestItems[arrayIndex].gameObject.SetActive(true);

                presentDisplayedSegment.Add(stroopTestItems[arrayIndex]);

                // Wait for a short duration (adjust as needed)
                //yield return new WaitForSeconds(0f);
            }
        }

        Debug.Log("Grid segment displayed!");

        //re-enable after load
        SetNavButtonsInteractableState(true);
    }

    public void ValidateAnswer(StroopTestItem item, string answer)
    {

        Debug.Log(item.Answer + " : " + answer);

        if (string.IsNullOrEmpty(answer)) return;

        if(answer.ToLower().Trim() == item.Answer.ToLower().Trim())
        {
            item.isCorrectGuess = true;
            Debug.Log("Correct Guess: " + answer);
        }
        else
        {
            item.isCorrectGuess = false;
            Debug.Log("Incorrect Guess: " + item.Answer);
        }
        
        item.SetAnswerStatusUI(item.isCorrectGuess);
        item.UnSubscribeAllButtonEvents();
        item.isAttempted = true;
    }

    public void SetNavButtonsInteractableState(bool state)
    {
        if(navButtons!=null)
        {
            for(int i=0; i<navButtons.Length; i++)
            {
                navButtons[i].interactable = state;
            }
        }
    }


    public void MoveUp()
    {
        if (startRow > 0)
        {
            startRow -= 1;
            endRow = Mathf.Min(endRow - 1, totalRows - 1);
            //StartCoroutine(DisplayGridSegment());
            DisplayGridSegment();
        }
    }

    public void MoveDown()
    {
        if (endRow < totalRows - 1)
        {
            startRow += 1;
            endRow = Mathf.Min(endRow + 1, totalRows - 1);
            //StartCoroutine(DisplayGridSegment());
            DisplayGridSegment();
        }
    }


    public void MoveLeft()
    {
        if (startColumn > 0)
        {
            startColumn -= 1;
            endColumn = Mathf.Min(endColumn - 1, totalColumns - 1);
            //StartCoroutine(DisplayGridSegment());
            DisplayGridSegment();
        }
    }

    public void MoveRight()
    {
        if (endColumn < totalColumns - 1)
        {
            startColumn += 1;
            endColumn = Mathf.Min(endColumn + 1, totalColumns - 1);
            //StartCoroutine(DisplayGridSegment());
            DisplayGridSegment();
        }
    }

    //Test Code
    private void HandleInput()
    {
        // Handle input for navigation
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }



}

