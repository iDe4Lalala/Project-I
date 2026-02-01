using System.Collections;

public interface IPlayerUIService
{
    public void ShowPlayerHP(float currentHP);
    public void ShowLeftAmmoCount(int currentAmmoCount);
    public IEnumerator ShowHitCrossHairForSeconds(float seconds);
}
