using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CreepWay : MonoBehaviour
{
	public int Id;

	public List<GameObject> WayPoints;

	private Vector2[] _points;

	public void InitPoints()
	{
		if (WayPoints == null || WayPoints.Count == 0)
		{
			WayPoints = new List<GameObject>();

			foreach (Transform ch in this.transform)
				WayPoints.Add(ch.gameObject);
		}

		if (WayPoints == null || WayPoints.Count == 0)
		{
			Debug.LogError("WayPoints == null || WayPoints.Count == 0");
			return;
		}

		_points = WayPoints.Select(x => new Vector2(x.transform.position.x, x.transform.position.y)).ToArray();
	}

	public Vector2[] GetVectorPoints()
	{
		return _points;
	}
}