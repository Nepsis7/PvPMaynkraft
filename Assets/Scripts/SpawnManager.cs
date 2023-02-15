using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    public Vector3 GetSpawnPoint()
    {
        if(spawnPoints.Count < 1)
            return Vector3.zero;
        Transform _point = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
        if(!_point)
            return Vector3.zero;
        return _point.position;
    }
}
