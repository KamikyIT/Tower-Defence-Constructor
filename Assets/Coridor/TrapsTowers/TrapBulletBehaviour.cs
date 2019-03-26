using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrapBulletBehaviour : MonoBehaviour
{
	public TrapBulletModel Model;

	public CreepsBehaviour targetCreepsa;
	public bool ReadyForBattle;

	public void SetBulletModelFromTrap(TrapModel trapModel, CreepsBehaviour targetCreepsa)
	{
		Model = new TrapBulletModel(trapModel);

		this.targetCreepsa = targetCreepsa;

		ReadyForBattle = true;
	}

	void Update()
	{
		if (ReadyForBattle == false)
			return;

		if (targetCreepsa == null || (targetCreepsa.gameObject.activeSelf == false))
		{
			Destroy(this.gameObject);
			return;
		}

		var posNow = transform.position;

		var creepPos = targetCreepsa.transform.position;

		if ((Math.Abs(creepPos.x - posNow.x) < 0.2f) &&
		    (Math.Abs(creepPos.y - posNow.y) < 0.2f))
		{
			Destroy(this.gameObject);

			targetCreepsa.Damaged(this.Model);
			return;
		}

		transform.rotation.SetLookRotation(targetCreepsa.transform.position);

		var dir = (creepPos - posNow).normalized;

		transform.Translate(dir * Model.MoveSpeed * Time.deltaTime, Space.Self);
	}
}

[Serializable]
public class TrapBulletModel
{
	public float MoveSpeed;

	public int Damage;

	public TrapAttackType AttackType;

	public TrapBulletModel(TrapModel trapModel)
	{
		MoveSpeed = MoveSpeedFromAttackType(trapModel.AttackType);

		Damage = Random.Range(trapModel.MinDamage, trapModel.MaxDamage + 1);

		AttackType = trapModel.AttackType;
	}

	private static float MoveSpeedFromAttackType(TrapAttackType trapModelAttackType)
	{
		switch (trapModelAttackType)
		{
			case TrapAttackType.Fear:
				return 10f;
			case TrapAttackType.Light:
				return 25f;
			case TrapAttackType.Chaos:
				return 15f;
			case TrapAttackType.Poison:
				return 10f;
			default:
				throw new ArgumentOutOfRangeException("trapModelAttackType", trapModelAttackType, null);
		}
	}
}