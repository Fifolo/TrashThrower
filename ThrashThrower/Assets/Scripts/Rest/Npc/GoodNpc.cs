public class GoodNpc : Npc
{
    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0) ObjectPool<GoodNpc>.Instance.ReturnObject(this);
    }
}
