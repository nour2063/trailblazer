using UnityEngine;

public class BouncePickUp : MonoBehaviour
{
    public float height = 0.3f;
    public float speed = 1f;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        var newY = _startPosition.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
}
