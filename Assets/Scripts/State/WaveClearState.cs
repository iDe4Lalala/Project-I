using System;

public class WaveClearState : IBattleState
{
    public event Action<int> OnChangingState;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;

    public WaveClearState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public void Enter()
    {
        // "WaveClear"表示
        _battleStateMachine.RunCoroutine(_battleUIService.ShowProgressTextForSeconds("Wave Clear!", 2f));

        // 一定時間待つ
        _battleStateMachine.RunCoroutine(_battleStateMachine.WaitForSeconds(2f));
    }

    public void Execute() { }

    public void Exit()
    {
        // 次のWaveStateへ遷移
        OnChangingState?.Invoke(1);
    }
}
