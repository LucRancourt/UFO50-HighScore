using UnityEngine;

namespace _Project.Code.Gameplay.PlayerController._Profile
{
    public abstract class MovementProfile : ScriptableObject
    {
        [field: SerializeField, Header("Speed")] public float WalkSpeed { get; private set; } = 3f;
    }
}
