using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextScale : MonoBehaviour
{
    [SerializeField] private Text textToScale;

    public IEnumerator ScaleText(Vector3 targetScale)
    {
        Vector3 originalScale = textToScale.transform.localScale;
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            textToScale.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        textToScale.transform.localScale = targetScale;
    }

    public void SetActiveText(bool isActive)
    { 
        textToScale.gameObject.SetActive(isActive);
    }

    public void SetScaleText(Vector3 scale)
    { 
        textToScale.transform.localScale = scale;
    }
}
