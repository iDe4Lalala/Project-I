using UnityEngine;

public class PlayerGenerator : IGenerator<GameObject>
{
    private HumanDataBase _playerDataBase;

    public PlayerGenerator(HumanDataBase playerDataBase)
    {
        _playerDataBase = playerDataBase;
    }

    public GameObject Generate()
    {
        return Object.Instantiate(_playerDataBase.HumanObject);
    }
}
