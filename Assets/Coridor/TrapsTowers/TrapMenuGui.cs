using System.Collections;
using UnityEngine;

public class TrapMenuGui : MonoBehaviour
{
	public bool showingAnimation;
	private BuildNewTrapGUI BuildNewTrapGUI;
	private ShowCurrentTrapGUI ShowCurrentTrapGUI;

	private TrapPlace selectedTrapPlace;

	private TrapGuiBase trap;

	void Start()
	{
		showingAnimation = false;

		BuildNewTrapGUI = GetComponentInChildren<BuildNewTrapGUI>(true);
		ShowCurrentTrapGUI = GetComponentInChildren<ShowCurrentTrapGUI>(true);

		BuildNewTrapGUI.gameObject.SetActive(false);
		ShowCurrentTrapGUI.gameObject.SetActive(false);
	}

	public void ShowMenu(TrapPlace trapPlace)
	{
		//if (showingAnimation)
		//	return;

		//showingAnimation = true;

		selectedTrapPlace = trapPlace;

		ActivateMenu();
	}

	private void ActivateMenu()
	{
		if (selectedTrapPlace.HasTrap())
			trap = ShowCurrentTrapGUI;
		else
			trap = BuildNewTrapGUI;

		trap.gameObject.SetActive(true);

		trap.SetCurrentSelectedTrapPlace(selectedTrapPlace);

		trap.StartShowingAnimation();
	}

	public void StartClosingCurrentGui()
	{
		trap.StartClosingAnimation();
	}
}