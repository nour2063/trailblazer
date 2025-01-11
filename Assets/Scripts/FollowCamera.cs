using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject camera;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);
    }
}
