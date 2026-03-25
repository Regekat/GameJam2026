using UnityEngine;

public class ItemUI : MonoBehaviour
{
    [SerializeField] private GameObject uiObject;

    public void Show()
    {
        if (uiObject != null)
            uiObject.SetActive(true);
    }

    public void Hide()
    {
        if (uiObject != null)
            uiObject.SetActive(false);
    }
}