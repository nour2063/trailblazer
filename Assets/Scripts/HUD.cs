using UnityEngine;

public class HUD : MonoBehaviour
{
    public Transform userCamera;
    public Vector3 offset = new Vector3(0, -0.2f, 1.5f);
    public float smoothTime = 0.2f;

    private Vector3 _velocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPosition = userCamera.position + userCamera.forward * offset.z + userCamera.up * offset.y + userCamera.right * offset.x;
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, smoothTime);
        
        transform.rotation = Quaternion.LookRotation(transform.position - userCamera.position);
    }
}