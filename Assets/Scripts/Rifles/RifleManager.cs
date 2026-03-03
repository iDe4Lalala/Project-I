using UnityEngine;

public class RifleManager : MonoBehaviour, IWeaponCommand, IFireRuntime, IReloadRuntime
{
    [field: SerializeField] public WeaponDataBase WeaponDataBase { get; private set; }
    [SerializeField] private Animator _reloadAnimator;
    [SerializeField] private AudioSource _shootingAudioSource;

    private ViewRifleAnimationManager _viewRifleAnimationManager;
    private LayerMask _targetMask;
    private bool _isFiring;
    private float _fireCooldown;
    private int _currentAmmo;
    private bool _isReloading;
    private float _reloadTimer;
    private bool _hasInfiniteAmmo;


    private void Start()
    {
        _viewRifleAnimationManager = new ViewRifleAnimationManager(_reloadAnimator);
        _isFiring = false;
        _hasInfiniteAmmo = false;
        _fireCooldown = 0f;
        _currentAmmo = WeaponDataBase.MagazineCapacity;
    }

    public void Initialize(LayerMask layerMask)
    {
        _targetMask = layerMask;
    }

    public Vector2 TryFire(float deltaTime, Vector3 position, Vector3 direction)
    {
        if (CanFireNow(deltaTime)) return Vector2.zero;

        if(!_hasInfiniteAmmo)
        {
            _currentAmmo--;
        }

        _fireCooldown = 1f / WeaponDataBase.FireRate;
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
        float recoilX = Random.Range(WeaponDataBase.RecoilMinX, WeaponDataBase.RecoilMaxX);
        float recoilY = Random.Range(WeaponDataBase.RecoilMinY, WeaponDataBase.RecoilMaxY);
        var recoil = new Vector2(recoilX, recoilY);

        _shootingAudioSource.PlayOneShot(WeaponDataBase.ShootingAudioClip);

        if (Physics.Raycast(position, direction.normalized, 
            out RaycastHit hit, WeaponDataBase.MaximumBallisticDistance, _targetMask))
        {
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            damageable?.TakeDamage(WeaponDataBase.Damage);
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
        if(_currentAmmo >= WeaponDataBase.MagazineCapacity) return;

        _isReloading = true;
        _reloadTimer = WeaponDataBase.ReloadTime;
        _viewRifleAnimationManager.SetReload();
    }

    public void UpdateReload(float deltaTime)
    {
        if (!_isReloading) return;

        _reloadTimer -= deltaTime;
        if (_reloadTimer > 0f) return;

        _isReloading = false;
        _currentAmmo = WeaponDataBase.MagazineCapacity;
    }
}
