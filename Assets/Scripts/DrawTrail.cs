using System.Collections.Generic;
using UnityEngine;

public class DrawTrail : MonoBehaviour
{
    public GameObject player;
    public float trailHeight = 0.1f;
    public float segmentDuration = 0.25f;
    public float offset = 2f;

    private LineRenderer _line;
    private readonly Queue<Vector3> _positionQueue = new Queue<Vector3>();
    
    private void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 1;
        
        _line.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
        
        InvokeRepeating(nameof(QueuePosition), 0f, segmentDuration);
        InvokeRepeating(nameof(AddSegment), offset, segmentDuration);
    }

    private void AddSegment()
    {
        if (_positionQueue.Count == 0)
        {
            return;
        }

        _line.positionCount += 1;
        _line.SetPosition(_line.positionCount - 1, _positionQueue.Dequeue());

    }

    private void QueuePosition()
    {
        _positionQueue.Enqueue(new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
    }
}
