using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Wander = 0,
    Chase = 1,
    Attack = 2,
    Dead = 3
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyManager : MonoBehaviour, IDamageable
{
    [SerializeField] private EnemyComponents _enemyComponents;
    [SerializeField] private EnemyAudioController _enemyAudioController;
    [SerializeField] private EnemyAnimationContoller _enemyAnimationController;
    [SerializeField] private float _searchRange = 35f;
    [SerializeField] private float _attackRange;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _searchInterval;
    [SerializeField] private float _fireThreshold = 0.0001f;
    [SerializeField] private float _wanderRadius = 8f;
    [SerializeField] private float _wanderArrivalDistance = 0.5f;
    [SerializeField] private int _maxTargetColliders = 16;

    public event Action<GameObject, EnemyComponents> OnDied;
    public event Action OnDamaged;
    private IFireRuntime _fireRuntime;
    private EnemyState _currentState;
    private int _enemyHP;
    private Transform _currentTarget;
    private float _searchTimer;
    private Collider[] _targetColliders;

    private void Start()
    {
        SetState(EnemyState.Wander);
        _enemyComponents.NavMeshAgent.avoidancePriority = Random.Range(0, 100);
        _enemyHP = _enemyComponents.HumanDataBase.HumanHP;
        _targetColliders = new Collider[_maxTargetColliders];

        _enemyComponents.NavMeshAgent.updateRotation = true;
        _enemyComponents.NavMeshAgent.speed = _enemyComponents.HumanDataBase.MovementSpeed;
        _enemyComponents.NavMeshAgent.angularSpeed = _enemyComponents.HumanDataBase.RotationSpeed;
    }

    private void Update()
    {
        if (_currentState == EnemyState.Dead) return;

        UpdateTargetSearch(Time.deltaTime);
        if (!HasTarget())
        {
            SetState(EnemyState.Wander);
            UpdateWander();
            return;
        }

        UpdateChaseOrAttack(_currentTarget);
    }

    public void Initialize(RifleManager rifleManager)
    {
        _enemyAudioController.Initialize(_enemyComponents);
        _enemyAnimationController.Initialize(_enemyComponents);

        if(rifleManager == null) return;
        _fireRuntime = rifleManager;
        rifleManager.SetInfiniteAmmo();
    }

    public void TakeDamage(int damage)
    {
        if(_currentState == EnemyState.Dead) return;
        _enemyHP -= damage;
        OnDamaged?.Invoke();

        if (_enemyHP > 0) return;
        SetState(EnemyState.Dead);
        _enemyComponents.NavMeshAgent.isStopped = true;
        _enemyComponents.NavMeshAgent.ResetPath();
        UpdateFootstepPresentation(false);
        OnDied?.Invoke(gameObject, _enemyComponents);
    }

    private Transform FindNearestTarget()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position, _searchRange, _targetColliders, _targetLayer);

        Transform nearestTarget = null;
        float nearestTargetMagnitude = float.MaxValue;
        for (int i = 0; i < hitCount; i++)
        {
            var collider = _targetColliders[i];
            if(collider == null) continue;

            float sqr = (collider.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestTargetMagnitude)
            {
                nearestTargetMagnitude = sqr;
                nearestTarget = collider.transform;
            }
        }

        // hitCount == _targetColliders.Lengthが成り立つようなら、バッファ不足なので_maxTargetCollidersを増やす必要があり
        return nearestTarget;
    }

    private void UpdateTargetSearch(float deltaTime)
    {
        _searchTimer += deltaTime;
        if (_currentTarget != null && _searchTimer < _searchInterval) return;

        _currentTarget = FindNearestTarget();
        _searchTimer = 0f;
    }

    private bool HasTarget()
    {
        return _currentTarget != null;
    }

    private void UpdateWander()
    {
        _enemyComponents.NavMeshAgent.isStopped = false;

        // 目的地がない or 到着済みなら次の放浪先を作る
        if (!_enemyComponents.NavMeshAgent.hasPath ||
            _enemyComponents.NavMeshAgent.remainingDistance <= _wanderArrivalDistance)
        {
            SetRandomWanderDestination();
        }

        _enemyAnimationController.UpdateAnimatorSpeed(true);
        _enemyAudioController.UpdateFootstep(true);
    }

    private void UpdateChaseOrAttack(Transform currentTarget)
    {
        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > _attackRange)
        {
            SetState(EnemyState.Chase);
            UpdateFootstepPresentation(true);
            _enemyComponents.NavMeshAgent.isStopped = false;
            _enemyComponents.NavMeshAgent.SetDestination(currentTarget.position);
        }
        else
        {
            SetState(EnemyState.Attack);
            UpdateFootstepPresentation(false);
            _enemyComponents.NavMeshAgent.isStopped = true;

            Vector3 direction = currentTarget.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < _fireThreshold) return;
            transform.rotation = Quaternion.LookRotation(direction);

            if (_fireRuntime == null) return;
            direction.Normalize();
                _fireRuntime.TryFire(Time.deltaTime, _enemyComponents.Camera.transform.position, direction);
        }
    }

    private void UpdateFootstepPresentation(bool isMoving)
    {
        _enemyAudioController.UpdateFootstep(isMoving);
        _enemyAnimationController.UpdateAnimatorSpeed(isMoving);
    }

    private void SetRandomWanderDestination()
    {
        Vector2 randomPoint = Random.insideUnitCircle * _wanderRadius;
        Vector3 candidate = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _wanderRadius, NavMesh.AllAreas))
        {
            _enemyComponents.NavMeshAgent.SetDestination(hit.position);
        }
    }

    private void SetState(EnemyState nextState)
    {
        if(_currentState == nextState) return;
        _currentState = nextState;
    }
}
