using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RifleManager : MonoBehaviour, IWeaponCommand, IFireRuntime, IReloadRuntime
{
    [SerializeField] private WeaponDataBase _weaponDataBase;
    [SerializeField] private AudioSource _shootingAudioSource;
    public event Action<int> AmmoCountUpdated;
    public event Action EnemyWasHit;
    private ViewRifleAnimationManager _viewRifleAnimationManager;
    private Animator _reloadAnimator;
    private LayerMask _targetMask;
    private bool _isFiring;
    private float _fireCooldown;
    private int _currentAmmo;
    private bool _isReloading;
    private float _reloadTimer;
    private bool _hasInfiniteAmmo;

    private void Start()
    {
        _isFiring = false;
        _hasInfiniteAmmo = false;
        _fireCooldown = 0f;
        _currentAmmo = _weaponDataBase.MagazineCapacity;
        AmmoCountUpdated?.Invoke(_currentAmmo);
    }

    public void Initialize(LayerMask layerMask)
    {
        _targetMask = layerMask;
    }

    public void InitializeAnimator(Animator animator)
    {
        _reloadAnimator = animator;
        _viewRifleAnimationManager = new ViewRifleAnimationManager(_reloadAnimator);
    }

    public Vector2 TryFire(float deltaTime, Vector3 position, Vector3 direction)
    {
        if (!CanFireNow(deltaTime)) return Vector2.zero;

        if(!_hasInfiniteAmmo)
        {
            _currentAmmo--;
            AmmoCountUpdated?.Invoke(_currentAmmo);
        }

        _fireCooldown = 1f / _weaponDataBase.FireRate;
        return Shoot(position, direction);
    }

    private bool CanFireNow(float deltaTime)
    {
        if(_isReloading) return false;
        if(!_isFiring) return false;
        if(_currentAmmo <= 0 && !_hasInfiniteAmmo) return false;

        _fireCooldown -= deltaTime;
        if (_fireCooldown > 0f) return false;

        return true;
    }

    private Vector2 Shoot(Vector3 position, Vector3 direction)
    {
        float recoilX = Random.Range(_weaponDataBase.RecoilMinX, _weaponDataBase.RecoilMaxX);
        float recoilY = Random.Range(_weaponDataBase.RecoilMinY, _weaponDataBase.RecoilMaxY);
        var recoil = new Vector2(recoilX, recoilY);

        _shootingAudioSource.PlayOneShot(_weaponDataBase.ShootingAudioClip);

        if (Physics.Raycast(position, direction.normalized, 
            out RaycastHit hit, _weaponDataBase.MaximumBallisticDistance, _targetMask))
        {
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            damageable?.TakeDamage(_weaponDataBase.Damage);
            EnemyWasHit?.Invoke();
        }

        return recoil;
    }

    public void SetInfiniteAmmo()
    {
        _hasInfiniteAmmo = true;
    }
    
    public void StartFire() => _isFiring = true;

    public void StopFire()  => _isFiring = false;

    public void Reload()
    {
        if(_isReloading) return;
        if(_currentAmmo >= _weaponDataBase.MagazineCapacity) return;

        _isReloading = true;
        _reloadTimer = _weaponDataBase.ReloadTime;
        _viewRifleAnimationManager.SetReload();
    }

    public void UpdateReload(float deltaTime)
    {
        if (!_isReloading) return;

        _reloadTimer -= deltaTime;
        if (_reloadTimer > 0f) return;

        _isReloading = false;
        _currentAmmo = _weaponDataBase.MagazineCapacity;
        AmmoCountUpdated?.Invoke(_currentAmmo);
    }
}
