using UnityEngine;

public interface IFireRuntime
{
    public Vector2 TryFire(float deltaTime, Vector3 position, Vector3 direction);
}
