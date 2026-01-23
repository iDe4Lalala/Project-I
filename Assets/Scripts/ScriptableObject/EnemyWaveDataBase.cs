using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveDataBase", menuName = "ScriptableObjects/EnemyWaveDataBase")]
public class EnemyWaveDataBase : ScriptableObject
{
    /// <summary>
    /// 敵のウェーブデータベース
    /// </summary>

    [field: SerializeField] public float BeforeWaveInterval { get; private set; }  // 前のウェーブからの間隔
    [field: SerializeField] public float AfterWaveInterval { get; private set; }
    [field: SerializeField] public int EnemyTotalCount { get; private set; }  // 敵の総数
    [field: SerializeField] public int MaxEnemyAppearanceCount { get; private set; }  // 最大同時出現数
    [field: SerializeField] public float SpawnInterval { get; private set; }  // スポーン間隔
    [field: SerializeField] public string WaveText { get; private set; }
}