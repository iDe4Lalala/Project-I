using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveDataBase", menuName = "ScriptableObjects/EnemyWaveDataBase")]
public class EnemyWaveDataBase : ScriptableObject
{
    [field: SerializeField] public float BeforeWaveInterval { get; private set; }
    [field: SerializeField] public float AfterWaveInterval { get; private set; }
    [field: SerializeField] public int EnemyTotalCount { get; private set; }
    [field: SerializeField] public int MaxEnemyAppearanceCount { get; private set; }
    [field: SerializeField] public float SpawnInterval { get; private set; }
    [field: SerializeField] public string WaveText { get; private set; }
}