using UnityEngine;

public class Multiplier : MonoBehaviour
{
    public float height = 0.3f;
    public float speed = 1f;

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float newY = _startPosition.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
    }
}
