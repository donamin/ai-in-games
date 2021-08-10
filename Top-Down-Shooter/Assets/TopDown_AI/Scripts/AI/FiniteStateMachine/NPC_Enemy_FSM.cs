using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Enemy_FSM : MonoBehaviour
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

	public NPC_EnemyAction currentAction = NPC_EnemyAction.NONE;

	bool canHearPlayer = false;

	static NPC_Enemy_FSM rifleSolider = null, shotgunSolider = null;

	// Start is called before the first frame update
	void Start()
	{
		startingPos = transform.position;
		hashSpeed = Animator.StringToHash("Speed");
		SetWeapon(weaponType);
		GameManager.AddToEnemyCount();
		GoToState(NPC_EnemyAction.IDLE);
	}

	// Update is called once per frame
	void Update()
	{
		npcAnimator.SetFloat(hashSpeed, navMeshAgent.velocity.magnitude);

		switch (currentAction)
		{
			case NPC_EnemyAction.IDLE:
				ActionUpdate_Idle();
				break;
			case NPC_EnemyAction.INSPECT:
				ActionUpdate_Inspect();
				break;
			case NPC_EnemyAction.PATROL:
				ActionUpdate_Patrol();
				break;
			case NPC_EnemyAction.ATTACK:
				ActionUpdate_Attack();
				break;
		}

		canHearPlayer = false;
	}

	void GoToState(NPC_EnemyAction newState)
	{
		if (currentAction != NPC_EnemyAction.NONE)
		{
			switch (currentAction)
			{
				case NPC_EnemyAction.IDLE:
					ActionEnd_Idle();
					break;
				case NPC_EnemyAction.INSPECT:
					ActionEnd_Inspect();
					break;
				case NPC_EnemyAction.PATROL:
					ActionEnd_Patrol();
					break;
				case NPC_EnemyAction.ATTACK:
					ActionEnd_Attack();
					break;
			}
		}
		currentAction = newState;
		switch (currentAction)
		{
			case NPC_EnemyAction.IDLE:
				ActionInit_Idle();
				break;
			case NPC_EnemyAction.INSPECT:
				ActionInit_Inspect();
				break;
			case NPC_EnemyAction.PATROL:
				ActionInit_Patrol();
				break;
			case NPC_EnemyAction.ATTACK:
				ActionInit_Attack();
				break;
		}
	}

	public string GetStateText()
	{
		string res = "";
		switch (currentAction)
		{
			case NPC_EnemyAction.IDLE:
				res = "IDLE";
				break;
			case NPC_EnemyAction.INSPECT:
				res = "INSPECT";
				break;
			case NPC_EnemyAction.PATROL:
				res = "PATROL";
				break;
			case NPC_EnemyAction.ATTACK:
				res = "ATTACK";
				break;
		}
		res += ", ";
		switch (weaponType)
		{
			case NPC_WeaponType.KNIFE:
				res += "KNIFE";
				break;
			case NPC_WeaponType.RIFLE:
				res += "RIFLE";
				break;
			case NPC_WeaponType.SHOTGUN:
				res += "SHOTGUN";
				break;
		}
		return res;
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

	void SetWeapon(NPC_WeaponType newWeapon)
	{
		weaponType = newWeapon;
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
				weaponRange = 10.0f;
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
		//TODO: YOUR CODE HERE (Q1): A soldier has been killed. Update the static variables (rifleSolider & shotgunSolider) accordingly.
		
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

	void AssignWeapons()
	{
		//TODO: YOUR CODE HERE (Q1): Set the weapon type using the static variables (rifleSolider & shotgunSolider).
		//Remember to update the static variables accordingly!
	}

	////////////////////////////// Action: IDLE //////////////////////////////
	void ActionInit_Idle()
	{
		navMeshAgent.SetDestination(startingPos);
		navMeshAgent.isStopped = false;
		//Q1: The player has been lost or killed. The static variables (rifleSolider & shotgunSolider) are now updated.
		if (rifleSolider == this)
		{
			rifleSolider = null;
		}
		else if (shotgunSolider == this)
		{
			shotgunSolider = null;
		}
	}

	void ActionUpdate_Idle()
	{
		//Q1: The last two conditions check if there is a rifle attacking solider or a shotgun attacking solider in the team!
		if (CanSeePlayer() || CanHearPlayer() || rifleSolider != null || shotgunSolider != null)
		{
			GoToState(NPC_EnemyAction.INSPECT);
		}
	}

	void ActionEnd_Idle()
	{
		AssignWeapons();
	}
	//////////////////////////////////////////////////////////////////////////

	//////////////////////////// Action: INSPECT /////////////////////////////
	Misc_Timer inspectTimer = new Misc_Timer();
	Misc_Timer inspectTurnTimer = new Misc_Timer();
	bool inspectWait;
	void ActionInit_Inspect()
	{
		navMeshAgent.speed = 16.0f;
		navMeshAgent.isStopped = false;
		inspectTimer.StopTimer();
		inspectWait = false;
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
			GoToState(NPC_EnemyAction.ATTACK);
		}
		else if (CanSeePlayer())
		{
			transform.forward = targetPos - transform.position;
		}
		else if (inspectWait)
		{
			inspectTimer.UpdateTimer();
			inspectTurnTimer.UpdateTimer();
			if (inspectTurnTimer.IsFinished())
			{
				RandomRotate();
				inspectTurnTimer.StartTimer(Random.Range(0.5f, 1.25f));
			}
			if (inspectTimer.IsFinished())
			{
				GoToState(NPC_EnemyAction.IDLE);
			}
		}
	}
	void ActionEnd_Inspect()
	{
	}
	//////////////////////////////////////////////////////////////////////////

	///////////////////////////// Action: ATTACK /////////////////////////////
	Misc_Timer attackActionTimer = new Misc_Timer();
	bool actionDone;
	void ActionInit_Attack()
	{
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
			GoToState(NPC_EnemyAction.INSPECT);
		}
	}
	void ActionEnd_Attack()
	{
		npcAnimator.SetBool("Attack", false);
		AssignWeapons();
	}
	//////////////////////////////////////////////////////////////////////////

	///////////////////////////// Action: PATROL /////////////////////////////
	void ActionInit_Patrol()
	{
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