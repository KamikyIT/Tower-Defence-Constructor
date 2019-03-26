using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AbstractTrapBehaviour : MonoBehaviour
{
	public TrapBulletBehaviour TrapBulletPrefab;

	public TrapType TrapType;

	public TrapModel Model;
	public TrapPlace TrapPlace;

	public bool AttackCooldown;

	public bool AbilityCooldown;

	public float Range;

	public void SetTrapPlace(TrapPlace trapPlace)
	{
		this.transform.parent = trapPlace.transform.transform;

		transform.localPosition = new Vector3();

		this.TrapPlace = trapPlace;

		trapPlace.CurrentTrap = this;
	}

	public bool IsMaxLevelUpgrade()
	{
		return Model.IsMaxLevelUpgrade();
	}

	public void UpgadeTrap()
	{
		Model.Upgrade();
	}

	public void Start()
	{
		var circleCollider = GetComponent<CircleCollider2D>();
		Range = circleCollider.radius;

		Destroy(circleCollider);
	}

	public void Update()
	{
		AttackBehaviour();
	}

	private void AttackBehaviour()
	{
		if (AttackCooldown)
			return;

		var targets = FindTargets();

		if (targets != null)
			Fire(targets);
	}

	private List<CreepsBehaviour> FindTargets()
	{
		var allCreeps = GameObject.FindObjectsOfType<CreepsBehaviour>();

		if (allCreeps == null || (allCreeps.Any(x => x.IsAlive()) == false))
			return null;

		var creepsDests = allCreeps.Where(x => x.IsAlive()).
			Select(x => new {creep = x, dest = Vector3.Magnitude(x.transform.position - this.transform.position)}).Where(x => x.dest < Range);

		return creepsDests.Select(x => x.creep).ToList();
	}

	public virtual void Fire(List<CreepsBehaviour> target)
	{
		AttackCooldown = true;

		StartCoroutine(ResetAttackCor());
	}

	private IEnumerator ResetAttackCor()
	{
		yield return new WaitForSeconds(Model.AttackSpeed);

		AttackCooldown = false;
	}

	public void UseAbility()
	{
		if (CanUseAbility() == false) return;

		AbilityCooldown = true;

		CreateAbilityEffect();

		StartCoroutine(ResetAbilityCor());
	}

	private void CreateAbilityEffect()
	{
		var allCreeps = GameObject.FindObjectsOfType<CreepsBehaviour>();

		if (allCreeps == null || allCreeps.All(x => x.gameObject.activeSelf == false))
			return;

		var aliveCreeps = allCreeps.Where(x => x.enabled && x.IsAlive() &&
		                                       (x.State == CreepsBehaviourState.MovingForward ||
		                                        x.State == CreepsBehaviourState.MovingBack)
		);

		switch (Model.AbilityType)
		{
			case TrapAbilityType.None:
				break;
			case TrapAbilityType.Frost:
				foreach (var creep in aliveCreeps)
					creep.FrostAbility(this);
				break;
			case TrapAbilityType.Envenom:
				foreach (var creep in aliveCreeps)
					creep.EnvenomAbility(this);
				break;
			case TrapAbilityType.Steal:
				foreach (var creep in aliveCreeps)
					creep.StealAbility(this);
				break;
			case TrapAbilityType.Ghost:
				foreach (var creep in aliveCreeps)
					creep.GhostAbility(this);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

	}

	private IEnumerator ResetAbilityCor()
	{
		yield return new WaitForSeconds(Model.AbilityCooldown);

		AbilityCooldown = false;
	}

	public bool CanUseAbility()
	{
		return Model.AbilityType != TrapAbilityType.None && !AbilityCooldown;
	}
}

[Serializable]
public class TrapModel
{
	public int MinDamage = 1;
	public int MaxDamage = 5;


	public float AttackSpeed = 1f;

	public TrapAttackType AttackType;

	public TrapAbilityType AbilityType;
	public int AbilityCooldown;

	public List<CreepBuff> ApplyingBuffs;

	public TrapModel NextUpgradeTrap;

	public string NextUpgradeTrapName;

	public int MoneyCost;

	public string TrapName;

	public bool IsMaxLevelUpgrade()
	{
		return this.NextUpgradeTrap == null;
	}

	private static List<TrapModel> AllTraps;

	static TrapModel()
	{
		return;

		var allTrapsInfo =
			"Fear|1|3|1|Fear2|50|Fear|\n" +
			"Void|1|3|1|Void2|50|Chaos|\n" +
			"Oblivion|1|3|1|Oblivion2|50|Poison|\n" +
			"Envy|1|3|1|Envy2|50|Fear|\n" +
			"Madness|1|3|1|Madness2|50|Light|\n" +
			"Hypocrisy|1|3|1|Hypocrisy2|50|Chaos|\n" +
			"Lust|1|3|1|Lust2|50|Light|\n" +
			"Pride|1|3|1|Pride2|50|Poison|\n" +
			"Fear2|1|3|1|Fear3|50|Fear|Ghost,60\n" +
			"Void2|1|3|1|Void3|50|Chaos|Ghost,60\n" +
			"Oblivion2|1|3|1|Oblivion3|50|Poison|Ghost,60\n" +
			"Envy2|1|3|1|Envy3|50|Fear|\n" +
			"Madness2|1|3|1|Madness3|50|Light|\n" +
			"Hypocrisy2|1|3|1|Hypocrisy3|50|Chaos|Steal,45\n" +
			"Lust2|1|3|1|Lust3|50|Light|Frost,60\n" +
			"Pride2|1|3|1|Pride3|50|Poison|Envenom,60\n" +
			"Fear3|1|3|1|Fear4|50|Fear|Ghost,30\n" +
			"Void3|1|3|1|Void4|50|Chaos|Ghost,30\n" +
			"Oblivion3|1|3|1|Oblivion4|50|Poison|Ghost,30\n" +
			"Envy3|1|3|1|Envy4|50|Fear|\n" +
			"Madness3|1|3|1|Madness4|50|Light|\n" +
			"Hypocrisy3|1|3|1|Hypocrisy4|50|Chaos|Steal,30\n" +
			"Lust3|1|3|1|Lust4|50|Light|Frost,30\n" +
			"Pride3|1|3|1|Pride4|50|Poison|Envenom,40\n" +
			"Fear4|1|3|1||50|Fear|Ghost,15\n" +
			"Void4|1|3|1||50|Chaos|Ghost,15\n" +
			"Oblivion4|1|3|1||50|Poison|Ghost,15\n" +
			"Envy4|1|3|1||50|Fear|\n" +
			"Madness4|1|3|1||50|Light|\n" +
			"Hypocrisy4|1|3|1||50|Chaos|Steal,15\n" +
			"Lust4|1|3|1||50|Light|Frost,15\n" +
			"Pride4|1|3|1||50|Poison|Envenom,20\n";

		AllTraps = new List<TrapModel>();

		// TrapName | MinDamage | MaxDamage | AttackSpeed | NextUpgradeTrapName | MoneyCost | AttackType | AbilityType
		foreach (var line in allTrapsInfo.Split('\n'))
		{
			if (string.IsNullOrEmpty(line)) continue;

			var part = line.Split('|');

			if (part.Length < 8)
			{
				Debug.LogError("part.Length < 5 : " + part);
				continue;
			}

			var trap = new TrapModel();

			var i = 0;

			trap.TrapName = part[i++];

			trap.MinDamage = int.Parse(part[i++]);
			trap.MaxDamage = int.Parse(part[i++]);

			trap.AttackSpeed = float.Parse(part[i++]);

			trap.NextUpgradeTrapName = part[i++];

			trap.MoneyCost = int.Parse(part[i++]);

			trap.AttackType = ParseAttackType(part[i++]);

			if (string.IsNullOrEmpty(part[i]))
			{
				trap.AbilityType = TrapAbilityType.None;
			}
			else
			{
				var abilityInfo = part[i].Split(',');

				var abilityType = ParseAbilityType(abilityInfo[0]);

				var abilityTime = int.Parse(abilityInfo[1]);

				trap.AbilityType = abilityType;

				trap.AbilityCooldown = abilityTime;
			}

			AllTraps.Add(trap);
		}

		// Задаем апгрейдычи из имен.

		foreach (var trap in AllTraps)
		{
			if (string.IsNullOrEmpty(trap.NextUpgradeTrapName)) continue;
			var nextTrap = AllTraps.FirstOrDefault(x => x.TrapName == trap.NextUpgradeTrapName);

			if (nextTrap == null)
			{
				Debug.LogError("nextTrap == null : trap.Upgrade = " + trap.NextUpgradeTrapName);
				continue;
			}

			trap.NextUpgradeTrap = nextTrap;
		}
	}

	private static TrapAbilityType ParseAbilityType(string str)
	{
		if (string.IsNullOrEmpty(str))
			return TrapAbilityType.None;

		str = str.Split(',')[0];

		return (TrapAbilityType)Enum.Parse(typeof(TrapAbilityType), str, true);
	}

	private static TrapAttackType ParseAttackType(string str)
	{
		// Fear
		// Chaos
		// Light
		// Poison
		return (TrapAttackType) Enum.Parse(typeof(TrapAttackType), str, true);
	}

	public void Upgrade()
	{
		CopyTrapModel(this.NextUpgradeTrap, this);
	}

	public static void CopyTrapModel(TrapModel source, TrapModel dest)
	{
		dest.MinDamage = source.MinDamage;
		dest.MaxDamage = source.MaxDamage;
		dest.TrapName = source.TrapName;
		dest.NextUpgradeTrapName = source.NextUpgradeTrapName;
		dest.AttackSpeed = source.AttackSpeed;
		dest.MoneyCost = source.MoneyCost;
		dest.NextUpgradeTrap = source.NextUpgradeTrap;

		dest.AttackType = source.AttackType;
		dest.AbilityType = source.AbilityType;
		dest.AbilityCooldown = source.AbilityCooldown;
	}


	public static TrapModel GetTrapModelInfo(TrapType trapType)
	{
		var staticTrap = AllTraps.FirstOrDefault(x => x.TrapName.ToLower() == trapType.ToString().ToLower());

		if (staticTrap == null)
			Debug.LogError("staticTrap == null : " + trapType.ToString());

		var newTrap = new TrapModel();
		CopyTrapModel(staticTrap, newTrap);

		return newTrap;
	}

	public static void SetTrapModelsFromTextFile(string text)
	{
		var lines = text.Split('\n');

		AllTraps = new List<TrapModel>();

		foreach (var line in lines)
		{
			var model = ReadTrapModelFromLine(line);

			if (model != null)
				AllTraps.Add(model);
		}

		// Задаем апгрейдычи из имен.

		foreach (var trap in AllTraps)
		{
			if (string.IsNullOrEmpty(trap.NextUpgradeTrapName)) continue;
			var nextTrap = AllTraps.FirstOrDefault(x => x.TrapName == trap.NextUpgradeTrapName);

			if (nextTrap == null)
			{
				Debug.LogError("nextTrap == null : trap.Upgrade = " + trap.NextUpgradeTrapName);
				continue;
			}

			trap.NextUpgradeTrap = nextTrap;
		}
	}

	// TrapName | MinDamage | MaxDamage | AttackSpeed | NextUpgradeTrapName | MoneyCost | AttackType | AbilityType
	private static TrapModel ReadTrapModelFromLine(string line)
	{
		try
		{
			if (string.IsNullOrEmpty(line) || line.IsNullOrWhiteSpace())
				return null;

			if (line.StartsWith("//"))
				return null;

			line = line.Replace("\r", "");

			while (line.Contains("\t\t"))
				line = line.Replace("\t\t", "\t");

			var parts = line.Split(new char[] {'|', '\t'});

			var i = 0;

			var prefabName = parts[i++];

			var minDamage = int.Parse(parts[i++]);

			var maxDamage = int.Parse(parts[i++]);

			var attackSpeed = float.Parse(parts[i++]);

			var nextUpgradeName = parts[i++] == "None" ? "" : parts[i-1];

			var moneyCost = int.Parse(parts[i++]);

			var attackType = ParseAttackType(parts[i++]);

			KeyValuePair<TrapAbilityType, int>? abilityCooldown = null;

			//var sb = new StringBuilder();

			//for (int j = 0; j < parts.Length; j++)
			//{
			//	sb.Append("[" + j + "]");
			//	sb.Append(parts[j]);
			//	sb.Append("   ");
			//}

			//Debug.Log(sb);

			if (parts.Length >= 8)
				abilityCooldown = ParseAbilityTypeWithCooldown(parts[i++]);

			var trap = new TrapModel()
			{
				MinDamage = minDamage,
				MaxDamage = maxDamage,
				AttackSpeed = attackSpeed,
				NextUpgradeTrapName = nextUpgradeName,
				MoneyCost = moneyCost,
				AttackType = attackType,
				TrapName = prefabName,
			};

			if (abilityCooldown.HasValue)
			{
				trap.AbilityType = abilityCooldown.Value.Key;
				trap.AbilityCooldown = abilityCooldown.Value.Value;
			}

			return trap;

		}
		catch (Exception e)
		{
			Debug.Log(e.Message.ToString() + " line = " + line);

			return null;
		}
	}

	private static KeyValuePair<TrapAbilityType, int>? ParseAbilityTypeWithCooldown(string str)
	{
		var abilityType = ParseAbilityType(str);

		if (abilityType == TrapAbilityType.None)
			return null;

		var cooldown = int.Parse(str.Split(',')[1]);
		return new KeyValuePair<TrapAbilityType, int>(abilityType, cooldown);
	}
}

public enum TrapAttackType
{
	Fear,
	Light,
	Chaos,
	Poison,
}

public enum TrapAbilityType
{
	None,
	Frost,
	Envenom,
	Steal,
	Ghost,
}


public static class OmfgShitDotNet
{
	public static bool IsNullOrWhiteSpace(this string value)
	{
		return value == null || value.Trim() == "";
	}
}