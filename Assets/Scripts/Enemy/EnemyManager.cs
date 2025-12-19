using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour, IDamageable
{
    /// <summary>
    /// 敵を管理する
    /// </summary>

    [SerializeField] private EnemyComponents _enemyComponents;
    [SerializeField] private float _wanderRange;    // 索敵範囲
    [SerializeField] private float _attackRange;    // 攻撃範囲
    [SerializeField] private string _playerCompareTag;    // プレイヤーのタグ名
    [SerializeField] private float _searchInterval;     // 索敵間隔

    private int _enemyHP;
    public event Action<GameObject, EnemyComponents> OnDied;  // 死亡時イベント
    public event Action OnDamaged;   // ダメージを受けた時のイベント
    private float _timer;
    private float _deltaTime; 

    void Start()
    {
        _enemyComponents.NavMeshAgent.avoidancePriority = Random.Range(0, 100);
        _enemyHP = _enemyComponents.HumanDataBase.HumanHP;

        _enemyComponents.NavMeshAgent.updateRotation = true;
        _enemyComponents.NavMeshAgent.speed = _enemyComponents.HumanDataBase.MovementSpeed;
        _enemyComponents.NavMeshAgent.angularSpeed = _enemyComponents.HumanDataBase.RotationSpeed;
    }

    void Update()
    {
        _deltaTime = Time.deltaTime;
        _timer += _deltaTime;
        
        _enemyComponents.Animator.SetFloat("speed", _enemyComponents.NavMeshAgent.velocity.magnitude);

        // 一定時間ごとに索敵
        if (_timer <= _searchInterval) return;

        SearchForPlayer();
        _timer = 0;
    }

    private void SearchForPlayer()
    {
        /// <summary>
        /// プレイヤーの位置を取得
        /// </summary>

        GameObject player = null;
        
        // 索敵範囲のhitColliderを作成
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _wanderRange);
        
        // hitCollider内のプレイヤーを探す
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(_playerCompareTag))
            {
                player = hitCollider.gameObject;
                break;
            }
        }

        if (player == null)
        {
            // 移動をやめて、ランダムな方向を向く
            _enemyComponents.NavMeshAgent.ResetPath();
            var course = new Vector3(0, Random.Range(0, 60), 0);
            transform.localRotation = Quaternion.Euler(course);
            return;
        }

        // プレイヤーとの距離を算出
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > _attackRange)
        {
            // 追跡
            _enemyComponents.NavMeshAgent.isStopped = false;
            _enemyComponents.NavMeshAgent.SetDestination(player.transform.position);

            // 足音を再生
            if (!_enemyComponents.FootstepAudioSource.isPlaying)
            {
                _enemyComponents.FootstepAudioSource.Play();
            }
        }
        else
        {
            // 攻撃
            _enemyComponents.NavMeshAgent.isStopped = true;
            if (_enemyComponents.FootstepAudioSource.isPlaying)
            {
                _enemyComponents.FootstepAudioSource.Stop();
            }

            var rand =  Quaternion.Euler(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f));
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction) * rand;

            _enemyComponents.RifleManager.ShootByRifle();
        }
    }


    public void TakeDamage(int damage)
    {
        _enemyHP -= damage;
        OnDamaged?.Invoke();

        if (_enemyHP > 0) return;
        // 死亡時処理
        OnDied?.Invoke(gameObject, _enemyComponents);
    }
}
