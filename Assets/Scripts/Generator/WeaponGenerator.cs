using UnityEngine;

public class WeaponGenerator : IWeaponGenerator
{
    private WeaponDataBase _weaponDataBase;

    public WeaponGenerator(WeaponDataBase weaponDataBase)
    {
        _weaponDataBase = weaponDataBase;
    }

    public GameObject Generate(Transform parent)
    {
        return Object.Instantiate(_weaponDataBase.WeaponObject, parent);
    }

    public GameObject GenerateView(Transform  parent)
    {
       return Object.Instantiate(_weaponDataBase.ViewWeaponObject, parent);
    }
}
