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
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _fireThreshold;
    [SerializeField] private float _searchRange;
    [SerializeField] private float _searchInterval;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackShotInterval;
    [SerializeField] private float _wanderRadius;
    [SerializeField] private float _wanderArrivalDistance;
    [SerializeField] private int _maxTargetColliders;
    [SerializeField] private float _aimSpreadDegrees;
    public event Action<EnemyComponents> OnDied;
    public event Action OnDamaged;
    private IFireRuntime _fireRuntime;
    private IWeaponCommand _weaponCommand;
    private EnemyState _currentState;
    private int _enemyHP;
    private Transform _currentTarget;
    private float _searchTimer;
    private float _shotCooldownTimer;
    private Collider[] _targetColliders;

    private void Start()
    {
        SetState(EnemyState.Wander);
        _enemyComponents.NavMeshAgent.avoidancePriority = Random.Range(0, 100);
        _shotCooldownTimer = _attackShotInterval;
        _enemyHP = _enemyComponents.HumanDataBase.HumanHP;
        _targetColliders = new Collider[_maxTargetColliders];

        _enemyComponents.NavMeshAgent.updateRotation = true;
        _enemyComponents.NavMeshAgent.speed = _enemyComponents.HumanDataBase.MovementSpeed;
        _enemyComponents.NavMeshAgent.angularSpeed = _enemyComponents.HumanDataBase.RotationSpeed;
    }

    private void Update()
    {
        if (_currentState == EnemyState.Dead) return;
        _shotCooldownTimer = Mathf.Max(0f, _shotCooldownTimer - Time.deltaTime);

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
        if (rifleManager == null) return;
        _fireRuntime = rifleManager;
        _weaponCommand = rifleManager;

        rifleManager.Initialize(_targetLayer);
        rifleManager.SetInfiniteAmmo();
        _enemyAudioController.Initialize(_enemyComponents);
        _enemyAnimationController.Initialize(_enemyComponents);

        SetState(EnemyState.Wander);
    }

    public void TakeDamage(int damage)
    {
        if (_currentState == EnemyState.Dead) return;
        _enemyHP -= damage;
        OnDamaged?.Invoke();

        if (_enemyHP > 0) return;
        SetState(EnemyState.Dead);
        _weaponCommand.StopFire();
        _enemyComponents.NavMeshAgent.isStopped = true;
        _enemyComponents.NavMeshAgent.ResetPath();
        UpdateFootstepPresentation(false);
        OnDied?.Invoke(_enemyComponents);
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
            if (collider == null) continue;

            float sqr = (collider.transform.position - transform.position).sqrMagnitude;
            if (sqr < nearestTargetMagnitude)
            {
                nearestTargetMagnitude = sqr;
                nearestTarget = collider.transform;
            }
        }

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

        if (!_enemyComponents.NavMeshAgent.hasPath ||
            _enemyComponents.NavMeshAgent.remainingDistance <= _wanderArrivalDistance)
        {
            SetState(EnemyState.Wander);
            _weaponCommand.StopFire();
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
            _weaponCommand.StopFire();
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
            TrySingleShot(direction);
        }
    }

    private void TrySingleShot(Vector3 direction)
    {
        if (_shotCooldownTimer > 0f) return;
        if (_weaponCommand == null || _fireRuntime == null) return;

        _weaponCommand.StartFire();
        direction = ApplyAimSpreed(direction);
        Vector2 recoil = _fireRuntime.TryFire(
            Time.deltaTime, _enemyComponents.Camera.transform.position, direction);
        _weaponCommand.StopFire();

        if (recoil != Vector2.zero)
        {
            _shotCooldownTimer = _attackShotInterval;
        }
    }

    private Vector3 ApplyAimSpreed(Vector3 direction)
    {
        float yaw = Random.Range(-_aimSpreadDegrees, _aimSpreadDegrees);
        float pitch = Random.Range(-_aimSpreadDegrees, _aimSpreadDegrees);
        return Quaternion.Euler(pitch, yaw, 0f) * direction;
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
        if (_currentState == nextState) return;
        _currentState = nextState;
    }
}