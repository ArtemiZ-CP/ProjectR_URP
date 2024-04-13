using System;
using UnityEngine;

[Serializable]
public class PowerupInfo
{
	[SerializeField] private PowerupType _powerupType;
	[SerializeField] private Powerup _powerupPrefab;

	public PowerupType Type => _powerupType;
	public Powerup Prefab => _powerupPrefab;
}
