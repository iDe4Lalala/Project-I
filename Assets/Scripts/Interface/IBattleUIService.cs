using System.Collections;

public interface IBattleUIService
{
    public void Initialize(BattleSceneManager battleSceneManager);
    public void UpdateTimer(float time);
    public void OnPlayerKilled();
    public void OnEnemyKilled();
    public IEnumerator OnNextWaveStarted(string text);
    public IEnumerator OnThisWaveCleared(string text);
    public IEnumerator OnGameCleared();
    public IEnumerator OnGameOvered();
    public IEnumerator PlayCountdown(float countdownTime);
}
