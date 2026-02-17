using UnityEngine;
using System.Collections;
using System;

public class WaveState : IBattleState
{
    public event Action<BattleStateType> OnChangingState;
    private IGenerator _enemyGenerator;
    private ISpawnPointProvider _enemySpawnPointProvider;
    private IGenerator _weaponGenerator;
    private IBattleUIService _battleUIService;

    public WaveState(IGenerator enemyGenerator, ISpawnPointProvider enemySpawnPointProvider, 
        IGenerator weaponGenerator, IBattleUIService battleUIManager)
    {
        _enemyGenerator = enemyGenerator;
        _enemySpawnPointProvider = enemySpawnPointProvider;
        _weaponGenerator = weaponGenerator;
        _battleUIService = battleUIManager;
    }

    public IEnumerator Enter()
    {
        // 敵、武器の生成
        Transform enemySpawnPoint = _enemySpawnPointProvider.GetSpawnPoint();
        GameObject enemy = _enemyGenerator.Generate(enemySpawnPoint);

        // "Wave開始"表示
        yield return _battleUIService.ShowProgressTextForSeconds("Wave Start!", 2f);
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
        OnChangingState?.Invoke(BattleStateType.Clear);
    }
}
