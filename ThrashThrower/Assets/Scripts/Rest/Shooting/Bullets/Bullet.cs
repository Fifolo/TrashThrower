using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Bullet : MonoBehaviour
{
    [SerializeField] private AudioClip collisionClip;
    private IEnumerator lifeTimeCoroutine;
    private float damage = 10f;
    private AudioSource audioSource;
    public static WaitForSeconds BulletLifeTime = new WaitForSeconds(5f);
    private void OnEnable()
    {
        lifeTimeCoroutine = LifeTimeCoroutine();
        StartCoroutine(lifeTimeCoroutine);
    }
    private void OnDisable() => StopCoroutine(lifeTimeCoroutine);
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
    private IEnumerator LifeTimeCoroutine()
    {
        yield return BulletLifeTime;
        if (ObjectPool<Bullet>.Instance != null) ObjectPool<Bullet>.Instance.ReturnObject(this);
        else Destroy(gameObject);
    }
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = false;
        bulletRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        bulletRb.useGravity = false;
        bulletRb.freezeRotation = true;
        bulletTransform = transform;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out IDamagable damagable)) damagable.TakeDamage(damage);

        //if (collisionClip != null) audioSource.PlayOneShot(collisionClip);

        ReturnToPool();
    }

    protected virtual void ReturnToPool()
    {
        if (ObjectPool<Bullet>.Instance != null) ObjectPool<Bullet>.Instance.ReturnObject(this);

        else
        {
            Debug.Log("ObjectPool<Bullet>.Instance is null");
            Destroy(gameObject);
        }
    }

    public void SetBulletMovement(Vector3 direction, float speed)
    {
        bulletTransform.up = direction;
        bulletRb.velocity = direction * speed;
    }
}
