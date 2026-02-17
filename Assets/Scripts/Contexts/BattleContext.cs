using UnityEngine;

public class BattleContext
{
    public BattleStateMachine BattleStateMachine { get; private set; }
    public IGenerator PlayerGenerator { get; private set; }
    public IGenerator EnemyGenerator { get; private set; }
    public IGenerator WeaponGenerator { get; private set; }
    public IBattleUIService BattleUIService { get; private set; }
    public ISpawnPointProvider PlayerSpawnPointProvider { get; private set; }
    public ISpawnPointProvider EnemySpawnPointProvider { get; private set; }
    public HumanDataBase PlayerDataBase { get; private set; }
    public HumanDataBase EnemyDataBase { get; private set; }
    public WeaponDataBase WeaponDataBase { get; private set; }
    public Transform PlayerSpawnPoint { get; private set; }
    public Transform[] EnemySpawnPoints { get; private set; }

    public BattleContext(
        BattleStateMachine battleStateMachine, IGenerator playerGenerator,
        IGenerator enemyGenerator, IGenerator weaponGenerator,IBattleUIService battleUIService,
        ISpawnPointProvider playerSpawnPointProvider, ISpawnPointProvider enemySpawnPointProvider,
        HumanDataBase playerDataBase, HumanDataBase enemyDataBase, WeaponDataBase weaponDataBase,
        Transform playerSpawnPoint, Transform[] enemySpawnPoints
    )
    {
        BattleStateMachine = battleStateMachine;
        PlayerGenerator = playerGenerator;
        EnemyGenerator = enemyGenerator;
        WeaponGenerator = weaponGenerator;
        BattleUIService = battleUIService;
        PlayerSpawnPointProvider = playerSpawnPointProvider;
        EnemySpawnPointProvider = enemySpawnPointProvider;
        PlayerDataBase = playerDataBase;
        EnemyDataBase = enemyDataBase;
        WeaponDataBase = weaponDataBase;
        PlayerSpawnPoint = playerSpawnPoint;
        EnemySpawnPoints = enemySpawnPoints;
    }
}
