using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    public float bounceSpeed = 2f; // Speed of the bounce
    public float bounceAmount = 0.1f; // How much it shrinks
    private Vector3 _originalScale;

    void Start()
    {
        _originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = 1f + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        transform.localScale = _originalScale * scaleFactor;
    }
}