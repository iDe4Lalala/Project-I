using System;
using System.Collections;
using UnityEngine;

public class BattleCountdownState : IBattleState
{
    public event Action<BattleStateType> ChangingState;
    public event Action StartingBattle;
    private IGenerator _playerGenerator;
    private ISpawnPointProvider _playerSpawnPointProvider;
    private IBattleUIService _battleUIService;

        public BattleCountdownState(IGenerator playerGenerator, ISpawnPointProvider playerSpawnPointProvider,
            IBattleUIService battleUIManager)
        {
            _playerGenerator = playerGenerator;
            _playerSpawnPointProvider = playerSpawnPointProvider;
            _battleUIService = battleUIManager;
        }

    public IEnumerator Enter()
    {
        // プレイヤー、武器(、Map)の生成
        Transform playerSpawnPoint = _playerSpawnPointProvider.GetSpawnPoint();
        GameObject player = _playerGenerator.Generate(playerSpawnPoint);

        // 3カウント
        yield return _battleUIService.PlayCountdown(3f);

        // WaveStateへ遷移
        StartingBattle?.Invoke();
        ChangingState?.Invoke(BattleStateType.Wave);
    }
}
