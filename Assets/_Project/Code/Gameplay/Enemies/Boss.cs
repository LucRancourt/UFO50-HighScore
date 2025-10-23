using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.GameManagement;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private EnemyManager waveManager;
    [SerializeField] private BossArm[] bossArms;
    [SerializeField] private int score;
    private EBossColor _requiredColor;
    private EColor[] _armColorsNeeded = new EColor[2];
    private bool _isDying;
    private SpriteRenderer _spriteRenderer;

    public void Reset()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        _requiredColor = (EBossColor)Random.Range(0, 3);
        _spriteRenderer.color = ServiceLocator.Get<GameManagementService>().EBossColorToColor(_requiredColor);

        switch (_requiredColor)
        {
            case EBossColor.Green:
                _armColorsNeeded[0] = EColor.Blue;
                _armColorsNeeded[1] = EColor.Yellow;
                break;

            case EBossColor.Purple:
                _armColorsNeeded[0] = EColor.Blue;
                _armColorsNeeded[1] = EColor.Red;
                break;

            case EBossColor.Orange:
                _armColorsNeeded[0] = EColor.Red;
                _armColorsNeeded[1] = EColor.Yellow;
                break;
        }

        int index = Random.Range(0, 1);
        bossArms[0].SetRequiredColor(_armColorsNeeded[index]);
        bossArms[1].SetRequiredColor(_armColorsNeeded[index - 1 < 0 ? 1 : 0]);

        bossArms[0].Reset();
        bossArms[1].Reset();

        _isDying = false;
    }

    private void Update()
    {
        if (bossArms[0].IsComplete() && bossArms[1].IsComplete() && !_isDying)
        {
            _isDying = true;
            Invoke("Die", 0.5f);
        }
    }

    private void Die()
    {
        ScoreManager.Instance.AddScore(score);
        waveManager.BossDefeated();
    }
}
