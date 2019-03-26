using System.Collections.Generic;
using UnityEngine;

public class MultishotTrapBehaviour : AbstractTrapBehaviour
{

	void Start()
	{
		base.Start();
	}

	void Update()
	{
		base.Update();
	}


	public override void Fire(List<CreepsBehaviour> targetsCreepsBehaviours)
	{
		foreach (var target in targetsCreepsBehaviours)
		{
			if (target == null)
				return;

			var bullet = GameObject.Instantiate(this.TrapBulletPrefab, this.transform.position, Quaternion.identity);

			bullet.SetBulletModelFromTrap(this.Model, target);
		}

		base.Fire(null);
	}
}