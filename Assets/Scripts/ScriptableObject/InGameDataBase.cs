using UnityEngine;

[CreateAssetMenu(fileName = "InGameDataBase", menuName = "ScriptableObjects/InGameDataBase")]
public class InGameDataBase : ScriptableObject
{
    public BattleResultType BattleResult;

    public void SetBattleResult(BattleResultType battleResult)
    {
        BattleResult = battleResult;
    }
}