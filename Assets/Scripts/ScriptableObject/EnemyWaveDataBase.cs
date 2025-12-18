using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveDataBase", menuName = "ScriptableObjects/EnemyWaveDataBase")]
public class EnemyWaveDataBase : ScriptableObject
{
    /// <summary>
    /// 敵のウェーブデータベース
    /// </summary>

    [field: SerializeField] public float WaveStartTime { get; private set; }  // ウェーブ開始時間
    [field: SerializeField] public int EnemyTotalCount { get; private set; }  // 敵の総数
    [field: SerializeField] public int MaxEnemyAppearanceCount { get; private set; }  // 最大同時出現数
    [field: SerializeField] public float SpawnInterval { get; private set; }  // スポーン間隔
}
