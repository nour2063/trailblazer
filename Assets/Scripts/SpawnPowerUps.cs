using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SpawnPowerUps : MonoBehaviour
{
    public GameObject[] powerUps;
    public float spawnInterval = 5f;
    
    private FindSpawnPositions _spawn;
    
    void Start()
    {
        _spawn = GetComponent<FindSpawnPositions>();
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }
    
    void SpawnObject()
    {
        _spawn.SpawnObject = powerUps[Random.Range(0, powerUps.Length)];
        _spawn.StartSpawn();
    }
}
