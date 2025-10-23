using _Project.Code.Core.General;

public class DifficultyModifier : Singleton<DifficultyModifier>
{
    public float DifficultyModifierAmount { get; private set; }

    public void IncreaseDifficulty()
    {
        DifficultyModifierAmount += 0.5f;
    }

    public void ResetDifficulty()
    {
        DifficultyModifierAmount = 1.0f;
    }
}
