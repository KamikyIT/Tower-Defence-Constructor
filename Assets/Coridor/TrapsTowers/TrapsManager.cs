using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapsManager: MonoBehaviour
{
	public TextAsset TextAsset;

	[SerializeField]
	private List<AbstractTrapBehaviour> AllTraps;

	[SerializeField]
	private List<AbilityEffectForCreep> AllAbilityEffects;

	public AbstractTrapBehaviour GetTrapOfType(TrapType trapType)
	{
		var selectedTrapGo = AllTraps.FirstOrDefault(x => x.TrapType == trapType);

		selectedTrapGo.Model = TrapModel.GetTrapModelInfo(trapType);

		return selectedTrapGo;
	}

	public AbilityEffectForCreep GetAbilityEffectForCreep(TrapAbilityType abilityType)
	{
		var abilityEffectGo = AllAbilityEffects.FirstOrDefault(x => x.AbilityType == abilityType);

		return abilityEffectGo;
	}

	public void SetText(string trapsInfoText)
	{
		TrapModel.SetTrapModelsFromTextFile(trapsInfoText);
	}
}