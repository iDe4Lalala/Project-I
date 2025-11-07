using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HumanType
{
    Player,
    Enemy
}

[CreateAssetMenu(fileName = "HumanDataBase", menuName = "ScriptableObjects/HumanDataBase")]
public class HumanDataBase : ScriptableObject
{
    [field: SerializeField] public GameObject HumanObject { get; private set; }
    [field: SerializeField] public HumanType HumanType { get; private set; }
    [field: SerializeField] public int HumanHP { get; private set; }
    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField] public float TurningMaxAngle { get; private set; }
    [field: SerializeField] public float TurningMinAngle { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    // [field: SerializeField] public AudioSource FootStepAudio { get; private set; }
}
