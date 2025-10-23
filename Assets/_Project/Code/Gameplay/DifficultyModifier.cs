using _Project.Code.Core.General;
using UnityEngine;

public class DifficultyModifier : Singleton<DifficultyModifier>
{
    public float DifficultyModifierAmount { get; private set; }
    [SerializeField] private float incrementAmount = 0.75f;

    public void IncreaseDifficulty()
    {
        DifficultyModifierAmount += incrementAmount;
    }

    public void ResetDifficulty()
    {
        DifficultyModifierAmount = 1.0f;
    }
}
