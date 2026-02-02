using UnityEngine;

public class WeaponGenerator : IWeaponGenerator<GameObject>
{
    public GameObject Generate(GameObject weaponPrefab, Transform parent)
    {
        return Object.Instantiate(weaponPrefab, parent);
    }
}
