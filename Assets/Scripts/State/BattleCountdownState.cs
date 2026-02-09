using UnityEngine;
using System;

public class BattleCountdownState : IBattleState
{
    private PlayerGenerator _playerGenerator;
    private WeaponGenerator _weaponGenerator;
    private IBattleUIService _battleUIService;
    public event Action<int> OnChangingState;

    public BattleCountdownState(PlayerGenerator playerGenerator, WeaponGenerator weaponGenerator, IBattleUIService battleUIManager)
    {
        _playerGenerator = playerGenerator;
        _weaponGenerator = weaponGenerator;
        _battleUIService = battleUIManager;
    }

    public void Enter()
    {
        _battleUIService.OnStartingBattle += OnStartingBattle;

        // プレイヤー、武器(、Map)の生成
        GameObject player = _playerGenerator.Generate();
        PlayerComponents playerComponents = player.GetComponent<PlayerComponents>();
        GameObject rifle = _weaponGenerator.Generate(playerComponents.RifleSocket.transform);
        // playerComponents.SetRifleManager(rifle);

        // 3カウント
        _battleUIService.PlayCountdown(3f);
    }

    public void Execute() { }

    public void Exit()
    {
        _battleUIService.OnStartingBattle -= OnStartingBattle;
    }

    public void OnStartingBattle()
    {
        // WaveStateへ遷移
        OnChangingState?.Invoke(1);
    }
}
