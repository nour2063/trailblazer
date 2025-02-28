using UnityEngine;

public class UIBounce : MonoBehaviour
{
    public float bounceSpeed = 7.5f; // Speed of the bounce
    public float bounceAmount = 0.15f; // How much it shrinks
    private Vector3 _originalScale;

    private void Start()
    {
        _originalScale = transform.localScale;
    }

    private void Update()
    {
        var scaleFactor = 1f + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
        transform.localScale = _originalScale * scaleFactor;
    }
}