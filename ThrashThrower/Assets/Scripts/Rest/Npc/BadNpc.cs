public class BadNpc : Npc
{
    public delegate void BadNpcEvent(float damageTaken);
    public static event BadNpcEvent OnBadNpcDamageTaken;
    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        OnBadNpcDamageTaken?.Invoke(damage);
        if (CurrentHealth <= 0) ObjectPool<BadNpc>.Instance.ReturnObject(this);
    }
}
