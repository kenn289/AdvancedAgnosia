using UnityEngine;
using UnityEngine.UI;

public class DynamicGridLayout : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public RectTransform cellPrefab;
    public int fixedColumnCount = 5;
    public Vector2 maxCellSize = new Vector2(300f, 100f);

    private void AddCell()
    {
        // Instantiate a new cell prefab
        RectTransform newCell = Instantiate(cellPrefab, gridLayoutGroup.transform);

        // Calculate the desired size based on the current number of cells
        float scaleFactor = Mathf.Clamp01((float)gridLayoutGroup.transform.childCount / fixedColumnCount);
        Vector2 desiredSize = Vector2.Lerp(Vector2.one, maxCellSize, scaleFactor);

        // Set the size of the new cell
        newCell.sizeDelta = desiredSize;

        // Update the layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());
    }
}
