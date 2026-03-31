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
    public BattleResultType BattleResult { get; private set; }
    public EnemyWaveDataBase CurrentWaveDataBase { get; private set; }
    public int CurrentWave { get; private set; }
    public int TotalWaves { get; private set; }
    public event Action<BattleResultType> ChangingScene;
    public event Action<GameObject> PlayerGenerated;
    public event Action BattleStarted;
    private Dictionary<BattleStateType, IBattleState> _battleStates;
    private IBattleState _currentState;
    private BattleCountdownState _battleCountdownState;
    private WaveState _waveState;
    private WaveClearState _waveClearState;
    private BattleEndState _battleEndState;
    private BattleContext _battleContext;
    private PlayerComponents _playerComponents;

    private void OnDisable()
    {
        DisposeEvents();
    }

    public void Initialize(BattleContext battleContext)
     {
        TotalWaves = battleContext.EnemyWaveDataBaseList.Count;
        _battleContext = battleContext;

        InitializeStates();
        InitializeEvents();
        OnChangeState(BattleStateType.Countdown);
    }

    public void GeneratePlayer()
    {
        Transform playerSpawnPoint = _battleContext.PlayerSpawnPointProvider.GetSpawnPoint();
        GameObject player = _battleContext.PlayerGenerator.Generate(playerSpawnPoint);
        _playerComponents = player.GetComponent<PlayerComponents>();
        _battleContext.PlayerUIService.Initialize(_playerComponents);
        PlayerGenerated?.Invoke(player);
    }

    public void EnterAliveState()
    {
        _playerComponents.PlayerManager.SetAliveState();
    }

    public IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    private void InitializeStates()
    {
        _battleCountdownState = new BattleCountdownState(_battleContext.BattleUIService, this);
        _waveState = new WaveState(_battleContext.EnemyGenerator, 
                _battleContext.EnemySpawnPointProvider, _battleContext.BattleUIService, this);
        _waveClearState = new WaveClearState(_battleContext.BattleUIService, this);
        _battleEndState = new BattleEndState(_battleContext.BattleUIService, this);

        _battleStates = new Dictionary<BattleStateType, IBattleState>()
        {
            { BattleStateType.Countdown, _battleCountdownState },
            { BattleStateType.Wave, _waveState },
            { BattleStateType.Clear, _waveClearState },
            { BattleStateType.End, _battleEndState }
        };
    }

    private void InitializeEvents()
    {
        foreach (var battleState in _battleStates.Values)
        {
            battleState.ChangingState += OnChangeState;
        }

        _battleContext.BattleSceneManager.PlayerRespawning += _waveState.OnPlayerRespawning;
        _battleCountdownState.StartingBattle += OnStartingBattle;
        _waveState.SettingBattleResult += OnSettingBattleResult;
        _battleContext.BattleSceneManager.TimeExpired += _waveState.OnTimeExpired;
        _battleContext.BattleSceneManager.PlayerLifePointBecameZero += _waveState.OnPlayerLifePointBecameZero;
        _waveClearState.IncreasingWaveCount += OnAdvancingWaveCount;
        _waveClearState.SettingBattleResult += OnSettingBattleResult;
        _battleEndState.BattleEnded += OnBattleEnded;
    }

    private void DisposeEvents()
    {
        foreach (var battleState in _battleStates.Values)
        {
            battleState.ChangingState -= OnChangeState;
        }

        _battleContext.BattleSceneManager.PlayerRespawning -= _waveState.OnPlayerRespawning;
        _battleCountdownState.StartingBattle -= OnStartingBattle;
        _waveState.SettingBattleResult -= OnSettingBattleResult;
        _battleContext.BattleSceneManager.TimeExpired -= _waveState.OnTimeExpired;
        _battleContext.BattleSceneManager.PlayerLifePointBecameZero -= _waveState.OnPlayerLifePointBecameZero;
        _waveClearState.IncreasingWaveCount -= OnAdvancingWaveCount;
        _waveClearState.SettingBattleResult -= OnSettingBattleResult;
        _battleEndState.BattleEnded -= OnBattleEnded;
    }

    private void OnStartingBattle()
    {
        CurrentWave = 0;
        OnAdvancingWaveCount();
        BattleStarted?.Invoke();
    }

    private void OnChangeState(BattleStateType newStateType)
    {
        _currentState = _battleStates[newStateType];
        Debug.Log($"current state: {_currentState}");
        StartCoroutine(_currentState.Enter());
    }

    private void OnAdvancingWaveCount()
    {
        CurrentWaveDataBase = _battleContext.EnemyWaveDataBaseList[CurrentWave];
        CurrentWave++;
    }

    private void OnSettingBattleResult(BattleResultType result)
    {
        BattleResult = result;
    }

    private void OnBattleEnded()
    {
        ChangingScene?.Invoke(BattleResult);
    }
}