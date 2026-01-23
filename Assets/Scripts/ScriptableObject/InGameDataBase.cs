using UnityEngine;

[CreateAssetMenu(fileName = "InGameDataBase", menuName = "ScriptableObjects/InGameDataBase")]
public class InGameDataBase : ScriptableObject
{
    public bool IsGameCleared;

    public void ResetStatus()
    {
        IsGameCleared = false;
    }
}
