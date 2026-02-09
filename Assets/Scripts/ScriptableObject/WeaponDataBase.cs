using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataBase", menuName = "ScriptableObjects/WeaponDataBase")]
public class WeaponDataBase : ScriptableObject
{
    [field: SerializeField] public GameObject WeaponObject { get; private set; }
    [field: SerializeField] public float MaximumBallisticDistance { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public int MagazineCapacity { get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
    [field: SerializeField] public float RecoilMinX { get; private set; }
    [field: SerializeField] public float RecoilMaxX { get; private set; }
    [field: SerializeField] public float RecoilMinY { get; private set; }
    [field: SerializeField] public float RecoilMaxY { get; private set; }
    [field: SerializeField] public AudioClip ShootingAudioClip { get; private set; }
}
