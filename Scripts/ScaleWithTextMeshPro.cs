using TMPro;
using UnityEngine;

public class ScaleWithTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI tmpText; // Reference to the TextMeshPro component
    public RectTransform rectTransform; // Reference to the RectTransform

    void FixedUpdate()
    {
        // Update the size of the RectTransform based on the text's preferred size
        Vector2 newSize = new Vector2(tmpText.preferredWidth, tmpText.preferredHeight);
        rectTransform.sizeDelta = newSize;
    }
}
