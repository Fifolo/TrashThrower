using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private float damage = 10f;
    public float Damage
    {
        get { return damage; }
        set
        {
            if (value > 0) damage = value;
        }
    }
    private Rigidbody bulletRb;
    private Transform bulletTransform;
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = false;
        bulletRb = GetComponent<Rigidbody>();
        bulletRb.useGravity = false;
        bulletRb.freezeRotation = true;
        bulletTransform = transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void SetBulletMovement(Vector3 direction, float speed)
    {
        bulletTransform.up = direction;
        bulletRb.velocity = direction * speed;
    }
}
