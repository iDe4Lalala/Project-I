using TMPro;
using UnityEngine;

public class PlayerGenerator : IGenerator
{
    private HumanDataBase _playerDataBase;
    private IGenerator _weaponGenerator;
    private PlayerInputHandler _playerInputHandler;

    public PlayerGenerator(HumanDataBase playerDataBase, IGenerator weaponGenerator, PlayerInputHandler playerInputHandler)
    {
        _playerDataBase = playerDataBase;
        _weaponGenerator = weaponGenerator;
        _playerInputHandler = playerInputHandler;
    }

    public GameObject Generate(Transform spawnPoint)
    {
        GameObject player = Object.Instantiate(_playerDataBase.HumanObject, spawnPoint);
        var playerComponents = player.GetComponent<PlayerComponents>();
        GameObject weapon = _weaponGenerator.Generate(playerComponents.WeaponSocket.transform);

        InitializePlayerParameter(player, weapon);
        return player;
    }

    private void InitializePlayerParameter(GameObject player, GameObject weapon)
    {
        var playerManager = player.GetComponent<PlayerManager>();
        var rifleManager = weapon.GetComponent<RifleManager>();

        playerManager.Initialize(_playerInputHandler, rifleManager);
        _playerInputHandler.InitializeManager(playerManager);
    }
}
