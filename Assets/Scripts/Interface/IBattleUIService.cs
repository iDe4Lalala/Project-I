using System.Collections;

public interface IBattleUIService
{
    public void UpdateTimer(float time);
    public IEnumerator ShowKillTextForSeconds(string text, float seconds);
    public IEnumerator ShowProgressTextForSeconds(string text, float seconds);
    public IEnumerator PlayCountdown(float countdownTime);
}
