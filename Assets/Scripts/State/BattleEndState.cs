using System;
using System.Collections;

public class BattleEndState : IBattleState
{
    public event Action<BattleStateType> OnChangingState;
    public event Action<BattleResultType> OnBattleEnded;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;

    public BattleEndState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public IEnumerator Enter()
    {
        // "Game Over"なら表示
        yield return _battleUIService.ShowProgressTextForSeconds("Game Over", 2f);

        // フェードアウトもしくは一定時間まつ
        yield return _battleStateMachine.WaitForSeconds(2f);
    }

    public void Execute() { }

    public void Exit()
    {
        // ResultSceneへ遷移呼び出し
        OnBattleEnded?.Invoke(BattleResultType.GameClear);
    }
}
