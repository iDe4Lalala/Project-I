using UnityEngine;

public class RifleManager : MonoBehaviour
{
    /// <summary>
    /// ライフルを管理する
    /// </summary>
    
    [SerializeField] private float _MaximumBallisticDistance;    // 検出可能な最大距離
    [SerializeField] private AudioSource _shootingAudioSource;      // 射撃時のオーディオソース
    [SerializeField] private HumanType _opponentHumanType;   // ダメージを与える相手のHumanType
    [SerializeField] private Camera _camera;    // 視点カメラ
    [SerializeField] private int _rifleDamage;      // ライフルのダメージ
    private Vector3 _rayStartPosition;      // Rayのスタート位置
    private Vector3 _rayDirection;      // Rayの方向
    private bool _isHitSomething;       // 何かに着弾したか

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

        // 人への着弾時UIを表示
        // _crosshairImage.SetActive(true);

        // ダメージを与える
        Debug.Log($"PlayerHitObject: {raycastHit.collider.gameObject.name}");
        var opponentHuman = raycastHit.collider.gameObject.GetComponent<IDamageable>();
        if(opponentHuman == null) return;
        opponentHuman.TakeDamage(_rifleDamage);
    }
}
