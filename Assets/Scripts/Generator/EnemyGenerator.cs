using UnityEngine;

public class EnemyGenerator : IGenerator
{
    private HumanDataBase _enemyDataBase;
    private IGenerator _weaponGenerator;

    public EnemyGenerator(HumanDataBase enemyDataBase, IGenerator weaponGenerator)
    {
        _enemyDataBase = enemyDataBase;
        _weaponGenerator = weaponGenerator;
    }

    public GameObject Generate(Transform spawnPoint)
    {
        GameObject enemy = Object.Instantiate(_enemyDataBase.HumanObject, spawnPoint);
        var enemyComponents = enemy.GetComponent<EnemyComponents>();
        _weaponGenerator.Generate(enemyComponents.WeaponSocket.transform);
        return enemy;
    }
}