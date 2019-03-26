using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreepsLevelManager: MonoBehaviour
{
	public TextAsset TextAsset;

	public List<CreepsBehaviour> AllCreepsEver;

	private List<CreepTimingWay> ListCreepTimingWay;

	public bool Playing;

	public int LevelNumber = 1;

	//public List<GameObject> currentLevelCreeps;

	/// <summary>
	/// Список строк через '\n' из описания 
	/// CreepPrefab.Name|Way|Time
	/// </summary>
	public string LevelCreeps;

	public int allCreepsCount;
	private bool creepsWereSet;

	private const string Level1 =
		"Armature | 1 | 0 " + "\n" +
		"Bird     | 2 | 2" + "\n" +
		"Armature | 1 | 4" + "\n" +
		"Bird     | 1 | 6" + "\n" +
		"Armature | 1 | 8" + "\n" +
		"Bird     | 1 | 10" + "\n" +
		"Armature | 2 | 12" + "\n";

	//PrefabName | WayId | Timing | Hp | MoveSpeed | KillReward | BrainsSteal
	private CreepTimingWay CreateCripFromLine(string line)
	{
		if (string.IsNullOrEmpty(line) || line.IsNullOrWhiteSpace())
			return null;

		if (line.StartsWith("//"))
			return null;

		var parts = line.Replace(" ", "").Replace("\r", "").Split(new char[] { '|', '\t', });

		if (parts.Length < 2)
			return null;

		var i = 0;

		var creepName = parts[i++];
		var creepInstance = this.AllCreepsEver.FirstOrDefault(x => x.name.Replace(" ", "") == creepName);
		if (creepInstance == null)
		{
			Debug.LogError("Cannot find creepName = " + creepName + "; line = " + line);
			return null;
		}

		var wayId = int.Parse(parts[i++]);

		var time = float.Parse(parts[i++]);

		//Hp | MoveSpeed | KillReward
		var hp = int.Parse(parts[i++]);
		var moveSpeed = float.Parse(parts[i++]);
		var killReward = int.Parse(parts[i++]);
		var brainSteal = int.Parse(parts[i++]);

		creepInstance.Model = new CreepModel()
		{
			Hp = hp,
			MoveSpeed = moveSpeed,
			KillReward = killReward,
			BrainSteal = brainSteal,
		};

		var creep = new CreepTimingWay()
		{
			Creep = creepInstance,
			WayId = wayId,
			Timing = time,
		};
		
		return creep;
	}

	private void Start()
	{
		//this.allCreepsCount = StartCreateCreeps();
	}

	void Update()
	{
		//if (currentLevelCreeps.Count == 0)

		if (creepsWereSet == false)
			return;

		if (ExitLevel())
			MySceneManager.CompleteTowerDefenceScene();
			
	}

	private bool ExitLevel()
	{
		return FinishedCreepsListGameObject.transform.childCount == allCreepsCount;
	}

	private int StartCreateCreeps()
	{
		foreach (var creepTiming in ListCreepTimingWay)
			StartCoroutine(CreateCreepCor(creepTiming));

		return ListCreepTimingWay.Count;
	}

	private IEnumerator CreateCreepCor(CreepTimingWay creepTiming)
	{
		yield return new WaitForSeconds(creepTiming.Timing);

		var creepBeh = GameObject.Instantiate(creepTiming.Creep);
		creepBeh.CreepWayId = creepTiming.WayId;

		//currentLevelCreeps.Add(creepBeh.gameObject);
	}

	private class CreepTimingWay
	{
		public CreepsBehaviour Creep;
		public int WayId;
		public float Timing;
	}

	public void KilledCreep(CreepsBehaviour creepsBehaviour)
	{
		//currentLevelCreeps.Remove(creepsBehaviour.gameObject);

		creepsBehaviour.gameObject.SetActive(false);

		creepsBehaviour.transform.parent = FinishedCreepsListGameObject.transform;
	}

	private const string FinishedCreepsListName = "FinishedCreepsList";
	public GameObject FinishedCreepsListGameObject
	{
		get { return GameObject.Find(FinishedCreepsListName); }
	}

	//public void SetText(TextAsset creepsText)
	public void SetText(string creepsText)
	{
		LevelCreeps = creepsText;

		var lines = LevelCreeps.Replace("\r", "").Split(new char[] { '\n' });

		ListCreepTimingWay = new List<CreepTimingWay>();

		foreach (var line in lines)
		{
			var creep = CreateCripFromLine(line);

			if (creep != null)
				ListCreepTimingWay.Add(creep);
		}

		creepsWereSet = true;

		this.allCreepsCount = StartCreateCreeps();
	}
}