using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
	private readonly List<StatusEffect> _statusEffects = new();

	public void AddNewStatusEffect(StatusEffect statusEffect, float duration)
	{
		StartCoroutine(AddStatusEffect(statusEffect, duration));
	}

	public bool HasStatusEffect(StatusEffect statusEffect)
	{
		return _statusEffects.Contains(statusEffect);
	}

	private void RemoveStatusEffect(StatusEffect statusEffect)
	{
		if (_statusEffects.Contains(statusEffect))
		{
			_statusEffects.Remove(statusEffect);
		}
	}

	private IEnumerator AddStatusEffect(StatusEffect statusEffect, float duration)
	{
		_statusEffects.Add(statusEffect);

		yield return new WaitForSeconds(duration);

		RemoveStatusEffect(statusEffect);
	}
}
