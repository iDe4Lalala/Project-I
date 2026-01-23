using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public enum EnemyWaveState
{
    Waiting,
    InProgress,
    Spawning,
    Cleared,
    Finished
}

public class EnemyAppearanceManager : MonoBehaviour
{
    [SerializeField] private HumanDataBase _enemyDataBase;
    [field: SerializeField] public List<EnemyWaveDataBase> EnemyWaveDataBaseList { get; private set; }  // 敵のウェーブデータベースリスト
    [SerializeField] private Transform _respawnPointParent;  // 敵のリスポーンポイントの親オブジェクト
    [SerializeField] private BattleSceneManager _battleSceneManager;
    [field: SerializeField] public InGameDataBase InGameDataBase { get; private set; }
    
    public EnemyWaveState CurrentWaveState { get; private set; }
    private Transform[] _respawnPoints;
    private List<EnemyComponents> _enemyComponentsList;
    private int _currentWaveIndex;
    private bool _isLastWave;
    public event Action<string> OnWaveStarted;  // ウェーブが開始された時のイベント

    private void OnEnable()
    {
        _currentWaveIndex = 0;
        _enemyComponentsList = new List<EnemyComponents>();
        _isLastWave = false;
        InGameDataBase.ResetStatus();
        CurrentWaveState = EnemyWaveState.Waiting;
        GetEnemyRespawnPoints();
        StartCoroutine(WaitForNextWave(EnemyWaveDataBaseList[_currentWaveIndex].BeforeWaveInterval));
    }

    public void SetEnemyWaveState(EnemyWaveState state)
    {
        /// <summary>
        /// 敵のウェーブ状態を設定する
        /// </summary>
        
        CurrentWaveState = state;
    }

    private void GenerateEnemies(int generateCount)
    {
        /// <summary>
        /// 敵を生成する
        /// </summary>
        
        for (int i = 0; i < generateCount; i++)
        {
            GameObject enemy = Instantiate(_enemyDataBase.HumanObject);
            SetEnemyRespawnPoint(enemy);

            EnemyComponents enemyComponents = enemy.GetComponent<EnemyComponents>();
            _enemyComponentsList.Add(enemyComponents);
            enemyComponents.EnemyManager.OnDied += OnEnemyDied;
            enemyComponents.EnemyManager.OnDamaged += OnEnemyDamaged;

            GameObject rifle = Instantiate(_battleSceneManager.RiflePrefab, enemyComponents.RifleSocket.transform);
            enemyComponents.SetRifleManager(rifle);
        }
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
        StartCoroutine(CountRemainingEnemies());
    }

    private IEnumerator CountRemainingEnemies()
    {
        if (_isLastWave && _enemyComponentsList.Count <= 0)
        {
            CurrentWaveState = EnemyWaveState.Finished;
            _battleSceneManager.BattleUIManager.OnThisWaveCleared(EnemyWaveDataBaseList[_currentWaveIndex].WaveText);
            yield return new WaitForSeconds(EnemyWaveDataBaseList[_currentWaveIndex].AfterWaveInterval);
            _battleSceneManager.PlayerUIManager.DisplayOrHideCursor(true);
            InGameDataBase.IsGameCleared = true;
            _battleSceneManager.LoadOtherScene();
            yield break;
        }

        if (_enemyComponentsList.Count > 0) yield break;
        CurrentWaveState = EnemyWaveState.Cleared;
        _battleSceneManager.BattleUIManager.OnThisWaveCleared(EnemyWaveDataBaseList[_currentWaveIndex].WaveText);
        yield return new WaitForSeconds(EnemyWaveDataBaseList[_currentWaveIndex].AfterWaveInterval);
        _currentWaveIndex++;
        StartCoroutine(WaitForNextWave(EnemyWaveDataBaseList[_currentWaveIndex].BeforeWaveInterval));
    }

    public void StartNextWave()
    {
        /// <summary>
        /// 次のウェーブを開始する
        /// </summary>
        
        if (CurrentWaveState != EnemyWaveState.Waiting) return;
        if (_currentWaveIndex == EnemyWaveDataBaseList.Count - 1)
        {
            _isLastWave = true;
        }

        StartCoroutine(SpawnEnemiesInWave(EnemyWaveDataBaseList[_currentWaveIndex]));
    }

    private IEnumerator SpawnEnemiesInWave(EnemyWaveDataBase waveData)
    {
        /// <summary>
        /// ウェーブ内の敵をスポーンするコルーチン
        /// </summary>

        int spawnedCount = waveData.EnemyTotalCount;
        int enemiesPerSpawn = waveData.MaxEnemyAppearanceCount;

        yield return new WaitForSeconds(waveData.SpawnInterval);
        CurrentWaveState = EnemyWaveState.Spawning;

        while (spawnedCount > 0)
        {   
            // 最後のスポーンの時
            if (spawnedCount <= enemiesPerSpawn)
            {
                GenerateEnemies(spawnedCount);
                yield break;
            }

            GenerateEnemies(enemiesPerSpawn);
            spawnedCount -= enemiesPerSpawn;
            yield return new WaitForSeconds(waveData.SpawnInterval);
        }

        if (CurrentWaveState == EnemyWaveState.Cleared) yield break;
        CurrentWaveState = EnemyWaveState.InProgress;
    }

    private IEnumerator WaitForNextWave(float waitTime)
    {
        CurrentWaveState = EnemyWaveState.Waiting;
        yield return new WaitForSeconds(waitTime);
        OnWaveStarted?.Invoke(EnemyWaveDataBaseList[_currentWaveIndex].WaveText);
        StartNextWave();
    }
}