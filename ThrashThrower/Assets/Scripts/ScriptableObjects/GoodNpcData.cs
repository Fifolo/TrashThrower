using UnityEngine;

[CreateAssetMenu(fileName = "GoodNpcData", menuName = "NpcData/GoodNpcData")]
public class GoodNpcData : NpcData
{
    public override void InitializeNpc(GameObject npc)
    {
        npc_Type = NPC_Type.Good;
    }
}
