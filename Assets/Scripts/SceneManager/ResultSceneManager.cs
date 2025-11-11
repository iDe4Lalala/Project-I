using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultSceneManager : MonoBehaviour
{
    /*public GameObject player;
    public GameObject enemy;
    PlayerScript playerscript;
    EnemyScript enemyscript;*/
    [SerializeField] private string _nextSceneName;     // 次のシーンの名前
    public Text playerKillCountText;
    public Text enemyKillCountText;
    public GameObject WinUI;
    public GameObject LossUI;
    public GameObject DrawUI;

    void Start(){
        // playerscript = player.GetComponent<PlayerScript>();
        // enemyscript = enemy.GetComponent<EnemyScript>();
        // playerKillCountText.text = enemyscript.playerKillCount.ToString();
        // enemyKillCountText.text = playerscript.enemyKillCount.ToString();
        // playerKillCountText.text = Enemy.playerKillCount.ToString();
        // enemyKillCountText.text = Player.enemyKillCount.ToString();
        // WinUI.SetActive(false);
        // LossUI.SetActive(false);
        // DrawUI.SetActive(false);


        // if (Enemy.playerKillCount > Player.enemyKillCount){
        //     WinProcess();
        // }
        // else if(Enemy.playerKillCount < Player.enemyKillCount){
        //     LossProcess();
        // }
        // else{
        //     DrawProcess();
        // }
    }

    void Update(){

    }

    // void WinProcess(){
    //     WinUI.SetActive(true);
    // }

    // void LossProcess(){
    //     LossUI.SetActive(true);
    // }

    // void DrawProcess(){
    //     DrawUI.SetActive(true);
    // }

    public void LoadOtherScene()
    {
        /// <summary>
        /// スタートシーンへ移動する
        /// </summary>
        
        SceneManager.LoadScene(_nextSceneName);
    }
}
