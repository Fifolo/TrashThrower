public class BadNpc : Npc
{
    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) ObjectPool<BadNpc>.Instance.ReturnObject(this);
    }
}
