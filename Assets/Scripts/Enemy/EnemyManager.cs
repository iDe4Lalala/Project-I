using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour, IDamageable
{
    [SerializeField] private int _enemyHP;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private float _wanderRange;
    [SerializeField] private RifleManager _rifleManager;
    [SerializeField] private string _playerCompareTag;
    [SerializeField] private float _searchInterval;

    private float _timeCount;

    void Start(){
        _agent.avoidancePriority = Random.Range(0, 100);
    }

    void Update(){
        _timeCount += Time.deltaTime;
        transform.position += transform.forward * Time.deltaTime;

        if (_timeCount <= _searchInterval) return;
        SearchForPlayer();
        _timeCount = 0;
    }

    private void SearchForPlayer()
    {
        // プレイヤーの位置を取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _wanderRange);
        GameObject player = null;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(_playerCompareTag))
            {
                player = hitCollider.gameObject;
                break;
            }
        }

        if(player != null)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            directionToPlayer.y = 0;
            if(directionToPlayer == Vector3.zero) return;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            _agent.SetDestination(player.transform.position);
            _rifleManager.ShootByRifle();
        }
        else
        {   
            _agent.ResetPath();
            Vector3 course = new Vector3(0, Random.Range(0, 180), 0);
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
