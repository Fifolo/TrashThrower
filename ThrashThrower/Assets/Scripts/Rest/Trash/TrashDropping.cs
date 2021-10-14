using UnityEngine;
using System.Collections;

public class TrashDropping : MonoBehaviour
{
    private static float DEFAULT_DROP_RATE = 5f;
    private static float DEFAULT_INITIAL_WAIT = 2f;
    private static float DEFAULT_DROP_CHANCE = 0.7f;

    private BadNpcData badNpcData;
    private Transform dropperTransform;
    private Vector3 dropPosition;

    public static WaitForSeconds ShortDelay = new WaitForSeconds(0.1f);
    private void Awake()
    {
        dropperTransform = transform;
    }
    public void InitializeData(BadNpcData badNpcData) => this.badNpcData = badNpcData;
    public void StartDroppingTrash()
    {
        if (badNpcData != null) StartCoroutine(DroppingCoroutine());

        else StartCoroutine(DefaultDroppingCoroutine());
    }
    private void OnEnable()
    {
        StartCoroutine(StartDroppingTrashAfterDelay());
    }
    private IEnumerator StartDroppingTrashAfterDelay()
    {
        yield return ShortDelay;
        StartDroppingTrash();
    }
    private float range = 180f;
    private void TryDropTrash(float dropChance)
    {
        if (Random.Range(0f, 1f) < DEFAULT_DROP_CHANCE)
        {
            Trash trashToDrop = TrashPool.Instance.GetRandomObject();
            if (trashToDrop != null)
            {
                dropPosition = dropperTransform.position;
                dropPosition.y = dropPosition.y + 3f;
                trashToDrop.transform.position = dropPosition;
                trashToDrop.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range)), ForceMode.Impulse);
                trashToDrop.gameObject.SetActive(true);
            }
        }
    }
    private IEnumerator DroppingCoroutine()
    {
        yield return new WaitForSeconds(badNpcData.initialWaitTime);

        while (true)
        {
            TryDropTrash(badNpcData.dropChance);
            yield return new WaitForSeconds(badNpcData.dropRate);
        }
    }
    private IEnumerator DefaultDroppingCoroutine()
    {
        yield return new WaitForSeconds(DEFAULT_INITIAL_WAIT);

        while (true)
        {
            TryDropTrash(DEFAULT_DROP_CHANCE);
            yield return new WaitForSeconds(DEFAULT_DROP_RATE);
        }
    }
    private void OnDisable() => StopAllCoroutines();
}
