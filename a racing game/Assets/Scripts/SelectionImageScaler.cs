using UnityEngine;

public class SelectionImageScaler : MonoBehaviour
{
    public Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 defaultScale = Vector3.one;
    public float scaleSpeed = 5f;

    private bool isSelected = false;

    void Update()
    {
        Vector3 targetScale = isSelected ? selectedScale : defaultScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }
}