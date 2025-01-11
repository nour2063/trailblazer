using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.LookAt(player);
    }
}
