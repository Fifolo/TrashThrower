using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Npc : MonoBehaviour, IDamagable
{
    public delegate void NpcAction(NpcData npcData);
    public static event NpcAction OnNpcDeath;

    #region Timers
    private WaitForSeconds NewPathSeconds;
    private WaitForSeconds CanReachPositionSeconds;
    private WaitForSeconds DistanceCheckSeconds;
    private WaitForSeconds FollowSeconds;
    #endregion

    #region Editor Exposed Fields
    [SerializeField] protected NpcData npcData;
    #endregion

    #region Instance Variables
    private float currentHealth = 100f;
    public float CurrentHealth
    {
        get { return currentHealth; }
        protected set
        {
            currentHealth = value;

            if (currentHealth <= 0) OnNpcDeath?.Invoke(npcData);
        }
    }

    protected Animator npcAnimator;
    protected NavMeshAgent agent;
    protected Transform npcTransform;
    protected IEnumerator roamCoroutine;
    private IEnumerator rotationCoroutine;
    protected IEnumerator followCoroutine;

    protected static string Velocity = "Velocity";
    protected static string holdsRifle = "holdsRifle";
    protected static string holdsPistol = "holdsPistol";
    public NpcData.NpcType NpcType { get { return npcData.NPCType; } }
    protected NPC_State currentState = NPC_State.Idle;
    public NPC_State CurrentState { get { return currentState; } }
    public enum NPC_State
    {
        Idle,
        Roaming,
        FollowingTarget
    }
    #endregion

    #region MonoBehaviour
    protected virtual void Awake()
    {
        GettingReferences();
        CheckNpcData();
    }

    protected virtual void Update()
    {
        npcAnimator.SetFloat(Velocity, agent.velocity.magnitude/agent.speed);
    }

    protected virtual void OnEnable()
    {
        currentHealth = npcData.health;
        StartRoaming();
    }
    protected virtual void OnDisable()
    {
        currentState = NPC_State.Idle;
        StopAllCoroutines();
    }
    #endregion

    #region Methods

    #region Ugly Method Variables
    private int timesRun = 0;
    private float previousDistance = 0;
    private float newDistance = 0;
    private Vector2 agentPosition;
    private Vector2 dest;
    #endregion
    protected virtual bool AgentReachedDestination(Vector3 destination)
    {
        //disable y (height) calculations
        agentPosition.x = npcTransform.position.x;
        agentPosition.y = npcTransform.position.z;

        dest.x = destination.x;
        dest.y = destination.z;

        newDistance = (dest - agentPosition).sqrMagnitude;

        //UGLYYY but prevents from getting stuck - can reach position not always passes right value
        if (timesRun == 0)
        {
            previousDistance = newDistance;
            timesRun++;
        }
        else
        {
            if (newDistance < previousDistance + 0.001f || newDistance > previousDistance - 0.001f)
            {
                timesRun += 1;
                if (timesRun > 4)
                {
                    timesRun = 0;
                    return true;
                }
            }
            else
            {
                timesRun = 0;
            }
        }

        return newDistance < agent.stoppingDistance;
    }
    private void GettingReferences()
    {
        npcTransform = transform;
        npcAnimator = GetComponentInChildren<Animator>();
        roamCoroutine = RoamCoroutine();
        followCoroutine = FollowTarget(null, 0f);
        rotationCoroutine = UpdateRotation(null, 0f);
        agent = npcTransform.GetComponent<NavMeshAgent>();
    }

    protected virtual void CheckNpcData()
    {
        if (npcData != null)
        {
            agent.speed = npcData.runningSpeed;
            currentHealth = npcData.health;
            npcData.InitializeNpc(gameObject);
            ManageTimers();
        }
        else Debug.LogWarning($"Npc: {name}, npc data not attached");
    }
    private void ManageTimers()
    {
        NewPathSeconds = new WaitForSeconds(npcData.NEW_PATH_CD);
        CanReachPositionSeconds = new WaitForSeconds(npcData.CAN_REACH_POSITION_CD);
        DistanceCheckSeconds = new WaitForSeconds(npcData.DISTANCE_CHECK_CD);
        FollowSeconds = new WaitForSeconds(npcData.FOLLOW_CD);
    }
    protected virtual IEnumerator RoamCoroutine()
    {
        agent.stoppingDistance = 1f;
        while (currentState == NPC_State.Roaming)
        {
            //get valid destination
            Vector3 nextDestination = GetRandomPosition();
            while (!CanReachPositionWithRaycast(nextDestination))
            {
                yield return CanReachPositionSeconds;
                nextDestination = GetRandomPosition();
            }
            //set agent destination
            agent.SetDestination(nextDestination);

            //wait until agent reaches the target

            while (!AgentReachedDestination(nextDestination))
            {
                yield return DistanceCheckSeconds;
            }

            //wait for some time before going to new path'
            yield return NewPathSeconds;
        }
    }
    protected virtual IEnumerator FollowTarget(Transform target, float range)
    {
        agent.stoppingDistance = range;
        Vector3 lastTargetPosition = target.position;

        StartCoroutine(UpdateRotation(target, 0.7f));

        agent.SetDestination(target.position);

        while (currentState == NPC_State.FollowingTarget)
        {
            if (target.gameObject.activeInHierarchy)
            {
                if (lastTargetPosition != target.position)
                {
                    agent.SetDestination(target.position);
                    lastTargetPosition = target.position;
                }

                RotateTowardsTarget(target);

                yield return FollowSeconds;
            }
            else
            {
                StopFollowingTarget();
                StartRoaming();
            }
        }
    }

    private void RotateTowardsTarget(Transform target)
    {
        StopCoroutine(rotationCoroutine);
        rotationCoroutine = UpdateRotation(target, 0.7f);
        StartCoroutine(UpdateRotation(target, 0.7f));
    }

    private IEnumerator UpdateRotation(Transform target, float time)
    {
        float currentLerpTime = 0;
        Vector3 lookDirection = (target.position - npcTransform.position);
        lookDirection.y = 0;
        Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
        while (npcTransform.rotation != desiredRotation)
        {
            npcTransform.rotation = Quaternion.Lerp(npcTransform.rotation, desiredRotation, currentLerpTime / time);
            currentLerpTime += Time.deltaTime;
            yield return null;
        }
    }
    protected virtual Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(
            npcTransform.position.x + Random.Range(-npcData.roamingRange, npcData.roamingRange),
            0,
            npcTransform.position.z + Random.Range(-npcData.roamingRange, npcData.roamingRange));
        return randomPosition;
    }
    //less expensive but for some reason it sometimes allows wrong position to pass
    protected virtual bool CanReachPositionWithRaycast(Vector3 position)
    {
        if (NavMesh.Raycast(agent.transform.position, position, out NavMeshHit hit, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }
    //more expensive but accurate
    protected virtual bool CanReachPositionWithCalculatePath(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(position, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete) return true;
        }
        return false;
    }
    public void StartFollowingTarget(Transform target, float range)
    {
        agent.updateRotation = false;
        StopCoroutine(roamCoroutine);
        currentState = NPC_State.FollowingTarget;
        StartCoroutine(FollowTarget(target, range));
    }
    public void StopFollowingTarget()
    {
        StopCoroutine(followCoroutine);
        agent.updateRotation = true;
        currentState = NPC_State.Idle;
    }
    public void StartRoaming()
    {
        currentState = NPC_State.Roaming;
        roamCoroutine = RoamCoroutine();
        StartCoroutine(RoamCoroutine());
    }
    public void StopRoaming()
    {
        StopCoroutine(roamCoroutine);
        currentState = NPC_State.Idle;
    }
    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
    }

    #endregion
}
