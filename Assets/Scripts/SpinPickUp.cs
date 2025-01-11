using UnityEngine;

public class AnimatePickUp : MonoBehaviour
{
    public float speed = 100;

    void Update()
    {
        transform.Rotate(new Vector3(1, 2, 1), Time.deltaTime * speed);
    }
}
