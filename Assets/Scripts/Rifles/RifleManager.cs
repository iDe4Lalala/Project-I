using UnityEngine;

public class RifleManager : MonoBehaviour
{
    /// <summary>
    /// ライフルを管理する
    /// </summary>
    
    [SerializeField] private AudioSource _shootingAudioSource;      // 射撃時のオーディオソース
    [field: SerializeField] public WeaponDataBase WeaponDataBase { get; private set; }    // 武器のデータベース

    private Camera _camera;    // 視点カメラ
    private HumanType _opponentHumanType;   // ダメージを与える相手のHumanType
    private IDamageable _opponentHuman;
    private Vector3 _rayStartPosition;      // Rayのスタート位置
    private Vector3 _rayDirection;      // Rayの方向
    private bool _isHitSomething;       // 何かに着弾したか
    private PlayerComponents _playerComponents;    // プレイヤーのコンポーネント
    private EnemyComponents _enemyComponents;      // 敵のコンポーネント
    private float _recoilX;
    private float _recoilY;
    private Vector2 _recoil;

    public void GetOwnerInfo(HumanType humanType, GameObject owner)
    {
        /// <summary>
        /// 所有者の情報を取得する
        /// </summary>

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
        /// <summary>
        /// 射撃する
        /// </summary>

        _recoilX = Random.Range(WeaponDataBase.RecoilMinX, WeaponDataBase.RecoilMaxX);
        _recoilY = Random.Range(WeaponDataBase.RecoilMinY, WeaponDataBase.RecoilMaxY);
        _recoil = new Vector2(_recoilX, _recoilY);

        // 射撃のRayを飛ばし、着弾判定
        _rayStartPosition = _camera.transform.position;
        _rayDirection = _camera.transform.forward.normalized;
        _isHitSomething = Physics.Raycast(
            _rayStartPosition, _rayDirection, out RaycastHit raycastHit, WeaponDataBase.MaximumBallisticDistance);
        
        // 射撃音を再生
        _shootingAudioSource.PlayOneShot(WeaponDataBase.ShootingAudioClip);

        // 着弾確認
        if (!_isHitSomething) return _recoil;

        // 着弾したオブジェクトが敵であるかどうか
        if (!raycastHit.collider.gameObject.name.Contains(_opponentHumanType.ToString())) return _recoil;

        // ダメージを与える
        _opponentHuman = raycastHit.collider.gameObject.GetComponent<IDamageable>();
        if(_opponentHuman == null) return _recoil;
        _opponentHuman.TakeDamage(WeaponDataBase.Damage);

        return _recoil;
    }
}
