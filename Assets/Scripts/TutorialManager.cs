using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial Images")]
    [SerializeField] private GameObject[] tutorialPages;

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Optional: Page Indicator")]
    [SerializeField] private TMPro.TextMeshProUGUI pageIndicatorText;

    private int currentPageIndex = 0;

    void Start()
    {
        // Validate setup
        if (tutorialPages == null || tutorialPages.Length == 0)
        {
            Debug.LogError("TutorialController: No tutorial pages assigned!");
            return;
        }

        // Hook up button listeners
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        if (previousButton != null)
        {
            previousButton.onClick.AddListener(OnPreviousButtonClicked);
        }

        // Show first page
        ShowPage(0);
    }

    public void OnNextButtonClicked()
    {
        if (currentPageIndex < tutorialPages.Length - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
        }
    }

    public void OnPreviousButtonClicked()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
        }
    }

    private void ShowPage(int pageIndex)
    {
        // Hide all pages
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if (tutorialPages[i] != null)
            {
                tutorialPages[i].SetActive(i == pageIndex);
            }
        }

        // Update button states
        UpdateButtonStates();

        // Update page indicator if present
        UpdatePageIndicator();
    }

    private void UpdateButtonStates()
    {
        // Disable previous button on first page
        if (previousButton != null)
        {
            previousButton.interactable = currentPageIndex > 0;
        }

        // Disable next button on last page
        if (nextButton != null)
        {
            nextButton.interactable = currentPageIndex < tutorialPages.Length - 1;
        }
    }

    private void UpdatePageIndicator()
    {
        if (pageIndicatorText != null)
        {
            pageIndicatorText.text = $"{currentPageIndex + 1} / {tutorialPages.Length}";
        }
    }

    // Optional: Allow keyboard navigation
    void Update()
    {
        var keyboard = UnityEngine.InputSystem.Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
        {
            OnNextButtonClicked();
        }

        if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
        {
            OnPreviousButtonClicked();
        }
    }
}