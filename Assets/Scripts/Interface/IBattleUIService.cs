using System.Collections;
using System;

public interface IBattleUIService
{
    public event Action OnStartingBattle;

    public void UpdateTimer(float time);
    public IEnumerator ShowKillTextForSeconds(string text, float seconds);
    public IEnumerator ShowProgressTextForSeconds(string text, float seconds);
    public IEnumerator PlayCountdown(float countdownTime);
}
