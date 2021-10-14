using UnityEngine;

[CreateAssetMenu(fileName = "BadNpcData", menuName = "NpcData/BadNpcData")]
public class BadNpcData : NpcData
{
    [HideInInspector]
    public bool dropsTrash;
    [HideInInspector]
    public float dropRate = 2f;
    [HideInInspector]
    public float initialWaitTime = 2f;
    [HideInInspector]
    public float dropChance = 2f;

    public override void InitializeNpc(GameObject npc)
    {
        npcType = NpcType.Bad;

        if (dropsTrash)
        {
            TrashDropping trashDropping = npc.AddComponent<TrashDropping>();
            trashDropping.InitializeData(this);
        }
    }
}
