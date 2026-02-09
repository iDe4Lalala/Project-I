using UnityEngine;
using System;

public class WaveState : IBattleState
{
    public event Action<int> OnChangingState;
    private EnemyGenerator _enemyGenerator;
    private WeaponGenerator _weaponGenerator;
    private IBattleUIService _battleUIService;

    public WaveState(EnemyGenerator enemyGenerator, WeaponGenerator weaponGenerator, IBattleUIService battleUIManager)
    {
        _enemyGenerator = enemyGenerator;
        _weaponGenerator = weaponGenerator;
        _battleUIService = battleUIManager;
    }

    public void Enter()
    {
        // 敵、武器の生成
        GameObject enemy = _enemyGenerator.Generate();
        EnemyComponents enemyComponents = enemy.GetComponent<EnemyComponents>();
        GameObject rifle = _weaponGenerator.Generate(enemyComponents.RifleSocket.transform);
        // enemyComponents.SetRifleManager(rifle);

        // "Wave開始"表示
        _battleUIService.ShowProgressTextForSeconds("Wave Start!", 2f);
    }

    public void Execute()
    {
        // 終了条件管理
        // 敵、プレイヤー死亡の監視
        // 制限時間の監視
    }

    public void Exit()
    {
        // WaveClearStateまたはBattleEndStateへ遷移
        OnChangingState?.Invoke(2);
    }
}
