public class BadNpcSpawner : NpcSpawner<BadNpc>
{
    protected override void Awake()
    {
        base.Awake();
        if (GameManager.Instance != null)
        {
            switch (GameManager.Instance.Game_Difficulty)
            {
                case GameManager.GameDifficulty.Easy:
                    SpawnRate = 15f;
                    break;
                case GameManager.GameDifficulty.Medium:
                    SpawnRate = 11f;
                    break;
                case GameManager.GameDifficulty.Hard:
                    SpawnRate = 6f;
                    break;
            }
        }
    }
}
