using UnityEngine;

public interface IWeaponGenerator: IGenerator
{
    public GameObject GenerateView(Transform parent);
}
