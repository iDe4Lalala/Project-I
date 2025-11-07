using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerCurrentHP;
    [SerializeField] private TMP_Text _playerMaxHP;
    [SerializeField] private GameObject _HitCrossHair;

    private void OnEnable()
    {
        _HitCrossHair.SetActive(false);
    }

    public void SetPlayerHP(int currentHP)
    {
        _playerMaxHP.text = currentHP.ToString();
        _playerCurrentHP.text = currentHP.ToString();
    }

    private void DecreasePlayerHP(int currentHP)
    {
        _playerCurrentHP.text = currentHP.ToString();
    }
}
