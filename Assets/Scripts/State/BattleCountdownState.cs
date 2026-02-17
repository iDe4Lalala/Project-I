using System;
using System.Collections;
using UnityEngine;

public class BattleCountdownState : IBattleState
    {
    private IGenerator _playerGenerator;
    private ISpawnPointProvider _playerSpawnPointProvider;
    private IGenerator _weaponGenerator;
    private IBattleUIService _battleUIService;
    public event Action<BattleStateType> OnChangingState;

        public BattleCountdownState(IGenerator playerGenerator, ISpawnPointProvider playerSpawnPointProvider,
            IGenerator weaponGenerator, IBattleUIService battleUIManager)
        {
            _playerGenerator = playerGenerator;
            _playerSpawnPointProvider = playerSpawnPointProvider;
            _weaponGenerator = weaponGenerator;
            _battleUIService = battleUIManager;
        }

    public IEnumerator Enter()
    {
        _battleUIService.OnStartingBattle += OnStartingBattle;

        // プレイヤー、武器(、Map)の生成
        Transform playerSpawnPoint = _playerSpawnPointProvider.GetSpawnPoint();
        GameObject player = _playerGenerator.Generate(playerSpawnPoint);

        // 3カウント
        yield return _battleUIService.PlayCountdown(3f);
    }

    public void Execute() { }

    public void Exit()
    {
        _battleUIService.OnStartingBattle -= OnStartingBattle;
    }

    public void OnStartingBattle()
    {
        // WaveStateへ遷移
        OnChangingState?.Invoke(BattleStateType.Wave);
    }
}
