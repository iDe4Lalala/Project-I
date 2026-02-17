using UnityEngine;

public class PlayerSpawnPointProvider : ISpawnPointProvider
{
    private Transform _spawnPoint;

    public PlayerSpawnPointProvider(Transform spawnPoint)
    {
        _spawnPoint = spawnPoint;
    }

    public Transform GetSpawnPoint()
    {
        return _spawnPoint;
    }
}
