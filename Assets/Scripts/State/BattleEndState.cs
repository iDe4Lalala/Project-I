using System;

public class BattleEndState : IBattleState
{
    public event Action<int> OnChangingState;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;

    public BattleEndState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public void Enter()
    {
        // "Game Over"表示
        _battleStateMachine.RunCoroutine(_battleUIService.ShowProgressTextForSeconds("Game Over", 2f));

        // フェードアウトもしくは一定時間まつ
        _battleStateMachine.RunCoroutine(_battleStateMachine.WaitForSeconds(2f));
    }

    public void Execute() { }

    public void Exit()
    {
        // ResultSceneへ遷移呼び出し
    }
}
