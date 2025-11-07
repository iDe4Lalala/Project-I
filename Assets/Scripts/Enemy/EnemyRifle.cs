using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRifle : MonoBehaviour
{
    // //[SerializeField] Camera fpsCam;             // カメラ
    // [SerializeField] float distance = 50.0f;    // 検出可能な距離
    // //public AudioClip ARsound;
    // AudioSource RifleAudio;
    // public GameObject player;
    // Player playerScript;

    // void Start(){
    //     playerScript = player.GetComponent<Player>();
    //     RifleAudio = this.gameObject.GetComponent<AudioSource>();
    // }

    // void Update(){
        
    // }

    // public void ShootingEnemy(){
    //     // Rayはカメラの位置からとばす
    //     var rayStartPosition = this.gameObject.transform.position;
    //     // Rayはカメラが向いてる方向にとばす
    //     var rayDirection = -this.gameObject.transform.forward.normalized;

    //     // Hitしたオブジェクト格納用
    //     RaycastHit raycastHit;

    //     // Rayを飛ばす（out raycastHit でHitしたオブジェクトを取得する）
    //     var isHit = Physics.Raycast(rayStartPosition, rayDirection, out raycastHit, distance);

    //     //GameObject ARsoundClone = Instantiate(ARsound) as GameObject;
    //     //Destroy(ARsoundClone, 3.0f);
    //     //Rifleaudio.clip = ARsound;
    //     RifleAudio.Play();


    //     // Debug.DrawRay (Vector3 start(rayを開始する位置), Vector3 dir(rayの方向と長さ), Color color(ラインの色));
    //     Debug.DrawRay(rayStartPosition, rayDirection * distance, Color.red);

    //     // なにかを検出したら
    //     if (isHit){
    //         if (raycastHit.collider.tag == "Player"){
    //             // LogにHitしたオブジェクト名を出力
    //             Debug.Log("EnemyHitObject : " + raycastHit.collider.gameObject.name);
    //             //ダメージ処理をする(プレイヤーのHP演算)
    //             playerScript.playerHPDecrease();
    //         }

    //     }
    // }
}
