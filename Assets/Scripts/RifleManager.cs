using UnityEngine;

public class RifleManager : MonoBehaviour
{
    /// <summary>
    /// ライフルを管理する
    /// </summary>
    
    [SerializeField] private float _MaximumBallisticDistance;    // 検出可能な最大距離
    [SerializeField] private AudioSource _shootingAudioSource;      // 射撃時のオーディオソース
    [SerializeField] private int _rifleDamage;      // ライフルのダメージ

    private Camera _camera;    // 視点カメラ
    private HumanType _opponentHumanType;   // ダメージを与える相手のHumanType
    private IDamageable _opponentHuman;
    private Vector3 _rayStartPosition;      // Rayのスタート位置
    private Vector3 _rayDirection;      // Rayの方向
    private bool _isHitSomething;       // 何かに着弾したか
    private PlayerComponents _playerComponents;    // プレイヤーのコンポーネント
    private EnemyComponents _enemyComponents;      // 敵のコンポーネント

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

    public void ShootByRifle(){
        /// <summary>
        /// 射撃する
        /// </summary>
        
        // 射撃のRayを飛ばし、着弾判定
        _rayStartPosition = _camera.transform.position;
        _rayDirection = _camera.transform.forward.normalized;
        _isHitSomething = Physics.Raycast(
            _rayStartPosition, _rayDirection, out RaycastHit raycastHit, _MaximumBallisticDistance);
        
        // 射撃音を再生
        _shootingAudioSource.Play();

        // 着弾確認
        if (!_isHitSomething) return;

        // 着弾したオブジェクトが敵であるかどうか
        if (!raycastHit.collider.gameObject.name.Contains(_opponentHumanType.ToString())) return;

        // ダメージを与える
        _opponentHuman = raycastHit.collider.gameObject.GetComponent<IDamageable>();
        if(_opponentHuman == null) return;
        _opponentHuman.TakeDamage(_rifleDamage);
    }
}
