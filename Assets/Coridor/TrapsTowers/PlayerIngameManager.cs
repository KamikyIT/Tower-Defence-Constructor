using System;
using Assets.Scenes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

internal class PlayerIngameManager: MonoBehaviour
{
	public BrainsColorAnim BrainsColorAnim;

	public int Money;
	public int Brains;

	public Text MoneyText;
	public Text BrainsText;

	void Start()
	{
		BrainsColorAnim = GameObject.FindObjectOfType<BrainsColorAnim>();

		Brains = MySceneManager.GetBrains();
	}

	void Update()
	{
		MoneyText.text = Money.ToString();

		BrainsText.text = Brains.ToString();
	}

	private CreepsLevelManager creepsLevelManager
	{
		get { return GameObject.FindObjectOfType<CreepsLevelManager>(); }
	}

	public bool CanUpgradeTower(AbstractTrapBehaviour selectedTrap)
	{
		return Money >= selectedTrap.Model.MoneyCost;
	}

	public void UpgradeTower(AbstractTrapBehaviour selectedTrap)
	{
		selectedTrap.UpgadeTrap();

		Money -= selectedTrap.Model.MoneyCost;
	}

	public bool CanCreateTrap(TrapType trapType)
	{
		var toCreateTrap = TrapModel.GetTrapModelInfo(trapType);

		return Money >= toCreateTrap.MoneyCost;
	}

	public void CreateNewTrap(TrapType trapType)
	{
		var toCreateTrap = TrapModel.GetTrapModelInfo(trapType);

		Money -= toCreateTrap.MoneyCost;
	}

	public void KilledCreep(CreepsBehaviour creepsBehaviour)
	{
		BackBrains(creepsBehaviour);

		this.Money += creepsBehaviour.Model.KillReward;
	}

	public void RemoveCreepFromGame(CreepsBehaviour creepsBehaviour)
	{
		creepsLevelManager.KilledCreep(creepsBehaviour);

		creepsBehaviour.gameObject.SetActive(false);
	}

	public void BackBrains(CreepsBehaviour creepsBehaviour)
	{
		Brains += creepsBehaviour.StolenBrains;

		BrainsColorAnim.BrainsChanged(Brains);

		creepsBehaviour.StolenBrains = 0;
	}

	public void StealBrain(CreepsBehaviour creepsBehaviour)
	{
		var creepToStealBrains = creepsBehaviour.Model.BrainSteal;

		if (Brains > creepToStealBrains)
		{
			Brains -= creepToStealBrains;

			creepsBehaviour.StolenBrains = creepToStealBrains;
		}
		else
		if (Brains > 0)
		{
			creepsBehaviour.StolenBrains = Brains;

			Brains = 0;
		}
		else
			// Brains == 0
			creepsBehaviour.StolenBrains = 0;

		BrainsColorAnim.BrainsChanged(Brains);
	}
}