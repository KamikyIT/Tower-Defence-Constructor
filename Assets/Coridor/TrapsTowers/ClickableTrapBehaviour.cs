using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickableTrapBehaviour : AbstractTrapBehaviour
{
	// Use this for initialization
	void Start ()
	{
		base.Start();
	}
	
	// Update is called once per frame
	void Update ()
	{
		base.Update();
	}


	public override void Fire(List<CreepsBehaviour> targetsCreepsBehaviours)
	{
		var target = targetsCreepsBehaviours.FirstOrDefault();

		if (target == null)
			return;

		var bullet = GameObject.Instantiate(this.TrapBulletPrefab, this.transform.position, Quaternion.identity);

		bullet.SetBulletModelFromTrap(this.Model, target);

		base.Fire(null);
	}
}