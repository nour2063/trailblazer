using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour
{
    public float popDuration = 0.5f; // Time taken for the pop-in animation
    public float overshootAmount = 1.2f; // How much it overshoots (1.2 = 120% size)

    private Vector3 _targetScale;

    private void Start()
    {
        _targetScale = transform.localScale; // Store original scale
        transform.localScale = Vector3.zero; // Start at zero scale
        StartCoroutine(PopInAnimation());
    }

    private IEnumerator PopInAnimation()
    {
        var elapsedTime = 0f;
        while (elapsedTime < popDuration)
        {
            var t = elapsedTime / popDuration;

            // Bounce effect using Sin wave for elasticity
            var bounce = Mathf.Sin(t * Mathf.PI) * overshootAmount;
            var scaleFactor = Mathf.Lerp(0, 1, t) + bounce * (1 - t);

            transform.localScale = _targetScale * scaleFactor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _targetScale; // Ensure final scale is correct
    }
}