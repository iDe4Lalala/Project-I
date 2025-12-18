using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearanceManager : MonoBehaviour
{
    [SerializeField] private HumanDataBase _enemyDataBase;
    [SerializeField] private List<EnemyWaveDataBase> _enemyWaveDataBaseList;  // 敵のウェーブデータベースリスト
    [SerializeField] private Transform _respawnPointParent;  // 敵のリスポーンポイントの親オブジェクト
    [SerializeField] private BattleSceneManager _battleSceneManager;

    private Transform[] _respawnPoints;
    private List<EnemyComponents> _enemyComponentsList;

    private void OnEnable()
    {
        _enemyComponentsList = new List<EnemyComponents>();
        GetEnemyRespawnPoints();
        GenerateEnemies();
    }

    private void GenerateEnemies()
    {
        /// <summary>
        /// 敵を生成する
        /// </summary>
        
        GameObject enemy = Instantiate(_enemyDataBase.HumanObject);
        SetEnemyRespawnPoint(enemy);

        EnemyComponents enemyComponents = enemy.GetComponent<EnemyComponents>();
        _enemyComponentsList.Add(enemyComponents);
        enemyComponents.EnemyManager.OnDied += OnEnemyDied;
        enemyComponents.EnemyManager.OnDamaged += OnEnemyDamaged;

        GameObject rifle = Instantiate(_battleSceneManager.RiflePrefab, enemyComponents.RifleSocket.transform);
        enemyComponents.SetRifleManager(rifle);
    }

    private void GetEnemyRespawnPoints()
    {
        /// <summary>
        /// 敵のリスポーンポイントを取得
        /// </summary>
        
        int count = _respawnPointParent.childCount;
        _respawnPoints = new Transform[count];

        for (int i = 1; i < count; i++)
        {
            _respawnPoints[i] = _respawnPointParent.GetChild(i);
        }
    }

    private void SetEnemyRespawnPoint(GameObject human)
    {
        /// <summary>
        /// 敵のリスポーンポイントを設定する
        /// </summary>
        
        int rand = Random.Range(1, _respawnPoints.Length - 1);
        human.transform.SetPositionAndRotation(_respawnPoints[rand].position, _respawnPoints[rand].rotation);
    }

    private void OnEnemyDamaged()
    {
        /// <summary>
        /// 敵がダメージを受けた時の処理
        /// </summary>
        
        StartCoroutine(_battleSceneManager.PlayerUIManager.ShowHitCrossHair());
    }

    private void OnEnemyDied(GameObject enemy, EnemyComponents enemyComponents)
    {
        /// <summary>
        /// 敵が死亡した時の処理
        /// </summary>

        enemyComponents.EnemyManager.OnDied -= OnEnemyDied;
        enemyComponents.EnemyManager.OnDamaged -= OnEnemyDamaged;
        
        _enemyComponentsList.Remove(enemyComponents);
        Destroy(enemy);
        _battleSceneManager.OnKilled?.Invoke(true);
    }
}
