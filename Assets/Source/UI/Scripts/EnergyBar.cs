using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
	[SerializeField] private PlayerEnergy _playerEnergy;
	[SerializeField] private Slider _slider;

	private void OnEnable()
	{
		_playerEnergy.OnEnergyChanged += UpdateHealthBar;
	}

	private void OnDisable()
	{
		_playerEnergy.OnEnergyChanged -= UpdateHealthBar;
	}

	private void UpdateHealthBar()
	{
		_slider.value = (float)_playerEnergy.CurrentEnergy / _playerEnergy.MaxEnergy;
	}
}
