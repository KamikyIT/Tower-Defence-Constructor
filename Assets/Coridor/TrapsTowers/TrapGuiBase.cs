using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class TrapGuiBase : MonoBehaviour
{
	public TrapPlace TrapPlace;

	protected Animator anim
	{
		get { return GetComponent<Animator>(); }
	}

	public List<Button> AllButtons;

	void Start()
	{
		AllButtons = GetComponentsInChildren<Button>().ToList();
	}

	public virtual void SetCurrentSelectedTrapPlace(TrapPlace trapPlace)
	{
		this.TrapPlace = trapPlace;
	}

	public void StartShowingAnimation()
	{
		AllButtons.ForEach(x => x.enabled = false);

		anim.Play("Opening");

		StartCoroutine(ActivateButtonsCor());
	}

	private IEnumerator ActivateButtonsCor()
	{
		yield return new WaitForSeconds(1f);

		ChildButtonsLogicActivate();
	}

	protected abstract void ChildButtonsLogicActivate();

	public void StartClosingAnimation()
	{
		AllButtons.ForEach(x => x.enabled = false);

		anim.Play("Closing");

		StartCoroutine(ClosingCor());
	}

	private IEnumerator ClosingCor()
	{
		yield return new WaitForSeconds(1f);

		TrapPlace = null;

		TrapPlace.ShowingMenu = false;

		this.gameObject.SetActive(false);
	}

	public void CloseButtonClick()
	{
		StartClosingAnimation();
	}
}