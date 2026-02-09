using UnityEngine;

public interface IWeaponGenerator<out T>
{
    public T Generate(Transform parent);
}
