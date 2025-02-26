using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{
    public float popDuration = 0.5f; // Time taken for the pop-in animation
    public float overshootAmount = 1.2f; // How much it overshoots (1.2 = 120% size)

    private Vector3 targetScale;

    void Start()
    {
        targetScale = transform.localScale; // Store original scale
        transform.localScale = Vector3.zero; // Start at zero scale
        StartCoroutine(PopInAnimation());
    }

    IEnumerator PopInAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < popDuration)
        {
            float t = elapsedTime / popDuration;

            // Bounce effect using Sin wave for elasticity
            float bounce = Mathf.Sin(t * Mathf.PI) * overshootAmount;
            float scaleFactor = Mathf.Lerp(0, 1, t) + bounce * (1 - t);

            transform.localScale = targetScale * scaleFactor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure final scale is correct
    }
}