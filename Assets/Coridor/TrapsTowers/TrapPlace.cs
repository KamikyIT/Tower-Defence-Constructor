using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class TrapPlace : MonoBehaviour
{
	public TrapsManager trapsManager;

	public TrapMenuGui TrapMenuGUI;

	public static bool ShowingMenu = false;

	public AbstractTrapBehaviour CurrentTrap;

	void Start()
	{
		TrapMenuGUI = GameObject.FindObjectOfType<TrapMenuGui>();
		trapsManager = GameObject.FindObjectOfType<TrapsManager>();
	}

	void OnMouseDown()
	{
		if (ShowingMenu)
			return;

		ShowingMenu = true;

		ShowTrapPlaceMenu();
	}

	void OnMouseOver()
	{
		if (Input.GetMouseButton(1) == false)
			return;

		CurrentTrap.UseAbility();
	}


	private void ShowTrapPlaceMenu()
	{
		TrapMenuGUI.gameObject.SetActive(true);

		TrapMenuGUI.ShowMenu(this);
	}

	public bool HasTrap()
	{
		return this.CurrentTrap != null;
	}

	public void CreateNewTrap(TrapType trapType)
	{
		var trapTemplate = trapsManager.GetTrapOfType(trapType);

		var newTrapGo = GameObject.Instantiate(trapTemplate);

		//this.CurrentTrap = newTrapGo;

		newTrapGo.SetTrapPlace(this);

		newTrapGo.Model = TrapModel.GetTrapModelInfo(trapType);

		TrapMenuGUI.StartClosingCurrentGui();
		
		Debug.Log("Building " + trapType);

		GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);

		var particle = GetComponentInChildren<ParticleSystem>(true);
		particle.gameObject.SetActive(true);
		var particleMain = particle.main;

		switch (newTrapGo.Model.AttackType)
		{
			case TrapAttackType.Fear:
				particleMain.startColor = new ParticleSystem.MinMaxGradient(new Color(110f/255f, 18f/255f, 223f/255f));
				break;
			case TrapAttackType.Light:
				particleMain.startColor = new ParticleSystem.MinMaxGradient(new Color(224f / 255f, 248f / 255f, 5f / 255f));
				break;
			case TrapAttackType.Chaos:
				particleMain.startColor = new ParticleSystem.MinMaxGradient(new Color(51f / 255f, 135f / 255f, 44f / 255f));
				break;
			case TrapAttackType.Poison:
				particleMain.startColor = new ParticleSystem.MinMaxGradient(new Color(55f / 255f, 165f / 255f, 165f / 255f));
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}


	}
}

public enum TrapType
{
	Fear,
	Void,
	Oblivion,
	Envy,
	Madness,
	Hypocrisy,
	Lust,
	Pride
}