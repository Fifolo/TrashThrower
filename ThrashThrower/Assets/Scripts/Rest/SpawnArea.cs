using UnityEngine;
using UnityEngine.AI;

public class SpawnArea : MonoBehaviour
{
    private Bounds bounds;
    private void Awake() => bounds = GetComponent<Collider>().bounds;
    public Vector3 GetRandomPosition()
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(-bounds.extents.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 position = new Vector3(x, y, z);

        int amountCalled = 1;

        while (!NavMesh.SamplePosition(position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            x = Random.Range(bounds.min.x, bounds.max.x);
            y = Random.Range(-bounds.extents.y, bounds.max.y);
            z = Random.Range(bounds.min.z, bounds.max.z);
            position = new Vector3(x, y, z);
            amountCalled += 1;
        }
        position.y += 1;
        return position;
    }
}
