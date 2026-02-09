using System.Collections.Generic;
using UnityEngine;

public enum BattleStateType
{
    Countdown = 0,
    Wave = 1,
    Clear = 2,
    End = 3
}

public class BattleStateMachine : MonoBehaviour
{
    [SerializeField] private HumanDataBase _playerDataBase;
    [SerializeField] private HumanDataBase _enemyDataBase;
    [SerializeField] private WeaponDataBase _weaponDataBase;
    private IBattleUIService _battleUIService;
    private Dictionary<BattleStateType, IBattleState> _battleStates;
    private IBattleState _currentState;
    private PlayerGenerator _playerGenerator;
    private EnemyGenerator _enemyGenerator;
    private WeaponGenerator _weaponGenerator;

    private void Awake()
    {
        _playerGenerator = new PlayerGenerator(_playerDataBase);
        _enemyGenerator = new EnemyGenerator(_enemyDataBase);
        _weaponGenerator = new WeaponGenerator(_weaponDataBase);
        InitializeStates();
    }

    private void InitializeStates()
    {
        _battleStates = new Dictionary<BattleStateType, IBattleState>()
        {
            { BattleStateType.Countdown, new BattleCountdownState(_playerGenerator, _weaponGenerator, _battleUIService) },
            { BattleStateType.Wave, new WaveState(_enemyGenerator, _weaponGenerator, _battleUIService) },
            { BattleStateType.Clear, new WaveClearState(_battleUIService) },
            { BattleStateType.End, new BattleEndState(_battleUIService) }
        };

        InitializeEvents();
        _currentState = _battleStates[BattleStateType.Countdown];
    }

    private void InitializeEvents()
    {
        foreach (var state in _battleStates.Values)
        {
            state.OnChangingState += ChangeState;
        }
    }

    private void ChangeState(int newStateTypeNumber)
    {
        BattleStateType newStateType = (BattleStateType)newStateTypeNumber;

        _currentState.Exit();
        _currentState = _battleStates[newStateType];
        _currentState.Enter();
    }

    private void Update()
    {
        _currentState.Execute();
    }
}
