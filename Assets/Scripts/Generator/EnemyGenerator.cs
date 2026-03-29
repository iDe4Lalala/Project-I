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
        GameObject enemy = Object.Instantiate(
            _enemyDataBase.HumanObject, spawnPoint.position, spawnPoint.rotation);
        var enemyComponents = enemy.GetComponent<EnemyComponents>();
        GameObject weapon = _weaponGenerator.Generate(enemyComponents.WeaponSocket.transform);

        InitializeEnemyParameter(enemy, weapon);
        return enemy;
    }

    private void InitializeEnemyParameter(GameObject enemy, GameObject weapon)
    {
        var enemyManager = enemy.GetComponent<EnemyManager>();
        var rifleManager = weapon.GetComponent<RifleManager>();

        enemyManager.Initialize(rifleManager);
    }
}
