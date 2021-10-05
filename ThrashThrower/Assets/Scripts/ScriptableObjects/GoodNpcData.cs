using UnityEngine;

[CreateAssetMenu(fileName = "GoodNpcData", menuName = "NpcData/GoodNpcData")]
public class GoodNpcData : NpcData
{
    private void Awake()
    {
        npc_Type = NPC_Type.Good;
    }
}
