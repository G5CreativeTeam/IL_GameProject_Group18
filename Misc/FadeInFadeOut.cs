using UnityEngine;

public class FadeInFadeOut : MonoBehaviour
{
    public float fadeSpeed = 1f; // Speed of fade effect (higher = faster)
    private CanvasGroup canvasGroup;
    private bool isFadingIn = true; // Toggle between fading in and out
    private float alpha = 0f; // Initial alpha value

    private void Start()
    {
        // Get or add a CanvasGroup component to control alpha
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        float fadeAmount = fadeSpeed * Time.deltaTime;

        // Adjust alpha based on whether fading in or out
        if (isFadingIn)
        {
            alpha += fadeAmount;
            if (alpha >= 1f)
            {
                alpha = 1f;
                isFadingIn = false; // Switch to fade out
            }
        }
        else
        {
            alpha -= fadeAmount;
            if (alpha <= 0f)
            {
                alpha = 0f;
                isFadingIn = true; // Switch to fade in
            }
        }

        // Set canvas group alpha
        canvasGroup.alpha = alpha;
    }
}
