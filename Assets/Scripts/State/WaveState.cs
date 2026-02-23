using UnityEngine;
using System.Collections;
using System;

public class WaveState : IBattleState
{
    public event Action<BattleStateType> ChangingState;
    public event Action<BattleResultType> SettingBattleResult;
    private IGenerator _enemyGenerator;
    private ISpawnPointProvider _enemySpawnPointProvider;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;
    private EnemyWaveDataBase _currentWaveDataBase;

    public WaveState(IGenerator enemyGenerator, ISpawnPointProvider enemySpawnPointProvider, 
        IBattleUIService battleUIManager , BattleStateMachine battleStateMachine)
    {
        _enemyGenerator = enemyGenerator;
        _enemySpawnPointProvider = enemySpawnPointProvider;
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public IEnumerator Enter()
    {
        _currentWaveDataBase = _battleStateMachine.CurrentWaveDataBase;

        // "Wave開始"表示
        yield return 
            _battleUIService.ShowProgressTextForSeconds($"{_currentWaveDataBase.WaveText} Wave Start!", 2f);

        GenerateEnemies(_currentWaveDataBase.MaxEnemyAppearanceCount, _currentWaveDataBase.EnemyTotalCount);
    }

    public void OnPlayerLifePointBecameZero()
    {
        SettingBattleResult?.Invoke(BattleResultType.GameOver);
        ChangingState?.Invoke(BattleStateType.End);
    }

    public void OnAllEnemiesDead()
    {
        ChangingState?.Invoke(BattleStateType.Clear);
    }

    public void OnTimeExpired()
    {
        SettingBattleResult?.Invoke(BattleResultType.GameOver);
        ChangingState?.Invoke(BattleStateType.End);
    }

    private void GenerateEnemies(int spawnBatchSize, int totalEnemies)
    {
        int remainingSpawnCount = totalEnemies;
        while (remainingSpawnCount > 0)
        { 
            int currentBatchSize = Mathf.Min(spawnBatchSize, remainingSpawnCount);
            for (int i = 0; i < currentBatchSize; i++)
            {
                // 敵、武器の生成
                Transform enemySpawnPoint = _enemySpawnPointProvider.GetSpawnPoint();
                GameObject enemy = _enemyGenerator.Generate(enemySpawnPoint);
            }
            remainingSpawnCount -= currentBatchSize;
        }
    }
}
