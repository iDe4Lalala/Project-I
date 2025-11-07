using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Collider))]
public class EnemyCollision : MonoBehaviour
{
    // [SerializeField] private GameObject _enemy;
    // [SerializeField] private GameObject _player;
    // [SerializeField] private EnemyRifle _enemyRifleScript;
    // [SerializeField] private string _playerCompareTag;
    // private NavMeshAgent _navMeshAgent;
    // private Player _playerScript;
    // private bool _isShooting = false;
    // private float _timer = 0f;

    // private void Start(){
    //     _playerScript = _player.GetComponent<Player>();
    // }

    // private void Update(){
    //     if (_isShooting == true){
    //         _timer += Time.deltaTime;
    //         _enemy.transform.LookAt(_player.transform);
    //         if (_timer > 1 && _playerScript.playerHP > 0){
    //             _enemyRifleScript.ShootingEnemy();
    //             _timer = 0;
    //         }
            
    //     }
    // }

    // private void OnTriggerEnter(Collider other){
    //     if (other.CompareTag(_playerCompareTag)){
    //         _isShooting = true;
    //         _navMeshAgent = _enemy.GetComponent<NavMeshAgent>();
    //         _navMeshAgent.destination = other.transform.position;
    //         //target = other.transform;
    //     }
    // }

    // void OnTriggerExit(Collider other){
    //     if (other.CompareTag(_playerCompareTag)){
    //         _isShooting = false;
    //         _navMeshAgent = _enemy.GetComponent<NavMeshAgent>();
    //         _navMeshAgent.ResetPath();
    //     }
    // }
}
