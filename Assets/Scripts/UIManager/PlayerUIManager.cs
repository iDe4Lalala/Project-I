using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerCurrentHP;
    [SerializeField] private TMP_Text _playerMaxHP;
    [SerializeField] private GameObject _HitCrossHair;

    private PlayerComponents _playerComponents;

    private void OnEnable()
    {
        _HitCrossHair.SetActive(false);
    }

    public void SetPlayerHP(PlayerComponents playerComponents)
    {
        /// <summary>
        /// プレイヤーのHPを設定する
        /// </summary>
        
        _playerComponents = playerComponents;
        _playerMaxHP.text = _playerComponents.HumanDataBase.HumanHP.ToString();
        _playerCurrentHP.text = _playerComponents.HumanDataBase.HumanHP.ToString();

        _playerComponents.PlayerManager.OnDamaged += DecreasePlayerHP;
    }

    private void DecreasePlayerHP()
    {
        /// <summary>
        /// プレイヤーのHPを減少させる
        /// </summary>
        
        _playerCurrentHP.text = _playerComponents.PlayerManager.PlayerHP.ToString();

        if (_playerComponents.PlayerManager.PlayerHP <= 0)
        {
             _playerComponents.PlayerManager.OnDamaged -= DecreasePlayerHP;
        }
    }
}
