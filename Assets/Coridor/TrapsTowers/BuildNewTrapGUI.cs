using System;
using UnityEngine;

public class BuildNewTrapGUI : TrapGuiBase
{
	private PlayerIngameManager playerIngameManager
	{
		get { return GameObject.FindObjectOfType<PlayerIngameManager>(); }
	}

	public void BuildTower(string towerName)
	{
		var trapType = (TrapType) Enum.Parse(typeof(TrapType), towerName, true);

		if (playerIngameManager.CanCreateTrap(trapType))
			CreateTower(trapType);
	}

	private void CreateTower(TrapType trapType)
	{
		if (playerIngameManager.CanCreateTrap(trapType) == false) return;

		this.TrapPlace.CreateNewTrap(trapType);
		playerIngameManager.CreateNewTrap(trapType);
	}

	protected override void ChildButtonsLogicActivate()
	{
		AllButtons.ForEach(x => x.enabled = true);
	}
}