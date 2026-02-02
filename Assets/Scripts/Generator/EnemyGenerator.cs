using UnityEngine;

public class EnemyGenerator : IGenerator<GameObject>
{
    public GameObject Generate(GameObject enemyPrefab)
    {
        return Object.Instantiate(enemyPrefab);
    }
}