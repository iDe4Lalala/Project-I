using System.Collections;

public interface IPlayerUIService
{
    public void SetVisible(bool isVisible);
    public void Initialize(PlayerComponents playerComponents);
    public void DisplayOrHideCursor(bool isDisplay);
    public void ShowPlayerHP(int currentHP);
    public void ShowLeftAmmoCount(int currentAmmoCount);
    public void ShowPlayerLifePoint(int currentLifePoint);
    public IEnumerator ShowHitCrossHairForSeconds(float seconds);
}
