using UnityEngine;

public interface IGenerator<out T>
{
    public T Generate(GameObject prefab);
}
