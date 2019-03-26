using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitGameManager : MonoBehaviour
{
	public TextAsset CreepsText;
	public TextAsset LevelPointsText;
	public TextAsset TrapsInfoText;
	public Camera MainCamera;

	public BrainsColorAnim BrainsGameObject;

	public CreepsLevelManager CreepsManager;

	public TrapsManager TrapsManager;

	public TrapPlace TrapPlace;

	private GameObject CreepWaysParent;
	private string creepsFile;
	private string wayPointsFile;
	private string towersFile;


	// Use this for initialization
	void Awake ()
	{
		//this.DebugText.gameObject.transform.parent.gameObject.SetActive(false);

		FilesLoad();

		var wayPointsText = File.ReadAllText(wayPointsFile).Replace("\r", "");
		MainCamera.orthographicSize = GetCameraSizeFromText(wayPointsText);

		// DebugText.text += "\r\nCameraSize";

		var cellInfos = ParseCellInfos(wayPointsText);

		BrainsGameObject.transform.position = GetBrainsPosition(cellInfos);

		// DebugText.text += "\r\nBrains";

		var sb = new StringBuilder();

		cellInfos.ForEach(x => sb.Append(x.ToString()+ "\n"));

		SetWayPoints(cellInfos);

		// DebugText.text += "\r\nWayPoints";

		SetTrapPlaces(cellInfos);

		// DebugText.text += "\r\nTrapPlaces";

		var creepsFileText = File.ReadAllText(creepsFile);
		CreepsManager.SetText(creepsFileText);

		// DebugText.text += "\r\nCreeps";

		var trapInfosFileText = File.ReadAllText(towersFile);
		TrapsManager.SetText(trapInfosFileText);
		// DebugText.text += "\r\nTrapsManager";
	}

	private void FilesLoad()
	{
		var sb = new StringBuilder();
		sb.Append("Application.persistentDataPath = " + Application.persistentDataPath);
		sb.Append("\r\n");
		

		var curScene = SceneManager.GetActiveScene().name;
		creepsFile = Application.persistentDataPath + "/" + curScene + "_Creeps.txt";
		wayPointsFile = Application.persistentDataPath + "/" + curScene + "_WayPoints_TrapPlaces.txt";
		towersFile = Application.persistentDataPath + "/" + curScene + "_towers.txt";


		//public TextAsset CreepsText;
		//public TextAsset LevelPointsText;
		//public TextAsset TrapsInfoText;

		if (!File.Exists(creepsFile))
		{
			sb.Append("Creating " + creepsFile);
			sb.Append("\r\n");

			File.Create(creepsFile).Close();

			File.WriteAllText(creepsFile, CreepsText.text);
		}

		if (!File.Exists(wayPointsFile))
		{
			sb.Append("Creating " + wayPointsFile);
			sb.Append("\r\n");

			File.Create(wayPointsFile).Close();

			File.WriteAllText(wayPointsFile, LevelPointsText.text);
		}

		if (!File.Exists(towersFile))
		{
			sb.Append("Creating " + towersFile);
			sb.Append("\r\n");


			File.Create(towersFile).Close();

			File.WriteAllText(towersFile, TrapsInfoText.text);
		}

		// DebugText.text = sb.ToString();
	}

	private void SetTrapPlaces(List<CellInfo> cellInfos)
	{
		var trapsParent = new GameObject("TrapPlaces");

		var traps = cellInfos.Where(x => x.CellInfoString == "TP").ToList();

		foreach (var trap in traps)
		{
			var trapGo = GameObject.Instantiate(TrapPlace, new Vector3(trap.X, trap.Y, 0), Quaternion.identity);

			trapGo.name = "TP";

			trapGo.transform.parent = trapsParent.transform;
		}

	}

	private void SetWayPoints(List<CellInfo> cellInfos)
	{
		CreepWaysParent = new GameObject("WaysParent");

		var wayPoints = cellInfos.Where(x => x.CellInfoString.StartsWith("W")).Select(x => new
		{
			cell = x,
			wayId = int.Parse(x.CellInfoString[1].ToString()),
			wayPointId = int.Parse(x.CellInfoString.Substring(2).ToString()),
		}).ToList();

		var allWayIds = wayPoints.Select(x => x.wayId).Distinct();

		foreach (var allWayId in allWayIds)
		{
			//var creepWay = GameObject.Instantiate(WayPathPrefab, new Vector3(), Quaternion.identity).GetComponent<CreepWay>();
			var creepWayGo = new GameObject("Way " + allWayId);
			var creepWay = creepWayGo.AddComponent<CreepWay>();

			creepWayGo.transform.parent = CreepWaysParent.transform;

			creepWay.Id = allWayId;

			creepWay.WayPoints = new List<GameObject>();

			var thisWayIdPoints = wayPoints.Where(x => x.wayId == allWayId).ToList();

			thisWayIdPoints = thisWayIdPoints.OrderBy(x => x.wayPointId).ToList();

			foreach (var thisWayIdPoint in thisWayIdPoints)
			{
				var goname = string.Format("WP({0})", thisWayIdPoint.wayPointId);

				var go = new GameObject(goname);
				go.transform.position = new Vector3(thisWayIdPoint.cell.X, thisWayIdPoint.cell.Y);

				go.transform.parent = creepWay.gameObject.transform;

				creepWay.WayPoints.Add(go);
			}

			creepWay.InitPoints();
		}
	}

	private Vector3 GetBrainsPosition(List<CellInfo> cellInfos)
	{
		var brainsCell = cellInfos.FirstOrDefault(x => x.CellInfoString == "B");

		if (brainsCell == null)
		{
			Debug.LogError("Не найдена ячейка B");
			return new Vector3();
		}

		return new Vector3(brainsCell.X, brainsCell.Y);
	}

	private List<CellInfo> ParseCellInfos(string text)
	{
		var lines = text.Split('\n');

		var started = 0;
		var finished = 0;

		var xCoordsLine = lines.First();
		var xCoords = new List<KeyValuePair<int, float>>();
		var yCoords = new List<KeyValuePair<int, float>>();

		foreach (var line in lines)
		{
			var parts = line.Split('\t');

			var num = 0;
			if (int.TryParse(parts[0], out num))
			{
				if (started == 0)
				{
					started = num;
				}
				else
					finished = num;
			}
		}

		if (xCoordsLine != "")
		{
			var parts = xCoordsLine.Split('\t');

			var index = 0;

			foreach (var part in parts)
			{
				if (part.IsNullOrWhiteSpace() || string.IsNullOrEmpty(part))
				{
					index++;
					continue;
				}

				xCoords.Add(new KeyValuePair<int, float>(index++, float.Parse(part)));
			}
		}

		var indexY = 0;
		foreach (var line in lines)
		{
			var parts = line.Split('\t');

			if (line == xCoordsLine)
			{
				indexY++;
				continue;
			}

			yCoords.Add(new KeyValuePair<int, float>(indexY++, float.Parse(parts[0]) * 0.1f));
		}


		var cellInfos = new List<CellInfo>();

		for (int i = 0; i < lines.Length; i++)
		{
			var line = lines[i];

			if (line == xCoordsLine)
				continue;

			var parts = line.Split('\t');
			for (int j = 0; j < parts.Length; j++)
			{
				var cell = parts[j];

				var cellInfosWithIJ = ParseCell(cell, j, i);

				if (cellInfosWithIJ == null)
					continue;

				foreach (var kvPair in cellInfosWithIJ)
				{
					var cellInfo = kvPair.Key;

					cellInfo.X = xCoords.FirstOrDefault(x => x.Key == kvPair.Value.Key).Value;
					
					cellInfo.Y = yCoords.FirstOrDefault(x => x.Key == kvPair.Value.Value).Value;

					cellInfos.Add(cellInfo);
				}
			}
		}


		return cellInfos;
	}

	private List<KeyValuePair<CellInfo, KeyValuePair<int, int>>> ParseCell(string cell, int i, int j)
	{
		var parts = cell.Replace("{", "").Replace("}", "").Split(',');

		if (parts.Length == 1)
		{
			var part = parts[0];

			if (CellInfo.IgnoreCellString(part))
				return null;

			var cellInfo = new CellInfo(part);

			return new List<KeyValuePair<CellInfo, KeyValuePair<int, int>>>()
			{
				new KeyValuePair<CellInfo, KeyValuePair<int, int>>(cellInfo, new KeyValuePair<int, int>(i, j))
			};
		}

		var result = new List<KeyValuePair<CellInfo, KeyValuePair<int, int>>>();

		foreach (var part in parts)
		{
			if (part == "" || part == "*")
				continue;

			var cellInfo = new CellInfo(part);

			result.Add(new KeyValuePair<CellInfo, KeyValuePair<int, int>>(cellInfo, new KeyValuePair<int, int>(i, j)));
		}

		return result;
	}

	private float GetCameraSizeFromText(string levelPointsText)
	{
		var text = levelPointsText;

		var started = 0;
		var finished = 0;
		foreach (var line in text.Split('\n'))
		{
			var parts = line.Split('\t');

			var num = 0;
			if (int.TryParse(parts[0], out num))
			{
				if (started == 0)
					started = num;
				else
					finished = num;
			}
		}

		var dif = (started - finished) * 0.1f + 1f;
		
		return dif / 2f;
	}


	private class CellInfo
	{
		public float X;
		public float Y;

		public string CellInfoString;

		public CellInfo(string cell)
		{
			CellInfoString = cell;
		}

		public static bool IgnoreCellString(string part)
		{
			float num;
			if (string.IsNullOrEmpty(part) || part == "*" || float.TryParse(part, out num))
				return true;

			return false;
		}

		public override string ToString()
		{
			return CellInfoString + " " + X + " " + Y;
		}
	}

	public void CloseDebugPanel()
	{
		// this.DebugText.gameObject.transform.parent.gameObject.SetActive(false);
	}
}
