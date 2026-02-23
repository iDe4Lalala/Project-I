using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleStateType
{
    Countdown = 0,
    Wave = 1,
    Clear = 2,
    End = 3
}

public enum BattleResultType
{
    GameClear = 0,
    GameOver = 1
}

public class BattleStateMachine : MonoBehaviour
{
    public event Action<BattleResultType> ChangingScene;
    public BattleResultType BattleResult { get; private set; }
    public int CurrentWave { get; private set; }
    public int TotalWaves { get; private set; }
    public EnemyWaveDataBase CurrentWaveDataBase { get; private set; }
    private Dictionary<BattleStateType, IBattleState> _battleStates;
    private IBattleState _currentState;
    private BattleContext _battleContext;

    public void Initialize(BattleContext battleContext)
     {
        TotalWaves = battleContext.EnemyWaveDataBaseList.Count;
        _battleContext = battleContext;
        InitializeStates();
        InitializeEvents();

        StartCoroutine(_currentState.Enter());
    }

    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private void InitializeStates()
    {
        // Stateの生成をinterfaceにするのを検討
        _battleStates = new Dictionary<BattleStateType, IBattleState>()
        {
            { BattleStateType.Countdown, new BattleCountdownState(_battleContext.PlayerGenerator, 
                _battleContext.PlayerSpawnPointProvider, _battleContext.BattleUIService) },
            { BattleStateType.Wave, new WaveState(_battleContext.EnemyGenerator, 
                _battleContext.EnemySpawnPointProvider, _battleContext.BattleUIService, this) },
            { BattleStateType.Clear, new WaveClearState(_battleContext.BattleUIService, this) },
            { BattleStateType.End, new BattleEndState(_battleContext.BattleUIService, this) }
        };

        _currentState = _battleStates[BattleStateType.Countdown];
    }

    private void InitializeEvents()
    {
        foreach (var battleState in _battleStates.Values)
        {
            battleState.ChangingState += OnChangeState;
        }

        if (!_battleStates.TryGetValue(BattleStateType.End, out var state)) return;
        if (state is BattleCountdownState countdownState)
        {
            countdownState.StartingBattle += OnStartingBattle;
        }
        else if (state is WaveState waveState)
        {
            waveState.SettingBattleResult += OnSettingBattleResult;
        }
        else if (state is WaveClearState clearState)
        {
            clearState.IncreasingWaveCount += OnAdvancingWaveCount;
            clearState.SettingBattleResult += OnSettingBattleResult;
        }
        else if (state is BattleEndState endState) 
        {
            endState.BattleEnded += OnBattleEnded;
        }
    }

    private void OnChangeState(BattleStateType newStateType)
    {
        // すでにこのコルーチンが動いている時の処理を書く(Coroutine型の変数追加)
        _currentState = _battleStates[newStateType];
        StartCoroutine(_currentState.Enter());
    }

    private void OnBattleEnded(BattleResultType result)
    {
        ChangingScene?.Invoke(result);
    }

    private void OnSettingBattleResult(BattleResultType result)
    {
        BattleResult = result;
    }

    private void OnAdvancingWaveCount()
    {
        CurrentWaveDataBase = _battleContext.EnemyWaveDataBaseList[CurrentWave];
        CurrentWave++;
    }

    private void OnStartingBattle()
    {
        CurrentWave = 0;
        OnAdvancingWaveCount();
    }
}
