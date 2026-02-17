using System;
using System.Collections;

public class WaveClearState : IBattleState
{
    public event Action<BattleStateType> OnChangingState;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;

    public WaveClearState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public IEnumerator Enter()
    {
        // "WaveClear"表示
        yield return _battleUIService.ShowProgressTextForSeconds("Wave Clear!", 2f);

        // 一定時間待つ
        yield return _battleStateMachine.WaitForSeconds(2f);
    }

    public void Execute() { }

    public void Exit()
    {
        // 次のWaveStateへ遷移
        OnChangingState?.Invoke(BattleStateType.Wave);
    }
}
