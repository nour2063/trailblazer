using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawTrail : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;
    public float trailHeight = 0.2f;
    public float segmentDuration = 1f;

    private LineRenderer _line;
    
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
        
        InvokeRepeating(nameof(AddSegment), 0f, segmentDuration);
    }

    void AddSegment()
    {
        _line.positionCount += 1;
        _line.SetPosition(_line.positionCount - 1, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
    }
}
