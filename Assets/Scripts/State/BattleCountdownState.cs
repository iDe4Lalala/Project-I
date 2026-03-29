using System;
using System.Collections;

public class BattleCountdownState : IBattleState
{
    public event Action<BattleStateType> ChangingState;
    public event Action StartingBattle;
    private BattleStateMachine _battleStateMachine;
    private IBattleUIService _battleUIService;

    public BattleCountdownState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public IEnumerator Enter()
    {
        _battleStateMachine.GeneratePlayer();

        yield return _battleUIService.PlayCountdown(3f);

        _battleStateMachine.EnterAliveState();
        StartingBattle?.Invoke();
        ChangingState?.Invoke(BattleStateType.Wave);
    }
}
