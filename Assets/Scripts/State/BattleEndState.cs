using System;
using System.Collections;

public class BattleEndState : IBattleState
{
    private const float _waitTimeAfterBattleEnd = 2f;
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

        yield return _battleStateMachine.WaitForSeconds(_waitTimeAfterBattleEnd);

        BattleEnded?.Invoke();
    }
}
