using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaysManager: MonoBehaviour
{
	public TextAsset TextAsset;

	public GameObject Brains;

	public GameObject WayPointsParent;

	public GameObject TrapPlacesParent;


	//void Awake()
	//{
	//	var folder = Directory.GetParent(Application.dataPath).Name;

	//	var filename = Path.Combine(folder, SceneManager.GetActiveScene().name + " creeps") + ".txt";

	//	if (TextAsset != null)
	//	{
	//		var lines = TextAsset.text.Split('\n');

	//		ParseLines(lines);
	//	}
	//	else if (File.Exists(filename))
	//	{
	//		var lines = File.ReadAllLines(filename);

	//		ParseLines(lines);
	//	}




	//}

	//private void ParseLines(string[] lines)
	//{
	//	if (lines == null || lines.Length == 0)
	//	{
	//		Debug.LogError("ParseLines : lines == null || lines.Length == 0");
	//		return;
	//	}

	//	var wayParents = new List<CreepWay>();

	//	var array = new List<List<string>>();

	//	foreach (var lineSource in lines)
	//	{
	//		if (string.IsNullOrEmpty(lineSource))
	//		{
	//			continue;
	//		}
	//		var line = lineSource.Replace("\r", "");
	//		var parts = line.Split('\t');

	//		array.Add(new List<string>());

	//		array.Last().AddRange(parts.Select(x => ParsePart(x)));
	//	}




	//}

	//private string ParsePart(string part)
	//{
	//	return null;
	//}
}