using System;

public interface IBattleState
{
    public event Action<int> OnChangingState;

    public void Enter();
    public void Execute();
    public void Exit();
}
