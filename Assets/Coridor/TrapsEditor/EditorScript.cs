using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorScript : MonoBehaviour
{
	public GameObject ButtonsPanel;
	
	public GameObject CreepsPanel;
	public GameObject TowersPanel;
	public GameObject LevelPanel;


	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CreepsPanelShow()
	{
		ButtonsPanel.gameObject.SetActive(false);
		CreepsPanel.gameObject.SetActive(true);
	}

	public void TowersPanelShow()
	{
		ButtonsPanel.gameObject.SetActive(false);
		TowersPanel.gameObject.SetActive(true);
	}

	public void LevelPanelShow()
	{
		ButtonsPanel.gameObject.SetActive(false);
		LevelPanel.gameObject.SetActive(true);
	}

	public void ClosePanel()
	{
		ButtonsPanel.gameObject.SetActive(true);

		CreepsPanel.gameObject.SetActive(false);
		TowersPanel.gameObject.SetActive(false);
		LevelPanel.gameObject.SetActive(false);
	}
}
