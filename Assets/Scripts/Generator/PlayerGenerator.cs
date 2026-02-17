using UnityEngine;

public class PlayerGenerator : IGenerator
{
    private HumanDataBase _playerDataBase;
    private IGenerator _weaponGenerator;

    public PlayerGenerator(HumanDataBase playerDataBase, IGenerator weaponGenerator)
    {
        _playerDataBase = playerDataBase;
        _weaponGenerator = weaponGenerator;
    }

    public GameObject Generate(Transform spawnPoint)
    {
        GameObject player = Object.Instantiate(_playerDataBase.HumanObject, spawnPoint);
        var playerComponents = player.GetComponent<PlayerComponents>();
        _weaponGenerator.Generate(playerComponents.WeaponSocket.transform);
        return player;
    }
}
