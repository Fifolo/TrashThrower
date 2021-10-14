using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    private Bounds bounds;
    private void Awake() => bounds = GetComponent<Collider>().bounds;
    public Vector3 GetRandomPosition()
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(-bounds.extents.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x,y,z);
    }
}
