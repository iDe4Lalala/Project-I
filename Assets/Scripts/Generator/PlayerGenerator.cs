using UnityEngine;

public class PlayerGenerator : IGenerator<GameObject>
{
    public GameObject Generate(GameObject playerPrefab)
    {
        return Object.Instantiate(playerPrefab);
    }
}
