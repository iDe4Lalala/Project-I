using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [SerializeField] private Camera _camera;
    [field: SerializeField] public HumanDataBase HumanDataBase { get; private set; }
    [SerializeField] private PlayerComponents _playerComponents;
    [SerializeField] private int _canJumpCount;
    [SerializeField] private string _groundTagName;
    //[SerializeField] private AudioSource footstepAudio;

    public int PlayerHP { get; private set; }
    private Quaternion _cameraRotation;
    private Quaternion _characterRotation;
    private Rigidbody _rigidbody;
    private float _footSoundTimer;
    private int _jumpCount;
    private Vector3 _joystickVector;

    public void SetCamera(Camera camera)
    {
        _camera = camera;
    }

    void Start(){
        _rigidbody = _playerComponents.Rigidbody;
        _cameraRotation = _camera.transform.localRotation;
        _characterRotation = this.gameObject.transform.localRotation;
        PlayerHP = HumanDataBase.HumanHP;
    }

    public void SetMovementInput(float x, float z)
    {
        _joystickVector = Vector3.right * x + Vector3.up * z;
        if(_joystickVector == Vector3.zero) return;
        this.gameObject.transform.position += 
            _camera.transform.forward * z * HumanDataBase.MovementSpeed + 
            _camera.transform.right * x * HumanDataBase.MovementSpeed;
        // 前はこの後に音を鳴らしていた。
    }

    public void SetRotationInput(float x, float y)
    {
        _cameraRotation *= Quaternion.Euler(-y * HumanDataBase.RotationSpeed, 0, 0);
        _characterRotation *= Quaternion.Euler(0, x * HumanDataBase.RotationSpeed, 0);
        _cameraRotation = ClampRotation(_cameraRotation);     //角度制限をつける
        _camera.transform.localRotation = _cameraRotation;
        this.gameObject.transform.localRotation = _characterRotation;
    }

    public void OnJumpButtonDown()
    {
        // ジャンプする
        if (_jumpCount >= _canJumpCount) return;
        _rigidbody.velocity = new Vector3(0, HumanDataBase.JumpForce, 0);
        _jumpCount++;
    }

    private void OnCollisionEnter(Collision col){
        // 地面についたら再度ジャンプ可能に
        if(col == null) return;
        if (col.gameObject.transform.parent.tag != _groundTagName
            && col.gameObject.transform.parent.parent.tag != _groundTagName) return;
        _jumpCount = 0;
    }

    private Quaternion ClampRotation(Quaternion q){
        // 角度制限をつける
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;
        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;
        angleX = Mathf.Clamp(angleX, HumanDataBase.TurningMinAngle, HumanDataBase.TurningMaxAngle);
        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);
        return q;
    }

    public void TakeDamage(int damage)
    {
        PlayerHP -= damage;
        Debug.Log($"Player HP: {PlayerHP}");
        if (PlayerHP <= 0){
            // 死亡時処理
        }
    }

    void respawnPointSetting(){
        int rnd = Random.Range(1, 5);
        switch (rnd){
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
