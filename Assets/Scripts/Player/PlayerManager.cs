using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    /// <summary>
    /// プレイヤーを管理する
    /// </summary>
    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private int _canJumpCount;     // ジャンプ可能回数
    [SerializeField] private string _groundTagName;     // 地面のタグ名

    public int PlayerHP { get; private set; }       // プレイヤーの体力
    private Quaternion _cameraRotation;     // カメラの回転保存用
    private Quaternion _characterRotation;      // キャラクターの回転保存用
    private Rigidbody _rigidbody;
    private int _jumpCount;   // 現在のジャンプ回数
    private Vector3 _joystickVector;    // ジョイスティックの入力保存用


    void Start()
    {
        // ステータス・コンポーネントの取得
        _rigidbody = _playerComponents.Rigidbody;
        _cameraRotation = _playerComponents.Camera.transform.localRotation;
        _characterRotation = gameObject.transform.localRotation;
        PlayerHP = _playerComponents.HumanDataBase.HumanHP;
    }

    public void SetMovementInput(float x, float z)
    {
        /// <summary>
        /// 移動処理
        /// </summary>

        // ジョイスティック入力をベクトルに変換
        _joystickVector = Vector3.right * x + Vector3.up * z;

        // アニメーションの速度パラメーターを更新
        _playerComponents.Animator.SetFloat("speed", _joystickVector.magnitude);

        if (_joystickVector == Vector3.zero) return;
        // カメラの向きに合わせてプレイヤーを移動
        gameObject.transform.position += 
            _playerComponents.HumanDataBase.MovementSpeed * z * _playerComponents.Camera.transform.forward + 
            _playerComponents.HumanDataBase.MovementSpeed * x * _playerComponents.Camera.transform.right;
        // 前はこの後に音を鳴らしていた。
    }

    public void SetRotationInput(float x, float y)
    {
        /// <summary>
        /// 回転処理
        /// </summary>

        // カメラとプレイヤーの回転を検出
        _cameraRotation *= Quaternion.Euler(-y * _playerComponents.HumanDataBase.RotationSpeed, 0, 0);
        _characterRotation *= Quaternion.Euler(0, x * _playerComponents.HumanDataBase.RotationSpeed, 0);

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

        _playerComponents.Animator.SetBool("isJumping", true);

        _jumpCount++;
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

        _playerComponents.Animator.SetBool("isJumping", false);
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
        Debug.Log($"Player HP: {PlayerHP}");
        
        if (PlayerHP <= 0){    // 死亡時処理
        }
    }

    private void respawnPointSetting()
    {
        /// <summary>
        /// リスポーンポイント設定
        /// </summary>

        int rnd = Random.Range(1, 5);

        switch (rnd)
        {
            case 1:
                transform.position = new Vector3(9.8f, 2.0f, 23.1f);
                break;
            case 2:
                transform.position = new Vector3(-3.5f, 2.0f, -46.7f);
                break;
            case 3:
                transform.position = new Vector3(44.2f, 2.0f, -17.8f);
                break;
            case 4:
                transform.position = new Vector3(69.7f, 2.4f, 21.4f);
                break;
            default:
                transform.position = new Vector3(88.96f, 2.2f, 82.8f);
                break;
        }
    }
}
