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
    protected WaitForSeconds SpawnSeconds;

    protected virtual void Awake() => SpawnRate = spawnRate;

    private void Start() => StartSpawning();

    public void StartSpawning()
    {
        spawningCoroutine = SpawningCoroutine();
        StartCoroutine(SpawningCoroutine());
    }

    public void StopSpawning() => StopCoroutine(spawningCoroutine);

    protected IEnumerator SpawningCoroutine()
    {
        //GameManager.Instance.currentState != GameManager.GameState.MainMenu
        while (true)
        {
            Npc npc = ObjectPool<T>.Instance.GetRandomObject();
            //GoodNpcPool.Instance.GetObject();
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
