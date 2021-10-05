using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadNpcData", menuName = "NpcData/BadNpcData")]
public class BadNpcData : NpcData
{

    private void Awake()
    {
        npc_Type = NPC_Type.Bad;
    }
}
