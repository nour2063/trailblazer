using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawTrail : MonoBehaviour
{
    public GameObject player;
    public GameObject trace;
    public float trailHeight = 0.1f;
    public float segmentDuration = 0.25f;
    public float offset = 2f;

    public AudioClip damageSound;

    private LineRenderer _line;
    private LineRenderer _trace;
    private readonly Queue<Vector3> _positionQueue = new Queue<Vector3>();
    private MeshCollider _collider;
    private Mesh _mesh;

    private Game _game;
    
    private void Start()
    {
        _game = FindAnyObjectByType<Game>();
        
        _line = GetComponent<LineRenderer>();
        _trace = trace.GetComponent<LineRenderer>();
        
        _collider = gameObject.AddComponent<MeshCollider>();
        _mesh = new Mesh();
        
        StartDrawTrail();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        _positionQueue.Clear();
        CancelInvoke();

        if (_game.score < 10)
        {
            _game.score = 0;
        }
        else
        {
            _game.score -= 10;
        }
        _game.scoreText.text = "" + _game.score;
        _game.GetComponent<AudioSource>().PlayOneShot(damageSound);
        
        StartDrawTrail();
    }
    
    private void StartDrawTrail()
    {
        _line.positionCount = 1;
        _trace.positionCount = 1;
        
        _line.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
        _trace.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
        
        InvokeRepeating(nameof(QueuePosition), 0f, segmentDuration);
        InvokeRepeating(nameof(AddSegment), offset, segmentDuration);
    }

    private void AddSegment()
    {
        if (_positionQueue.Count == 0) return;

        _line.positionCount += 1;
        _line.SetPosition(_line.positionCount - 1, _positionQueue.Dequeue());
        
        _line.BakeMesh(_mesh);
        _collider.sharedMesh = _mesh;
    }

    private void QueuePosition()
    {
        var currentPosition = new Vector3(player.transform.position.x, trailHeight, player.transform.position.z);
        
        _positionQueue.Enqueue(currentPosition);

        _trace.positionCount += 1;
        _trace.SetPosition(_trace.positionCount - 1, currentPosition);
    }
}
