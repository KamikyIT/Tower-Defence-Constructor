using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreepsBehaviour : MonoBehaviour
{
	private PlayerIngameManager playerManager
	{
		get { return GameObject.FindObjectOfType<PlayerIngameManager>(); }
	}

	public int CreepWayId;
	[SerializeField]
	private Vector2[] _points;

	public CreepModel Model;

	public CreepsBehaviourState State = CreepsBehaviourState.Borning;

	public List<CreepBuff> Buffs;

	private Action ActingDueState;
	private bool _flagBusy;
	private int _moveToPointIndex;

	private Animator anim;

	public bool FacingLeftToRight;
	public int StolenBrains;

	// Use this for initialization
	void Start ()
	{
		InitPoints();

		Buffs = new List<CreepBuff>();

		anim = GetComponent<Animator>();

		SetAction(CreepsBehaviourState.Borning);

		transform.position = _points[0];
	}

	// Update is called once per frame
	void Update()
	{

		ActingDueState();
	}

	private void SetAction(CreepsBehaviourState state)
	{

		State = state;

		switch (State)
		{
			case CreepsBehaviourState.Borning:
				anim.Play("born");
				ActingDueState = Borning;
				break;
			case CreepsBehaviourState.MovingForward:
				anim.Play("walk");
				ActingDueState = MovingForward;
				break;
			case CreepsBehaviourState.MovingBack:
				anim.Play("walk");
				ActingDueState = MovingBack;
				break;
			case CreepsBehaviourState.Dying:
				anim.Play("die");
				playerManager.KilledCreep(this);
				ActingDueState = Dying;
				break;
			case CreepsBehaviourState.Finished:
				ActingDueState = Finished;
				break;
			default:
				break;
		}
	}

	private void Finished()
	{
		if (_flagBusy)
			return;

		_flagBusy = false;

		StartCoroutine(FinishedCoroutine());
	}

	private IEnumerator FinishedCoroutine()
	{
		yield return new WaitForSeconds(Model.FinishedTime);

		NextState();
	}

	private void Dying()
	{
		if (_flagBusy)
			return;

		_flagBusy = false;

		//playerManager.KilledCreep(this);

		StartCoroutine(DyingCoroutine());
	}

	private IEnumerator DyingCoroutine()
	{
		yield return new WaitForSeconds(Model.DyingTime);

		NextState();
	}

	private void MovingBack()
	{
		var posNow = (Vector2)transform.position;

		var nextPoint = _points[_moveToPointIndex];

		var dif = (nextPoint - posNow).magnitude;

		var step = Time.deltaTime * Model.MoveSpeed;

		if (dif < 1.5f * step)
		{

			_moveToPointIndex++;

			if (_moveToPointIndex == _points.Length)
				NextState();

			return;
		}

		var direction = (nextPoint - posNow).normalized;

		transform.Translate(step * direction, Space.World);

		SetDirection(direction);
	}

	private void SetDirection(Vector2 direction)
	{
		var movingLeftToRight = direction.x > 0f;

		GetComponentInChildren<SpriteRenderer>().flipX = FacingLeftToRight ? !movingLeftToRight : movingLeftToRight;
	}

	private void MovingForward()
	{
		var posNow = (Vector2)transform.position;

		var nextPoint = _points[_moveToPointIndex];

		var dif = (nextPoint - posNow).magnitude;

		var step = Time.deltaTime * Model.MoveSpeed;

		if (dif < 1.5f * step)
		{
			
			_moveToPointIndex++;

			if (_moveToPointIndex == _points.Length)
				NextState();

			return;
		}

		var direction = (nextPoint - posNow).normalized;

		transform.Translate(step * direction, Space.World);

		SetDirection(direction);
	}

	private void Borning()
	{
		if (_flagBusy)
			return;

		anim.Play("born");

		_flagBusy = true;

		StartCoroutine(BorningCor());
	}

	private IEnumerator BorningCor()
	{
		yield return new WaitForSeconds(Model.BornTime);

		NextState();
	}

	private void NextState()
	{

		_flagBusy = false;

		if (State == CreepsBehaviourState.Borning)
		{
			_moveToPointIndex = 0;

			transform.position = _points[0];

			SetAction(CreepsBehaviourState.MovingForward);

			return;
		}

		if (State == CreepsBehaviourState.MovingForward)
		{
			_moveToPointIndex = 0;

			transform.position = _points.Last();
			
			_points = _points.Reverse().ToArray();

			playerManager.StealBrain(this);

			SetAction(CreepsBehaviourState.MovingBack);

			return;
		}

		if (State == CreepsBehaviourState.MovingBack)
		{
			OnFinished();

			SetAction(CreepsBehaviourState.Finished);

			return;
		}

		if (State == CreepsBehaviourState.Dying)
		{
			OnDead();

			SetAction(CreepsBehaviourState.Dying);

			return;
		}
	}

	private void OnDead()
	{
		Debug.Log("Сдох крип.");

		playerManager.RemoveCreepFromGame(this);

		//GameObject.Destroy(this.gameObject);
		gameObject.SetActive(false);
	}

	private void OnFinished()
	{
		// TODO: Крип съебалсо.
		Debug.Log("Крип съебалсо.");

		playerManager.RemoveCreepFromGame(this);

		//GameObject.Destroy(this.gameObject);
		gameObject.SetActive(false);
	}

	private void InitPoints()
	{
		var creepWays = FindObjectsOfType<CreepWay>();
		if (creepWays == null || creepWays.Length == 0)
		{
			Debug.LogError("creep == null || creep.Length == 0");
			return;
		}

		var thatCreepWay = creepWays.FirstOrDefault(x => x.Id == CreepWayId);

		if (thatCreepWay == null)
		{
			Debug.LogError("thatCreepWay == null. Id = " + this.CreepWayId);
			return;
		}

		this._points = thatCreepWay.GetVectorPoints();
	}

	public string GetCurrentMethod()
	{
		var mb = System.Reflection.MethodBase.GetCurrentMethod();
		return mb.Name;
	}

	public bool IsAlive()
	{
		return Model.Hp > 0 && (State == CreepsBehaviourState.MovingForward || State == CreepsBehaviourState.MovingBack);
	}

	public void FrostAbility(AbstractTrapBehaviour abstractTrapBehaviour)
	{
		var trapsManager = GameObject.FindObjectOfType<TrapsManager>();

		var abilityGoPrefab = trapsManager.GetAbilityEffectForCreep(abstractTrapBehaviour.Model.AbilityType);

		var go = GameObject.Instantiate(abilityGoPrefab);

		go.transform.position = this.transform.position;
		
		var moveSpeed = this.Model.MoveSpeed;
		this.Model.MoveSpeed = 0f;

		var animSpeed = anim.speed;
		anim.speed = 0f;

		StartCoroutine(FrostCor(moveSpeed, animSpeed, go, 2f));
	}

	private IEnumerator FrostCor(float prevMoveSpeed, float prevAnimSpeed, AbilityEffectForCreep abilityGo, float time, CreepBuff buff = null)
	{
		yield return new WaitForSeconds(time);

		Destroy(abilityGo.gameObject);

		if (buff != null)
			this.Buffs.Remove(buff);

		this.Model.MoveSpeed = prevMoveSpeed;
		anim.speed = prevAnimSpeed;
	}

	public void EnvenomAbility(AbstractTrapBehaviour abstractTrapBehaviour)
	{
		var trapsManager = GameObject.FindObjectOfType<TrapsManager>();

		var abilityGoPrefab = trapsManager.GetAbilityEffectForCreep(abstractTrapBehaviour.Model.AbilityType);

		var go = GameObject.Instantiate(abilityGoPrefab);

		go.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.05f, this.transform.position.z);

		go.transform.parent = this.transform;

		go.transform.localScale *= 0.3f;

		var moveSpeed = this.Model.MoveSpeed;
		this.Model.MoveSpeed = moveSpeed * 0.3f;

		var animSpeed = anim.speed;
		anim.speed = 0.3f;

		StartCoroutine(FrostCor(moveSpeed, animSpeed, go, 5f));
	}

	public void StealAbility(AbstractTrapBehaviour abstractTrapBehaviour)
	{
		playerManager.BackBrains(this);	
	}

	public void GhostAbility(AbstractTrapBehaviour abstractTrapBehaviour)
	{
		var trapsManager = GameObject.FindObjectOfType<TrapsManager>();

		var abilityGoPrefab = trapsManager.GetAbilityEffectForCreep(abstractTrapBehaviour.Model.AbilityType);

		var go = GameObject.Instantiate(abilityGoPrefab);

		go.transform.position = this.transform.position;

		go.transform.parent = this.transform;

		go.transform.localScale *= 0.5f;

		var moveSpeed = this.Model.MoveSpeed;
		this.Model.MoveSpeed = moveSpeed * 0.5f;

		this.Buffs.Add(new CreepBuff("ghost"));

		var animSpeed = anim.speed;
		anim.speed = 0.5f;

		StartCoroutine(FrostCor(moveSpeed, animSpeed, go, 5f));
	}

	public void Damaged(TrapBulletModel model)
	{
		this.Model.Hp -= model.Damage;

		if (Model.Hp <= 0)
		{
			_flagBusy = false;

			SetAction(CreepsBehaviourState.Dying);
		}
	}
}

public enum CreepsBehaviourState
{
	Borning,
	MovingForward,
	MovingBack,
	Dying,
	Finished,
}

[Serializable]
public class CreepModel
{
	public int Hp = 1;
	public float MoveSpeed = 1f;
	
	//public List<CreepBuff> Buffs;

	public float BornTime = 1f;
	internal float DyingTime = 1f;
	internal float FinishedTime = 1f;
	public int KillReward = 10;
	public int BrainSteal;
}

[Serializable]
public class CreepBuff
{
	public CreepBuff(string buffName)
	{
		this.buffName = buffName;
	}

	public string buffName;
}
