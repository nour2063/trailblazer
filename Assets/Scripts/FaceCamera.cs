using Oculus.Interaction;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform player;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
    }

    void Update()
    {
        transform.LookAt(player);
    }
}
