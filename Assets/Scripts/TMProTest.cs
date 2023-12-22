using UnityEngine;
using TMPro;

public class TMProText : MonoBehaviour
{
    public GameObject textMeshProPrefab; // Assign your TextMeshPro prefab in the Unity Editor

    void Start()
    {
        // Instantiate TextMeshPro prefab
        GameObject textMeshProObject = Instantiate(textMeshProPrefab, Vector3.zero, Quaternion.identity);

        // Get the TextMeshPro component from the instantiated object
        TextMeshProUGUI textMeshProComponent = textMeshProObject.GetComponent<TextMeshProUGUI>();

        if (textMeshProComponent != null)
        {
            // Set the text
            textMeshProComponent.text = "TestText";
        }
        else
        {
            Debug.LogError("TextMeshPro component not found on the instantiated object.");
        }
    }
}