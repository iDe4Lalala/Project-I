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
    public event Action<BattleResultType> OnChangingScene;
    private Dictionary<BattleStateType, IBattleState> _battleStates;
    private IBattleState _currentState;
    private BattleContext _battleContext;

    private void Update()
    {
        if(_currentState == null) return;
        _currentState.Execute();
    }

    public void Initialize(BattleContext battleContext)
     {
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
            { BattleStateType.Countdown, new BattleCountdownState(
                _battleContext.PlayerGenerator, _battleContext.PlayerSpawnPointProvider, _battleContext.WeaponGenerator, _battleContext.BattleUIService) },
            { BattleStateType.Wave, new WaveState(_battleContext.EnemyGenerator, _battleContext.EnemySpawnPointProvider, _battleContext.WeaponGenerator, _battleContext.BattleUIService) },
            { BattleStateType.Clear, new WaveClearState(_battleContext.BattleUIService, this) },
            { BattleStateType.End, new BattleEndState(_battleContext.BattleUIService, this) }
        };

        _currentState = _battleStates[BattleStateType.Countdown];
    }

    private void InitializeEvents()
    {
        foreach (var battleState in _battleStates.Values)
        {
            battleState.OnChangingState += ChangeState;
        }

        if (!_battleStates.TryGetValue(BattleStateType.End, out var state)) return;
        if (state is BattleEndState endState) 
        {
            endState.OnBattleEnded += OnBattleEnded;
        }
    }

    private void ChangeState(BattleStateType newStateType)
    {
        // すでにこのコルーチンが動いている時の処理を書く
        _currentState.Exit();
        _currentState = _battleStates[newStateType];
        StartCoroutine(_currentState.Enter());
    }

    public void OnBattleEnded(BattleResultType result)
    {
        OnChangingScene?.Invoke(result);
    }
}
