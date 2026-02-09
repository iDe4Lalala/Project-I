using System;

public class WaveClearState : IBattleState
{
    public event Action<int> OnChangingState;
    private IBattleUIService _battleUIService;

    public WaveClearState(IBattleUIService battleUIManager)
    {
        _battleUIService = battleUIManager;
    }

    public void Enter()
    {
        // "WaveClear"表示
        _battleUIService.ShowProgressTextForSeconds("Wave Clear!", 2f);

        // 一定時間待つ
    }

    public void Execute() { }

    public void Exit()
    {
        // 次のWaveStateへ遷移
        OnChangingState?.Invoke(1);
    }
}
