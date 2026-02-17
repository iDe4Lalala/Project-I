using System;
using System.Collections;

public interface IBattleState
{
    public event Action<BattleStateType> OnChangingState;

    public IEnumerator Enter();
    public void Execute();
    public void Exit();
}
