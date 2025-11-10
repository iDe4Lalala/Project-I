using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour, IDamageable
{
    /// <summary>
    ///  敵を管理する
    /// </summary>

    [SerializeField] private int _enemyHP;      // 敵のHP
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _wanderRange;    // 索敵範囲
    [SerializeField] private RifleManager _rifleManager;
    [SerializeField] private string _playerCompareTag;    // プレイヤーのタグ名
    [SerializeField] private float _searchInterval;     // 索敵間隔

    private float _timeCount;   // 索敵用タイマー
    private float _deltaTime;   // deltaTime保存用

    void Start(){
        _agent.avoidancePriority = Random.Range(0, 100);
    }

    void Update(){
        // 索敵の経過時間と移動処理
        _deltaTime = Time.deltaTime;
        _timeCount += _deltaTime;
        transform.position += transform.forward * _deltaTime;

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
            _agent.SetDestination(player.transform.position);
            _rifleManager.ShootByRifle();
        }
        else    // プレイヤーが索敵範囲内にいなければ
        {
            // 移動をやめて、ランダムな方向を向く
            _agent.ResetPath();
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
            // Destroy(gameObject);
    }
}
