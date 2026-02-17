using UnityEngine;

public class EnemySpawnPointProvider : ISpawnPointProvider
{
    private Transform[] _spawnPoints;

    public EnemySpawnPointProvider(Transform[] spawnPoints)
    {
        _spawnPoints = spawnPoints;
    }

    public Transform GetSpawnPoint()
    {
        int index = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[index];
    }
}
