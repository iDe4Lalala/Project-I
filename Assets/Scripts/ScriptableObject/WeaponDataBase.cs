using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataBase", menuName = "ScriptableObjects/WeaponDataBase")]
public class WeaponDataBase : ScriptableObject
{
    [field: SerializeField] public float MaximumBallisticDistance { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    [field: SerializeField] public int MagazineCapacity { get; private set; }
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
    [field: SerializeField] public float RecoilForce { get; private set; }
    [field: SerializeField] public AudioClip ShootingAudioClip { get; private set; }
}
