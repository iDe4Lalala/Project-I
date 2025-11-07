using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RifleManager : MonoBehaviour
{
    [SerializeField] private float _MaximumBallisticDistance;    // 検出可能な最大距離
    [SerializeField] private AudioSource _shootingAudioSource;
    [SerializeField] private HumanType _opponentHumanType;   // ダメージを与える相手のHumanType
    [SerializeField] private Camera _camera;
    [SerializeField] private int _rifleDamage;
    private Vector3 _rayStartPosition;
    private Vector3 _rayDirection;
    private bool _isHitSomething;

    public void ShootByRifle(){
        _rayStartPosition = _camera.transform.position;
        _rayDirection = _camera.transform.forward.normalized;
        _isHitSomething = Physics.Raycast(
            _rayStartPosition, _rayDirection, out RaycastHit raycastHit, _MaximumBallisticDistance);
        // Debug.DrawRay(_rayStartPosition, _rayDirection * _MaximumBallisticDistance, Color.red);
        _shootingAudioSource.Play();

        if (!_isHitSomething)return;
        if (!raycastHit.collider.gameObject.name.Contains(_opponentHumanType.ToString())) return;
        // _crosshairImage.SetActive(true);
        Debug.Log($"PlayerHitObject: {raycastHit.collider.gameObject.name}");
        var opponentHuman = raycastHit.collider.gameObject.GetComponent<IDamageable>();
        if(opponentHuman == null) return;
        opponentHuman.TakeDamage(_rifleDamage);
    }
}
