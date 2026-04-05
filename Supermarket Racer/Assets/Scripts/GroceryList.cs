using UnityEngine;
using UnityEngine.UI;

public class GroceryList : MonoBehaviour
{
    [SerializeField] private RawImage groceryPaper;
    [SerializeField] private GameObject guideText;

    private RectTransform groceryPaperRect;

    [SerializeField] private Vector2 listShownPosition = new Vector2(335f, -160f);
    [SerializeField] private Vector2 listHiddenPosition = new Vector2(335f, -885f);

    [SerializeField] private bool listStartsShown = false;

    private bool listShown;

    private void Awake()
    {
        if (groceryPaper == null)
        {
            Debug.LogError("GroceryPaper RawImage is not assigned in the Inspector.");
            return;
        }

        if (guideText == null)
        {
            Debug.LogError("GuideText is not assigned in the Inspector.");
            return;
        }

        groceryPaperRect = groceryPaper.GetComponent<RectTransform>();

        if (groceryPaperRect == null)
        {
            Debug.LogError("GroceryPaper does not have a RectTransform.");
            return;
        }

        listShown = listStartsShown;
        ApplyState();
    }

    private void Update()
    {
        if (groceryPaperRect == null) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            listShown = !listShown;

            if (SoundManager.Instance != null)
            {
                if (listShown)
                {
                    SoundManager.Instance.PlayGroceryListOpenSound();
                }
                else
                {
                    SoundManager.Instance.PlayGroceryListCloseSound();
                }
            }

            ApplyState();
        }
    }

    private void ApplyState()
    {
        groceryPaperRect.anchoredPosition = listShown ? listShownPosition : listHiddenPosition;
        guideText.SetActive(!listShown);
    }
}