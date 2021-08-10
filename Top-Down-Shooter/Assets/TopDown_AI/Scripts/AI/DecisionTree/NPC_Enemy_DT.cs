using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Enemy_DT : MonoBehaviour
{
	public enum NPC_EnemyAction { NONE = 0, IDLE, PATROL, INSPECT, ATTACK }
	public enum NPC_WeaponType { KNIFE = 0, RIFLE, SHOTGUN }

	public UnityEngine.AI.NavMeshAgent navMeshAgent;
	public Animator npcAnimator;

	public GameObject proyectilePrefab;

	public LayerMask hitTestLayer;

	Vector3 targetPos, startingPos;
	int hashSpeed;

	public NPC_WeaponType weaponType = NPC_WeaponType.KNIFE;
	public Transform weaponPivot;
	float weaponRange;
	float weaponActionTime, weaponTime;

	public NPC_PatrolNode patrolNode;

	DT_Node dtNode_Root;
	DT_Action dt_LastAction;
	public NPC_EnemyAction currentAction;

	bool inAttackRange = false;

	bool canHearPlayer = false;
	bool useHardcodedDT = false;  //TODO: YOUR CODE HERE (Q2) - Simply change this to true!

	DT_Action idle = null, inspect = null, attack = null;

	// Start is called before the first frame update
	void Start()
    {
		startingPos = transform.position;
		hashSpeed = Animator.StringToHash("Speed");
		SetWeapon(weaponType);
		GameManager.AddToEnemyCount();
		BuildDecisionTree();
	}

    // Update is called once per frame
    void Update()
    {
		npcAnimator.SetFloat(hashSpeed, navMeshAgent.velocity.magnitude);

		DT_Action dt_NewAction = null;
		if (!useHardcodedDT)
		{
			dt_NewAction = dtNode_Root.MakeDecision() as DT_Action;
		}
		else
        {
            //TODO: YOUR CODE HERE (Q2)
		}
		if (dt_NewAction != null)
		{
			if (dt_NewAction != dt_LastAction)
			{
				if (dt_LastAction != null)
				{
					dt_LastAction.end();
				}
				dt_NewAction.init();
			}
			else
			{
				dt_NewAction.update();
			}
		}
		dt_LastAction = dt_NewAction;

		//canHearPlayer = false;
	}

	void BuildDecisionTree()
	{
		idle = new DT_Action(ActionInit_Idle, ActionUpdate_Idle, ActionEnd_Idle);
		inspect = new DT_Action(ActionInit_Inspect, ActionUpdate_Inspect, ActionEnd_Inspect);
		attack = new DT_Action(ActionInit_Attack, ActionUpdate_Attack, ActionEnd_Attack);

		//TODO: YOUR CODE HERE (Q1)
		DT_Decision dtNode_InAttackRange = new DT_Decision(IsInAttackRange, attack, inspect);
		dtNode_Root = new DT_Decision(CanSeePlayer, dtNode_InAttackRange, idle);

		dt_LastAction = null;
	}

	bool CanSeePlayer()
    {
		RaycastHit hit = new RaycastHit();
		int nRays = 100;
		for (int i = 0; i < nRays; i++)
		{
			float theta = (float)i / nRays * 2 * Mathf.PI;
			if (Physics.Raycast(transform.position, new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta)), out hit, 50.0f, hitTestLayer) && hit.collider.tag == "Player")
			{
				SetTargetPos(hit.point);
				return true;
			}
		}
		return false;
	}

	bool CanHearPlayer()
    {
		return canHearPlayer;
	}

	bool IsInAttackRange()
    {
		return inAttackRange;
    }

	public void SetAction(NPC_EnemyAction newAction)
	{
		currentAction = newAction;
	}

	void SetWeapon(NPC_WeaponType newWeapon)
	{
		npcAnimator.SetTrigger("WeaponChange");
		npcAnimator.SetInteger("WeaponType", (int)weaponType);
		switch (weaponType)
		{
			case NPC_WeaponType.KNIFE:
				weaponRange = 1.0f;
				weaponActionTime = 0.2f;
				weaponTime = 0.4f;
				break;
			case NPC_WeaponType.RIFLE:
				weaponRange = 20.0f;
				weaponActionTime = 0.025f;
				weaponTime = 0.05f;
				break;
			case NPC_WeaponType.SHOTGUN:
				weaponRange = 20.0f;
				weaponActionTime = 0.35f;
				weaponTime = 0.75f;
				break;
		}
	}

	void AttackAction()
	{
		switch (weaponType)
		{
			case NPC_WeaponType.KNIFE:
				RaycastHit[] hits = Physics.SphereCastAll(weaponPivot.position, 2.0f, weaponPivot.forward);
				foreach (RaycastHit hit in hits)
				{
					if (hit.collider != null && hit.collider.tag == "Player")
					{
						hit.collider.GetComponent<PlayerBehavior>().DamagePlayer();
					}
				}
				break;
			case NPC_WeaponType.RIFLE:
				GameObject bullet = GameObject.Instantiate(proyectilePrefab, weaponPivot.position, weaponPivot.rotation) as GameObject;
				bullet.transform.Rotate(0, Random.Range(-7.5f, 7.5f), 0);
				break;
			case NPC_WeaponType.SHOTGUN:
				for (int i = 0; i < 5; i++)
				{
					GameObject birdshot = GameObject.Instantiate(proyectilePrefab, weaponPivot.position, weaponPivot.rotation) as GameObject;
					birdshot.transform.Rotate(0, Random.Range(-15, 15), 0);
				}
				break;
		}
	}

	void RandomRotate()
	{
		float randomAngle = Random.Range(45, 180);
		float randomSign = Random.Range(0, 2);
		if (randomSign == 0)
			randomAngle *= -1;

		transform.Rotate(0, randomAngle, 0);
	}

	public bool HasReachedMyDestination()
	{
		float dist = Vector3.Distance(transform.position, navMeshAgent.destination);
		if (dist <= 1.5f)
		{
			return true;
		}

		return false;
	}

	public void SetTargetPos(Vector3 newPos)
	{
		targetPos = newPos;
	}

	public void SetAlertPos(Vector3 newPos)
	{
		targetPos = newPos;
		canHearPlayer = true;
	}

	public void Damage()
	{
		navMeshAgent.velocity = Vector3.zero;
		//navMeshAgent.Stop ();
		npcAnimator.SetBool("Dead", true);
		GameManager.AddScore(100);
		npcAnimator.transform.parent = null;
		Vector3 pos = npcAnimator.transform.position;
		pos.y = 0.2f;
		npcAnimator.transform.position = pos;
		GameManager.RemoveEnemy();
		Destroy(gameObject);
	}

	////////////////////////////// Action: IDLE //////////////////////////////
	void ActionInit_Idle()
	{
		SetAction(NPC_EnemyAction.IDLE);
		navMeshAgent.SetDestination(startingPos);
		navMeshAgent.isStopped = false;
	}

	void ActionUpdate_Idle() { }

	void ActionEnd_Idle() {	}
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////// Action: INSPECT /////////////////////////////
	Misc_Timer inspectTimer = new Misc_Timer();
	Misc_Timer inspectTurnTimer = new Misc_Timer();
	bool inspectWait;
	void ActionInit_Inspect()
	{
		SetAction(NPC_EnemyAction.INSPECT);
		navMeshAgent.speed = 16.0f;
		navMeshAgent.isStopped = false;
		inspectTimer.StopTimer();
		inspectWait = false;
		inAttackRange = false;
	}
	void ActionUpdate_Inspect()
	{
		if (HasReachedMyDestination() && !inspectWait)
		{
			inspectWait = true;
			inspectTimer.StartTimer(2.0f);
			inspectTurnTimer.StartTimer(1.0f);
		}
		navMeshAgent.SetDestination(targetPos);
		RaycastHit hit;
		Physics.Raycast(transform.position, transform.forward, out hit, weaponRange, hitTestLayer);

		if (hit.collider != null && hit.collider.tag == "Player")
		{
			inAttackRange = true;
		}
		if (inspectWait)
		{
			inspectTimer.UpdateTimer();
			inspectTurnTimer.UpdateTimer();
			if (inspectTurnTimer.IsFinished())
			{
				RandomRotate();
				inspectTurnTimer.StartTimer(Random.Range(0.5f, 1.25f));
			}
			if (inspectTimer.IsFinished())
				SetAction(NPC_EnemyAction.IDLE);
		}
	}
	void ActionEnd_Inspect() { }
	//////////////////////////////////////////////////////////////////////////

	///////////////////////////// Action: ATTACK /////////////////////////////
	Misc_Timer attackActionTimer = new Misc_Timer();
	bool actionDone;
	void ActionInit_Attack()
	{
		SetAction(NPC_EnemyAction.ATTACK);
		navMeshAgent.isStopped = true;
		navMeshAgent.velocity = Vector3.zero;
		npcAnimator.SetBool("Attack", true);
		CancelInvoke("AttackAction");
		Invoke("AttackAction", weaponActionTime);
		attackActionTimer.StartTimer(weaponTime);

		actionDone = false;
	}
	void ActionUpdate_Attack()
	{
		attackActionTimer.UpdateTimer();
		if (!actionDone && attackActionTimer.IsFinished())
		{
			actionDone = true;
			inAttackRange = false;
		}
	}
	void ActionEnd_Attack()
	{
		npcAnimator.SetBool("Attack", false);
	}
	//////////////////////////////////////////////////////////////////////////
	
	///////////////////////////// Action: PATROL /////////////////////////////
	void ActionInit_Patrol()
	{
		SetAction(NPC_EnemyAction.PATROL);
		navMeshAgent.speed = 6.0f;
		navMeshAgent.SetDestination(patrolNode.GetPosition());
	}
	void ActionUpdate_Patrol()
	{
		if (HasReachedMyDestination())
		{
			patrolNode = patrolNode.nextNode;
			navMeshAgent.SetDestination(patrolNode.GetPosition());
		}
	}
	void ActionEnd_Patrol() { }
	//////////////////////////////////////////////////////////////////////////
}