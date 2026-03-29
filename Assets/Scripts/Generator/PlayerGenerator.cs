using UnityEngine;

public class PlayerGenerator : IGenerator
{
    private HumanDataBase _playerDataBase;
    private IWeaponGenerator _weaponGenerator;
    private PlayerInputHandler _playerInputHandler;
    private IReloadRuntime _reloadRuntime;

    public PlayerGenerator(
        HumanDataBase playerDataBase, IWeaponGenerator weaponGenerator, PlayerInputHandler playerInputHandler)
    {
        _playerDataBase = playerDataBase;
        _weaponGenerator = weaponGenerator;
        _playerInputHandler = playerInputHandler;
    }

    public GameObject Generate(Transform spawnPoint)
    {
        GameObject player = Object.Instantiate(
            _playerDataBase.HumanObject, spawnPoint.position, spawnPoint.rotation);
        var playerComponents = player.GetComponent<PlayerComponents>();
        GameObject weapon = _weaponGenerator.Generate(playerComponents.WeaponSocket.transform);
        GameObject viewWeapon = _weaponGenerator.GenerateView(playerComponents.ViewRifleSocket.transform);

        InitializePlayerParameter(playerComponents, player, weapon, viewWeapon);
        return player;
    }

    private void InitializePlayerParameter(
        PlayerComponents playerComponents, GameObject player, GameObject weapon, GameObject viewWeapon)
    {
        var playerManager = player.GetComponent<PlayerManager>();
        var rifleManager = weapon.GetComponent<RifleManager>();
        var animator = viewWeapon.GetComponent<Animator>();
        playerComponents.SetRifleManager(rifleManager);
        _reloadRuntime = rifleManager;

        playerManager.Initialize(_playerInputHandler);
        _playerInputHandler.InitializeManager(playerManager);
        _reloadRuntime.InitializeAnimator(animator);
    }
}
