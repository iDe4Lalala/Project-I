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
    [SerializeField] private int _enemyHP;      // 敵のHP
    [SerializeField] private float _wanderRange;    // 索敵範囲
    [SerializeField] private string _playerCompareTag;    // プレイヤーのタグ名
    [SerializeField] private float _searchInterval;     // 索敵間隔

    public event Action<GameObject> OnDeath;  // 死亡時イベント
    private float _timeCount;   // 索敵用タイマー
    private float _deltaTime;   // deltaTime保存用
    private Vector3 _speed;

    void Start()
    {
        _enemyComponents.NavMeshAgent.avoidancePriority = Random.Range(0, 100);
    }

    void Update()
    {
        // 索敵の経過時間と移動処理
        _deltaTime = Time.deltaTime;
        _timeCount += _deltaTime;
        _speed = transform.forward * _enemyComponents.HumanDataBase.MovementSpeed;
        transform.position += _speed;
        
        _enemyComponents.Animator.SetFloat("speed", _speed.magnitude);

        // アニメーションに応じて手の向きを調整
        if (_enemyComponents.Animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            _enemyComponents.EnemyHand.transform.localRotation = Quaternion.Euler(51.276f, -103.915f, 22.893f);
        }

        // 一定時間ごとに索敵
        if (_timeCount <= _searchInterval) return;
        SearchForPlayer();
        _timeCount = 0;
    }

    private void SearchForPlayer()
    {
        /// <summary>
        /// プレイヤーの位置を取得
        /// </summary>

        GameObject player = null;
        
        // 索敵範囲のhitColliderを作成
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _wanderRange);
        foreach (var hitCollider in hitColliders)
        {
            // hitCollider内のプレイヤーを探す
            if (hitCollider.CompareTag(_playerCompareTag))
            {
                player = hitCollider.gameObject;
                break;
            }
        }

        if (player != null)     // プレイヤーが索敵範囲内にいれば
        {
            // プレイヤーの方向を算出
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            directionToPlayer.y = 0;

            if (directionToPlayer == Vector3.zero) return;

            // プレイヤーの方向を向き、移動し、射撃する
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            _enemyComponents.NavMeshAgent.SetDestination(player.transform.position);
            // _enemyComponents.RifleManager.ShootByRifle();
        }
        else    // プレイヤーが索敵範囲内にいなければ
        {
            // 移動をやめて、ランダムな方向を向く
            _enemyComponents.NavMeshAgent.ResetPath();
            var course = new Vector3(0, Random.Range(0, 180), 0);
            transform.localRotation = Quaternion.Euler(course);
        }
    }

    public void TakeDamage(int damage)
    {
        _enemyHP -= damage;
        Debug.Log($"Enemy HP: {_enemyHP}");
        if (_enemyHP > 0) return;
            // 死亡時処理
            OnDeath?.Invoke(gameObject);
    }
}
