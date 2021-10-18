using UnityEngine;
using System.Collections;
public abstract class NpcSpawner<T> : MonoBehaviour where T : Npc
{
    [SerializeField] protected SpawnArea spawnArea;
    [Range(100f, 1f)]
    [SerializeField] protected float spawnRate = 2f;
    public float SpawnRate
    {
        get { return spawnRate; }
        set
        {
            if (value > 0 && value < 100)
            {
                spawnRate = value;
                SpawnSeconds = new WaitForSeconds(spawnRate);
            }
        }
    }
    protected IEnumerator spawningCoroutine;
    protected IEnumerator difficultyCoroutine;
    protected WaitForSeconds SpawnSeconds;
    protected WaitForSeconds DifficultySeconds;

    private static float easyMultiplier = 0.95f;
    private static float mediumMultiplier = 0.9f;
    private static float hardMultiplier = 0.85f;
    private static float minSpawnRate = 5f;

    protected virtual void Awake()
    {
        SpawnRate = spawnRate;
        DifficultySeconds = new WaitForSeconds(15f);
    }

    private void Start() => StartSpawning();

    public void StartSpawning()
    {
        spawningCoroutine = SpawningCoroutine();
        difficultyCoroutine = IncreaseDifficultyOverTime();

        StartCoroutine(IncreaseDifficultyOverTime());
        StartCoroutine(SpawningCoroutine());
    }

    private IEnumerator IncreaseDifficultyOverTime()
    {
        if (GameManager.Instance != null)
        {
            while (GameManager.Instance.currentState != GameManager.GameState.MainMenu)
            {
                //wait for x seconds,
                yield return DifficultySeconds;
                //increase spawn rate
                if (SpawnRate > minSpawnRate)
                {
                    switch (GameManager.Instance.Game_Difficulty)
                    {
                        case GameManager.GameDifficulty.Easy:
                            SpawnRate *= easyMultiplier;
                            break;
                        case GameManager.GameDifficulty.Medium:
                            SpawnRate *= mediumMultiplier;
                            break;
                        case GameManager.GameDifficulty.Hard:
                            SpawnRate *= hardMultiplier;
                            break;
                    }
                }
            }
        }
    }
    public void StopSpawning() => StopCoroutine(spawningCoroutine);

    protected IEnumerator SpawningCoroutine()
    {
        while (GameManager.Instance.currentState != GameManager.GameState.MainMenu)
        {
            Npc npc = ObjectPool<T>.Instance.GetRandomObject();
            if (npc != null)
            {
                npc.transform.position = spawnArea.GetRandomPosition();
                npc.gameObject.SetActive(true);
                npc.StartRoaming();
            }
            yield return SpawnSeconds;
        }
    }
}
