using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyComponents _enemyComponents;
    [SerializeField] private float _wanderRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private string _playerCompareTag;
    [SerializeField] private float _searchInterval;

    public event Action<GameObject, EnemyComponents> OnDied;
    public event Action OnDamaged;
    private int _enemyHP;
    private float _timer;
    private float _deltaTime; 
    private float _shootTimer;

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
        _shootTimer += _deltaTime;
        
        _enemyComponents.Animator.SetFloat("Speed", _enemyComponents.NavMeshAgent.velocity.magnitude);

        if (_timer <= _searchInterval) return;

        FindAndSearchPlayer();
        _timer = 0;
    }

    public void TakeDamage(int damage)
    {
        _enemyHP -= damage;
        OnDamaged?.Invoke();

        if (_enemyHP > 0) return;
        OnDied?.Invoke(gameObject, _enemyComponents);
    }

    private void FindAndSearchPlayer()
    {
        GameObject player = null;
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _wanderRange);
        
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
            _enemyComponents.NavMeshAgent.ResetPath();
            var course = new Vector3(0, Random.Range(0, 60), 0);
            transform.localRotation = Quaternion.Euler(course);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > _attackRange)
        {
            _enemyComponents.NavMeshAgent.isStopped = false;
            _enemyComponents.NavMeshAgent.SetDestination(player.transform.position);

            if (!_enemyComponents.FootstepAudioSource.isPlaying)
            {
                _enemyComponents.FootstepAudioSource.PlayOneShot(_enemyComponents.FootstepAudioClip);
            }
        }
        else
        {
            _enemyComponents.NavMeshAgent.isStopped = true;
            if (_enemyComponents.FootstepAudioSource.isPlaying)
            {
                _enemyComponents.FootstepAudioSource.Stop();
            }

            var rand =  Quaternion.Euler(Random.Range(-15f, 15f), 0, Random.Range(-15f, 15f));
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction) * rand;

            _ = _enemyComponents.RifleManager.ShootByRifle();
        }
    }
}
