using UnityEngine;

public interface IWeaponGenerator<out T>
{
    public T Generate(GameObject weaponPrefab, Transform parent);
}
