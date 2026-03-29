using System;
using System.Collections;

public class BattleEndState : IBattleState
{
    public event Action<BattleStateType> ChangingState;
    public event Action BattleEnded;
    private IBattleUIService _battleUIService;
    private BattleStateMachine _battleStateMachine;

    public BattleEndState(IBattleUIService battleUIManager, BattleStateMachine battleStateMachine)
    {
        _battleUIService = battleUIManager;
        _battleStateMachine = battleStateMachine;
    }

    public IEnumerator Enter()
    {
        switch (_battleStateMachine.BattleResult)
        {
            case BattleResultType.GameClear:
                yield return _battleUIService.OnGameCleared();
                break;
            case BattleResultType.GameOver:
                yield return _battleUIService.OnGameOvered();
                break;
            default:
                break;
        }

        // フェードアウトもしくは一定時間まつ
        yield return _battleStateMachine.WaitForSeconds(2f);

        // ResultSceneへ遷移呼び出し
        BattleEnded?.Invoke();
    }
}
