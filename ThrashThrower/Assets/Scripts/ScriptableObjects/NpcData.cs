using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NpcData : ScriptableObject
{
    public float roamingRange = 10f;
    public float walkingSpeed = 5f;
    public float runningSpeed = 8f;
    [Tooltip("Amount of time that npc waits after reaching destination, before setting new path")]
    public float newPathCd = 1f;

    //Amount of time that npc controller waits, before trying to look for new valid path
    public static readonly float CAN_REACH_POSITION_CD = 0.2f;
    public static readonly float DISTANCE_CHECK_CD = 0.4f;


    protected NPC_Type npc_Type;
    public NPC_Type Npc_Type { get { return npc_Type; } }
    public enum NPC_Type
    {
        Good,
        Bad
    }
}
