using System;

public class BattleEndState : IBattleState
{
    public event Action<int> OnChangingState;
    private IBattleUIService _battleUIService;

    public BattleEndState(IBattleUIService battleUIManager)
    {
        _battleUIService = battleUIManager;
    }

    public void Enter()
    {
        // "Game Over"表示
        _battleUIService.ShowProgressTextForSeconds("Game Over", 2f);

        // フェードアウトもしくは一定時間まつ
    }

    public void Execute() { }

    public void Exit()
    {
        // ResultSceneへ遷移呼び出し
    }
}
