using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour, IDamageable
{
    /// <summary>
    /// プレイヤーを管理する
    /// </summary>
    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private int _canJumpCount;     // ジャンプ可能回数
    [SerializeField] private string _groundTagName;     // 地面のタグ名
    [SerializeField] private float _footSoundThreshold;   // 足音を鳴らす速度の閾値
    [SerializeField] private float _movementThreshold;
    [SerializeField] private ViewRifleAnimationManager _viewRifleAnimationManager;

    public int PlayerHP { get; private set; }       // プレイヤーの体力
    private Quaternion _cameraRotation;     // カメラの回転保存用
    private Quaternion _characterRotation;      // キャラクターの回転保存用
    private Rigidbody _rigidbody;
    private int _jumpCount;   // 現在のジャンプ回数
    private Vector3 _joystickVector;    // ジョイスティックの入力保存用
    private Transform _headRotation;
    private bool _isMoving;
    private int _ammoCount;
    private float _shootTimer;
    private Vector2 _recoil;
    private Vector2 _currentRecoil;
    private Vector2 _targetRecoil;
    public event Action OnDamaged;   // プレイヤーがダメージを受けた時のイベント
    public event Action<GameObject> OnDied;   // プレイヤーが死亡した時のイベント

    void Start()
    {
        // ステータス・コンポーネントの取得
        _rigidbody = _playerComponents.Rigidbody;
        PlayerHP = _playerComponents.HumanDataBase.HumanHP;
        _cameraRotation = _playerComponents.Camera.transform.localRotation;
        _characterRotation = gameObject.transform.localRotation;
        _headRotation = _playerComponents.Animator.GetBoneTransform(HumanBodyBones.Head);
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;
    }

    public void SetMovementInput(float x, float z)
    {
        /// <summary>
        /// 移動処理
        /// </summary>
        
        _isMoving = Mathf.Abs(x) > _movementThreshold || Mathf.Abs(z) > _movementThreshold;
        _playerComponents.Animator.SetBool("IsMoving", _isMoving);

        // ジョイスティック入力をベクトルに変換
        _joystickVector = Vector3.right * x + Vector3.up * z;

        // 足音を再生
        if (_joystickVector.magnitude > _footSoundThreshold && !_playerComponents.FootstepAudioSource.isPlaying)
        {
            _playerComponents.FootstepAudioSource.PlayOneShot(_playerComponents.FootstepAudioClip);
        }
        else if (_joystickVector.magnitude <= _footSoundThreshold && _playerComponents.FootstepAudioSource.isPlaying)
        {
            _playerComponents.FootstepAudioSource.Stop();
        }

        if (_joystickVector == Vector3.zero) return;
        // カメラの向きに合わせてプレイヤーを移動
        gameObject.transform.position += 
            _playerComponents.HumanDataBase.MovementSpeed * z * _playerComponents.Camera.transform.forward + 
            _playerComponents.HumanDataBase.MovementSpeed * x * _playerComponents.Camera.transform.right;
    }

    public void SetRotationInput(float x, float y, Vector2 recoil = default)
    {
        /// <summary>
        /// 回転処理
        /// </summary>

        // カメラとプレイヤーの回転を検出
        _cameraRotation *= Quaternion.Euler(-y * _playerComponents.HumanDataBase.RotationSpeed - recoil.y, 0, 0);
        _characterRotation *= Quaternion.Euler(0, x * _playerComponents.HumanDataBase.RotationSpeed + recoil.x, 0);

        // 角度制限をつけて回転を適用
        _cameraRotation = ClampRotation(_cameraRotation);
        _playerComponents.Camera.transform.localRotation = _cameraRotation;
        gameObject.transform.localRotation = _characterRotation;
    }

    public void OnJumpButtonDown()
    {
        /// <summary>
        /// ジャンプ処理
        /// </summary>

        if (_jumpCount >= _canJumpCount) return;
        _rigidbody.linearVelocity = new Vector3(0, _playerComponents.HumanDataBase.JumpForce, 0);
        _jumpCount++;

        _playerComponents.Animator.SetTrigger("Jump");
        if (!_playerComponents.Animator.GetBool("IsGround")) return;
        _playerComponents.Animator.SetBool("IsGround", false);
    }

    private void OnCollisionEnter(Collision col){
        /// <summary>
        /// 地面についたら再度ジャンプ可能に
        /// </summary>

        if (col == null) return;

        // プレイヤーが接地しているオブジェクトの親の親まで確認
        if (col.gameObject.transform.parent.tag != _groundTagName
            && col.gameObject.transform.parent.parent.tag != _groundTagName) return;
        _jumpCount = 0;

        if (_playerComponents.Animator.GetBool("IsGround")) return;
        _playerComponents.Animator.SetBool("IsGround", true);
    }

    private Quaternion ClampRotation(Quaternion q){
        /// <summary>
        /// 回転の角度制限
        /// </summary>
        
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;
        
        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX, _playerComponents.HumanDataBase.TurningMinAngle, _playerComponents.HumanDataBase.TurningMaxAngle);
        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
        return q;
    }

    public void TakeDamage(int damage)
    {
        /// <summary>
        /// 被ダメージ処理
        /// </summary>
        
        PlayerHP -= damage;
        OnDamaged?.Invoke();

        // 死亡時処理
        if (PlayerHP <= 0)
        {
            OnDied?.Invoke(gameObject);
        }
    }

    private void Update()
    {
        _shootTimer += Time.deltaTime;
    }

    public void CheckCanShoot()
    {
        if (_shootTimer <= 1f / _playerComponents.RifleManager.WeaponDataBase.FireRate) return;
        _shootTimer = 0;
        if (_ammoCount <= 0) return;
        _ammoCount--;

        _recoil = _playerComponents.RifleManager.ShootByRifle();
        SetRotationInput(0, 0, _recoil);
    }

    public void Reload()
    {
        _viewRifleAnimationManager.PlayReloadAnimation();
    }

    public void FinishedReload()
    {
        _ammoCount = _playerComponents.RifleManager.WeaponDataBase.MagazineCapacity;
    }
}
