using UnityEngine;

public class RifleManager : MonoBehaviour
{
    [field: SerializeField] public WeaponDataBase WeaponDataBase { get; private set; }
    [SerializeField] private AudioSource _shootingAudioSource;

    private Camera _camera;
    private HumanType _opponentHumanType;
    private IDamageable _opponentHuman;
    private Vector3 _rayStartPosition;
    private Vector3 _rayDirection;
    private bool _isHitSomething;
    private PlayerComponents _playerComponents;
    private EnemyComponents _enemyComponents;
    private float _recoilX;
    private float _recoilY;
    private Vector2 _recoil;

    public void GetOwnerInfo(HumanType humanType, GameObject owner)
    {
        switch (humanType)
        {
            case HumanType.Player:
                {
                    _playerComponents = owner.GetComponent<PlayerComponents>();
                    _camera = _playerComponents.Camera;
                    _opponentHumanType = HumanType.Enemy;
                }
                break;
            case HumanType.Enemy:
                {
                    _enemyComponents = owner.GetComponent<EnemyComponents>();
                    _camera = _enemyComponents.Camera;
                    _opponentHumanType = HumanType.Player;
                }
                break;
        }
    }

    public Vector2 ShootByRifle()
    {
        _recoilX = Random.Range(WeaponDataBase.RecoilMinX, WeaponDataBase.RecoilMaxX);
        _recoilY = Random.Range(WeaponDataBase.RecoilMinY, WeaponDataBase.RecoilMaxY);
        _recoil = new Vector2(_recoilX, _recoilY);

        _rayStartPosition = _camera.transform.position;
        _rayDirection = _camera.transform.forward.normalized;
        _isHitSomething = Physics.Raycast(
            _rayStartPosition, _rayDirection, out RaycastHit raycastHit, WeaponDataBase.MaximumBallisticDistance);
        
        _shootingAudioSource.PlayOneShot(WeaponDataBase.ShootingAudioClip);

        if (!_isHitSomething) return _recoil;

        if (!raycastHit.collider.gameObject.name.Contains(_opponentHumanType.ToString())) return _recoil;

        _opponentHuman = raycastHit.collider.gameObject.GetComponent<IDamageable>();
        if(_opponentHuman == null) return _recoil;
        _opponentHuman.TakeDamage(WeaponDataBase.Damage);

        return _recoil;
    }
}
