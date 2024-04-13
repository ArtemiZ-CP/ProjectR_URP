using UnityEngine;

public class EnergyPower : Powerup
{
	private BasePowerup _basePowerup;

	public int AdditionalEnergy => _basePowerup.AdditionalEnergy;
	public bool IsDestroyable => _basePowerup.IsDestroyable;
	public int RoadIndex => _basePowerup.RoadIndex;

	private void Awake()
	{
		_basePowerup = transform.GetComponentInParent<BasePowerup>();
	}
}
