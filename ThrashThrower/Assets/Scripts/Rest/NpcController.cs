using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NpcController : MonoBehaviour, IDamagable
{
    public delegate void NpcAction(NpcData.NPC_Type npc_Type);
    public static event NpcAction OnNpcDeath;

    #region Editor Exposed Fields
    [SerializeField] private NpcData npcData;
    #endregion

    #region Instance Variables
    private NavMeshAgent agent;
    private Transform npcTransform;
    public NpcData.NPC_Type npc_Type { get { return npcData.Npc_Type; } }
    private NPC_State currentState = NPC_State.Idle;
    public enum NPC_State
    {
        Idle,
        Roaming,
        FightingPlayer
    }
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        npcTransform = transform;
        agent = npcTransform.GetComponent<NavMeshAgent>();

        CheckNpcData();
        currentState = NPC_State.Roaming;
        StartCoroutine(Roam());
    }
    private void CheckNpcData()
    {
        if (npcData == null)
        {
            Debug.LogWarning($"Npc: {name}, npc data not attached");
        }
        else
        {
            agent.speed = npcData.runningSpeed;
            npcData.InitializeNpc(gameObject);
        }
    }
    private IEnumerator Roam()
    {
        //Debug.Log("Entering roaming state");
        while (currentState == NPC_State.Roaming)
        {
            //get valid destination
            //Debug.Log("Searching for valid destination...");
            Vector3 nextDestination = GetRandomPosition();
            while (!CanReachPositionWithRaycast(nextDestination))
            {
                yield return new WaitForSeconds(NpcData.CAN_REACH_POSITION_CD);
                nextDestination = GetRandomPosition();
            }
            //Debug.Log($"Valid destination found {nextDestination}");
            //set agent destination
            agent.SetDestination(nextDestination);

            //wait until agent reaches the target

            while (!AgentReachedDestination(nextDestination))
            {
                yield return new WaitForSeconds(NpcData.DISTANCE_CHECK_CD);
            }

            //wait for some time before going to new path
            yield return new WaitForSeconds(NpcData.NEW_PATH_CD);
            //Debug.Log($"{timeBetweenNewPath} seconds passed, returning to main loop");
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion

    #region Methods
    private int timesRun = 0;
    private float previousDistance = 0;
    private bool AgentReachedDestination(Vector3 destination)
    {
        //disable y (height) calculations
        Vector2 agentPosition = new Vector2(npcTransform.position.x, npcTransform.position.z);
        Vector2 dest = new Vector2(destination.x, destination.z);


        float newDistance = Vector2.Distance(agentPosition, dest);

        //UGLYYY but prevents from getting stuck - can reach position not always passes right value
        if (timesRun == 0)
        {
            previousDistance = newDistance;
            timesRun++;
        }
        else
        {
            if (newDistance == previousDistance)
            {
                timesRun += 1;
                if (timesRun > 4) return true;
            }
            else
            {
                timesRun = 0;
            }
        }

        return previousDistance < agent.stoppingDistance;
    }
    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(
            npcTransform.position.x + Random.Range(-npcData.roamingRange, npcData.roamingRange),
            0,
            npcTransform.position.z + Random.Range(-npcData.roamingRange, npcData.roamingRange));
        return randomPosition;
    }
    //less expensive but for some reason it sometimes allows wrong position to pass
    private bool CanReachPositionWithRaycast(Vector3 position)
    {
        if (NavMesh.Raycast(agent.transform.position, position, out NavMeshHit hit, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }
    //more expensive but accurate
    private bool CanReachPositionWithCalculatePath(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(position, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete) return true;
        }
        return false;
    }

    public void TakeDamage(float damage)
    {
        OnNpcDeath?.Invoke(npc_Type);
        Destroy(gameObject);
    }
    #endregion
}
