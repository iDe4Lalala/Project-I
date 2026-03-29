using System.Collections.Generic;
using UnityEngine;

public class BattleContext
{
    public BattleStateMachine BattleStateMachine { get; private set; }
    public BattleSceneManager BattleSceneManager { get; private set; }
    public IGenerator PlayerGenerator { get; private set; }
    public IGenerator EnemyGenerator { get; private set; }
    public IWeaponGenerator WeaponGenerator { get; private set; }
    public IBattleUIService BattleUIService { get; private set; }
    public IPlayerUIService PlayerUIService { get; private set; }
    public ISpawnPointProvider PlayerSpawnPointProvider { get; private set; }
    public ISpawnPointProvider EnemySpawnPointProvider { get; private set; }
    public HumanDataBase PlayerDataBase { get; }
    public HumanDataBase EnemyDataBase { get; }
    public WeaponDataBase WeaponDataBase { get; }
    public Transform PlayerSpawnPoint { get; }
    public Transform[] EnemySpawnPoints { get; }
    public List<EnemyWaveDataBase> EnemyWaveDataBaseList { get; }

    public BattleContext(
        BattleStateMachine battleStateMachine, BattleSceneManager battleSceneManager, IGenerator playerGenerator,
        IGenerator enemyGenerator, IWeaponGenerator weaponGenerator, IBattleUIService battleUIService, IPlayerUIService playerUIService,
        ISpawnPointProvider playerSpawnPointProvider, ISpawnPointProvider enemySpawnPointProvider,
        HumanDataBase playerDataBase, HumanDataBase enemyDataBase, WeaponDataBase weaponDataBase,
        Transform playerSpawnPoint, Transform[] enemySpawnPoints, List<EnemyWaveDataBase> enemyWaveDataBaseList
    )
    {
        BattleStateMachine = battleStateMachine;
        BattleSceneManager = battleSceneManager;
        PlayerGenerator = playerGenerator;
        EnemyGenerator = enemyGenerator;
        WeaponGenerator = weaponGenerator;
        BattleUIService = battleUIService;
        PlayerUIService = playerUIService;
        PlayerSpawnPointProvider = playerSpawnPointProvider;
        EnemySpawnPointProvider = enemySpawnPointProvider;
        PlayerDataBase = playerDataBase;
        EnemyDataBase = enemyDataBase;
        WeaponDataBase = weaponDataBase;
        PlayerSpawnPoint = playerSpawnPoint;
        EnemySpawnPoints = enemySpawnPoints;
        EnemyWaveDataBaseList = enemyWaveDataBaseList;
    }
}
