using System;
using System.Collections;

public interface IBattleState
{

    public event Action<BattleStateType> ChangingState;

    public IEnumerator Enter();
}
