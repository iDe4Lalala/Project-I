using System;
using System.Collections;

public class WaveClearState : IBattleState
{
    public event Action IncreasingWaveCount;
    public event Action<BattleResultType> SettingBattleResult;
    public event Action<BattleStateType> ChangingState;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;
    private EnemyWaveDataBase _currentWaveDataBase;

    public WaveClearState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public IEnumerator Enter()
    {   
        _currentWaveDataBase = _battleStateMachine.CurrentWaveDataBase;

        // "WaveClear"表示
        yield return _battleUIService.ShowProgressTextForSeconds($"{_currentWaveDataBase.WaveText} Clear!", 2f);

        // 一定時間待つ
        yield return _battleStateMachine.WaitForSeconds(_currentWaveDataBase.AfterWaveInterval);

        if (_battleStateMachine.CurrentWave >= _battleStateMachine.TotalWaves)
        {
            SettingBattleResult?.Invoke(BattleResultType.GameClear);
            ChangingState?.Invoke(BattleStateType.End);
            yield break;
        }

        // 次のWaveStateへ遷移
        IncreasingWaveCount?.Invoke();
        ChangingState?.Invoke(BattleStateType.Wave);
    }
}
