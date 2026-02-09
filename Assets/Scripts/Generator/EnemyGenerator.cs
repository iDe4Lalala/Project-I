using UnityEngine;

public class EnemyGenerator : IGenerator<GameObject>
{
    private HumanDataBase _enemyDataBase;

    public EnemyGenerator(HumanDataBase enemyDataBase)
    {
        _enemyDataBase = enemyDataBase;
    }

    public GameObject Generate()
    {
        return Object.Instantiate(_enemyDataBase.HumanObject);
    }
}