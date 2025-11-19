using System.Collections;
using UnityEngine;

public class TextMoving : MonoBehaviour
{
    public static TextMoving instance;

    private RectTransform rectTransform;
    public float movementDuration = 1.0f;
    public GameObject targetPosition;
    public GameObject startPosition;
    Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 startScale = new Vector3(1f, 1f, 1f);

    // Do NOT access other singletons in field initializers. Initialize at runtime instead.
    public bool isInGame;

    private void Awake()
    {
        // Safe singleton assignment
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("Multiple TextMoving instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Get the RectTransform component when the script starts
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("TextMover RectTransform null");
        }
    }

    private void Start()
    {
        // Initialize isInGame safely after other singletons have had a chance to initialize.
        if (MenuManager.instance != null)
        {
            isInGame = MenuManager.instance.isInGame;
        }
        else
        {
            Debug.LogWarning("MenuManager.instance null");
        }

        // Warn if inspector references are missing
        if (startPosition == null)
        {
            Debug.LogWarning("startPosition null");
        }
        if (targetPosition == null)
        {
            Debug.LogWarning("targetPosition null");
        }
    }

    public void TextMove()
    {
        // Validate references
        if (startPosition == null || targetPosition == null)
        {
            Debug.LogError("Cannot move text: startPosition and/or targetPosition is not assigned.");
            return;
        }

        // Refresh isInGame from MenuManager if available (keeps value up-to-date)
        if (MenuManager.instance != null)
        {
            isInGame = MenuManager.instance.isInGame;
        }

        Vector3 startPos = startPosition.GetComponent<RectTransform>().anchoredPosition;
        Vector3 targetPos = targetPosition.GetComponent<RectTransform>().anchoredPosition;

        if (isInGame)
        {
            MoveToTarget(targetPos, targetScale);
        }
        else
        {
            BackToStart(startPos, startScale);
        }
    }

    public void MoveToTarget(Vector3 targetPosition, Vector3 targetScale)
    {
        // Stop any previous movement coroutine to prevent jerky behavior
        StopAllCoroutines();

        // Start the new movement coroutine
        StartCoroutine(MoveCoroutine(targetPosition, targetScale));
    }

    public void BackToStart(Vector3 startPosition, Vector3 startScale)
    {
        // Stop any previous movement coroutine to prevent jerky behavior
        StopAllCoroutines();
        // Start the new movement coroutine
        StartCoroutine(BackToStartCoroutine(startPosition, startScale));
    }

    private IEnumerator MoveCoroutine(Vector3 targetPosition, Vector3 targetScale)
    {
        if (rectTransform == null)
            yield break;

        // Store the starting position of the UI element
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 startScale = rectTransform.localScale;

        // Track the elapsed time since the movement started
        float elapsedTime = 0f;

        // Loop until the elapsed time exceeds the desired duration
        while (elapsedTime < movementDuration)
        {
            // Calculate the fraction (0.0 to 1.0) of the movement completed
            float t = elapsedTime / movementDuration;

            // Use Lerp to interpolate between the start and end positions
            rectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // Increment the elapsed time by the time passed since the last frame
            elapsedTime += Time.deltaTime;

            // Yield control back to Unity, waiting until the next frame
            yield return null;
        }

        // Ensure the element snaps to the exact final position to avoid small floating-point errors
        rectTransform.localPosition = targetPosition;
        rectTransform.localScale = targetScale;

        Debug.Log($"Text movement complete! Final position: {targetPosition}");
    }

    private IEnumerator BackToStartCoroutine(Vector3 startPosition, Vector3 startScale)
    {
        if (rectTransform == null)
            yield break;

        // Store the starting position of the UI element
        Vector3 targetPosition = rectTransform.localPosition;
        Vector3 targetScale = rectTransform.localScale;

        // Track the elapsed time since the movement started
        float elapsedTime = 0f;

        // Loop until the elapsed time exceeds the desired duration
        while (elapsedTime < movementDuration)
        {
            // Calculate the fraction (0.0 to 1.0) of the movement completed
            float t = elapsedTime / movementDuration;

            // Use Lerp to interpolate between the start and end positions
            rectTransform.localPosition = Vector3.Lerp(targetPosition, startPosition, t);
            rectTransform.localScale = Vector3.Lerp(targetScale, startScale, t);

            // Increment the elapsed time by the time passed since the last frame
            elapsedTime += Time.deltaTime;

            // Yield control back to Unity, waiting until the next frame
            yield return null;
        }

        // Ensure the element snaps to the exact final position to avoid small floating-point errors
        rectTransform.localPosition = startPosition;
        rectTransform.localScale = startScale;

        Debug.Log($"Text movement complete! Final position: {startPosition}");
    }
}
