using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowCurrentTrapGUI : TrapGuiBase
{
	private PlayerIngameManager playerIngameManager
	{
		get { return GameObject.FindObjectOfType<PlayerIngameManager>(); }
	}

	public Button CloseButton;

	public Button UpGradeButton;

	public Text TrapName;
	public Text DamageText;
	public Text AttackSpeedText;

	public Text UpgradeTrapName;
	public Text UpgradeDamageText;
	public Text UpgradeAttackSpeedText;
	public Text UpgradeCost;

	public Image UpgradeTrapInfoPanel;

	public Image TrapImage;

	public Image FearType;
	public Image LightType;
	public Image ChaosType;
	public Image PoisonType;

	public Image FrostAbility;
	public Image EnvenomAbility;
	public Image StealAbility;
	public Image GhostAbility;

	public Image UpgradeFrostAbility;
	public Image UpgradeEnvenomAbility;
	public Image UpgradeStealAbility;
	public Image UpgradeGhostAbility;

	public void UpgradeButtonClick()
	{
		Debug.Log("UpgradeButtonClick");

		playerIngameManager.UpgradeTower(CurrentTrap);

		CloseButtonClick();
	}

	public AbstractTrapBehaviour CurrentTrap
	{
		get { return this.TrapPlace.CurrentTrap; }
	}

	protected override void ChildButtonsLogicActivate()
	{
		CloseButton.enabled = true;

		SetUpgradeButtonEnable();
	}

	void Update()
	{
		if (CurrentTrap == null) return;

		SetUpgradeButtonEnable();
	}

	private void SetUpgradeButtonEnable()
	{
		if (CurrentTrap == null) return;

		if (CurrentTrap.IsMaxLevelUpgrade()) return;

		UpGradeButton.enabled = playerIngameManager.CanUpgradeTower(CurrentTrap);
	}

	public void DisplayCurrentTrapInfo(AbstractTrapBehaviour trap)
	{
		this.TrapPlace = trap.TrapPlace;

		this.TrapName.text = trap.Model.TrapName;
		this.DamageText.text = string.Format("{0} - {1}", trap.Model.MinDamage, trap.Model.MaxDamage);
		this.AttackSpeedText.text = trap.Model.AttackSpeed.ToString("F1");
		this.TrapImage.overrideSprite = trap.GetComponentInChildren<SpriteRenderer>().sprite;
		this.TrapImage.color = trap.GetComponentInChildren<SpriteRenderer>().color;

		this.UpgradeTrapName.gameObject.SetActive(!trap.Model.IsMaxLevelUpgrade());
		this.UpgradeTrapInfoPanel.gameObject.SetActive(!trap.Model.IsMaxLevelUpgrade());
		this.UpgradeCost.gameObject.SetActive(!trap.Model.IsMaxLevelUpgrade());
		this.UpGradeButton.gameObject.SetActive(!trap.Model.IsMaxLevelUpgrade());

		if (trap.Model.IsMaxLevelUpgrade() == false)
		{
			this.UpgradeTrapName.text = trap.Model.NextUpgradeTrap.TrapName;
			this.UpgradeDamageText.text = string.Format("{0} - {1}", trap.Model.NextUpgradeTrap.MinDamage, trap.Model.NextUpgradeTrap.MaxDamage);
			this.UpgradeAttackSpeedText.text = trap.Model.NextUpgradeTrap.AttackSpeed.ToString("F1");
			this.UpgradeCost.text = trap.Model.NextUpgradeTrap.MoneyCost.ToString();
		}

		#region AttackType

		FearType.color = new Color(1f, 1f, 1f, 0.3f);
		LightType.color = new Color(1f, 1f, 1f, 0.3f);
		ChaosType.color = new Color(1f, 1f, 1f, 0.3f);
		PoisonType.color = new Color(1f, 1f, 1f, 0.3f);

		switch (trap.Model.AttackType)
		{
			case TrapAttackType.Fear:
				FearType.color = new Color(1f, 1f, 1f, 1f);
				break;
			case TrapAttackType.Light:
				LightType.color = new Color(1f, 1f, 1f, 1f);
				break;
			case TrapAttackType.Chaos:
				ChaosType.color = new Color(1f, 1f, 1f, 1f);
				break;
			case TrapAttackType.Poison:
				PoisonType.color = new Color(1f, 1f, 1f, 1f);
				break;
		}

		#endregion

		#region Ability
		
		FrostAbility.color = new Color(1f, 1f, 1f, 0.3f);
		EnvenomAbility.color = new Color(1f, 1f, 1f, 0.3f);
		StealAbility.color = new Color(1f, 1f, 1f, 0.3f);
		GhostAbility.color = new Color(1f, 1f, 1f, 0.3f);

		switch (trap.Model.AbilityType)
		{
			case TrapAbilityType.None:
				break;
			case TrapAbilityType.Frost:
				FrostAbility.color = new Color(1f, 1f, 1f, 1f);
				break;
			case TrapAbilityType.Envenom:
				EnvenomAbility.color = new Color(1f, 1f, 1f, 1f);
				break;
			case TrapAbilityType.Steal:
				StealAbility.color = new Color(1f, 1f, 1f, 1f);
				break;
			case TrapAbilityType.Ghost:
				GhostAbility.color = new Color(1f, 1f, 1f, 1f);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		#endregion

		#region Upgrade Ability

		if (trap.Model.IsMaxLevelUpgrade() == false)
		{
			UpgradeFrostAbility.color = new Color(1f, 1f, 1f, 0.3f);
			UpgradeEnvenomAbility.color = new Color(1f, 1f, 1f, 0.3f);
			UpgradeStealAbility.color = new Color(1f, 1f, 1f, 0.3f);
			UpgradeGhostAbility.color = new Color(1f, 1f, 1f, 0.3f);

			switch (trap.Model.NextUpgradeTrap.AbilityType)
			{
				case TrapAbilityType.None:
					break;
				case TrapAbilityType.Frost:
					UpgradeFrostAbility.color = new Color(1f, 1f, 1f, 1f);
					break;
				case TrapAbilityType.Envenom:
					UpgradeEnvenomAbility.color = new Color(1f, 1f, 1f, 1f);
					break;
				case TrapAbilityType.Steal:
					UpgradeStealAbility.color = new Color(1f, 1f, 1f, 1f);
					break;
				case TrapAbilityType.Ghost:
					UpgradeGhostAbility.color = new Color(1f, 1f, 1f, 1f);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		#endregion

	}

	public override void SetCurrentSelectedTrapPlace(TrapPlace trapPlace)
	{
		base.SetCurrentSelectedTrapPlace(trapPlace);

		DisplayCurrentTrapInfo(trapPlace.CurrentTrap);
	}
}