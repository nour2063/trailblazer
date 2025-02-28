using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrail : MonoBehaviour
{
    public GameObject player;
    public GameObject trace;
    public GameObject outline;
    public VignetteController vignette; 

    public float trailHeight = 0.1f;
    public float segmentDuration = 0.25f;
    public float offset = 2f;
    public float grace = 2f; 
    public int minimumLineLength = 150;
    public int segmentTotal;

    public float vignetteMaxIntensity = 0.5f; 
    public float vignetteMinIntensity = 0.1f;
    public float vignetteDistanceThreshold = 5f; 

    public AudioClip damageSound;

    private LineRenderer _line;
    private LineRenderer _trace;
    private LineRenderer _outline;
    private readonly Queue<Vector3> _positionQueue = new Queue<Vector3>();
    private MeshCollider _collider;
    private Mesh _mesh;

    private int _segmentCount = 0;
    private Game _game;

    private void Start()
    {
        _game = FindAnyObjectByType<Game>();

        _line = GetComponent<LineRenderer>();
        _outline = outline.GetComponent<LineRenderer>();
        _trace = trace.GetComponent<LineRenderer>();
        
        segmentTotal = minimumLineLength;

        _collider = gameObject.AddComponent<MeshCollider>();
        _mesh = new Mesh();

        StartDrawTrail();
    }

    private void Update()
    {
        UpdateVignetteIntensity();
    }

    private void UpdateVignetteIntensity()
    {
        if (_line.positionCount == 0) return;

        // Find the closest point on the trail
        var closestDistance = float.MaxValue;
        for (var i = 0; i < _line.positionCount; i++)
        {
            var distance = Vector3.Distance(player.transform.position, _line.GetPosition(i));
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        // Normalize intensity based on distance
        var intensity = Mathf.Clamp01(1 - (closestDistance / vignetteDistanceThreshold));
        var vignetteValue = Mathf.Lerp(vignetteMinIntensity, vignetteMaxIntensity, intensity);

        vignette.SetIntensity(vignetteValue, 0.1f); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _positionQueue.Clear();
        CancelInvoke();
        
        _line.positionCount = 0;
        _outline.positionCount = 0;
        _trace.positionCount = 0;
        segmentTotal = minimumLineLength;

        if (_game.score < 20)
        {
            _game.score = 0;
        }
        else
        {
            _game.score -= 20;
        }
        
        _game.scoreText.text = "" + _game.score;
        _game.GetComponent<AudioSource>().PlayOneShot(damageSound);
        
        StartCoroutine(_game.SetVignette(Color.red, 0.75f));

        Invoke(nameof(StartDrawTrail), grace);
    }

    private void StartDrawTrail()
    {
        _line.positionCount = 1;
        _trace.positionCount = 1;
        _outline.positionCount = 1;

        _line.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
        _trace.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));
        _outline.SetPosition(0, new Vector3(player.transform.position.x, trailHeight, player.transform.position.z));

        InvokeRepeating(nameof(QueuePosition), 0f, segmentDuration);
        InvokeRepeating(nameof(AddSegment), offset, segmentDuration);
    }

    private void AddSegment()
    {
        if (_positionQueue.Count == 0) return;

        _line.positionCount += 1;
        _outline.positionCount += 1;

        var position = _positionQueue.Dequeue();
        _line.SetPosition(_line.positionCount - 1, position);
        _outline.SetPosition(_outline.positionCount - 1, position);

        _line.BakeMesh(_mesh);
        _collider.sharedMesh = _mesh;
        _segmentCount++;

        if (_segmentCount <= segmentTotal) return;
        RemoveOldSegment();
        _segmentCount--;
    }

    private void QueuePosition()
    {
        var currentPosition = new Vector3(player.transform.position.x, trailHeight, player.transform.position.z);

        _positionQueue.Enqueue(currentPosition);

        _trace.positionCount += 1;
        _trace.SetPosition(_trace.positionCount - 1, currentPosition);
    }

    private void RemoveOldSegment()
    {
        if (_line.positionCount <= 1) return;

        for (var i = 1; i < _line.positionCount; i++)
        {
            _line.SetPosition(i - 1, _line.GetPosition(i));
            _outline.SetPosition(i - 1, _outline.GetPosition(i));
        }

        _line.positionCount--;
        _outline.positionCount--;

        for (var i = 1; i < _trace.positionCount; i++)
        {
            _trace.SetPosition(i - 1, _trace.GetPosition(i));
        }
        _trace.positionCount--;
    }
}
