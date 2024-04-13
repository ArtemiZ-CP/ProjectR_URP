using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private Slider _slider;

	private void OnEnable()
	{
		_playerHealth.OnHealthChanged += UpdateHealthBar;
	}

	private void OnDisable()
	{
		_playerHealth.OnHealthChanged -= UpdateHealthBar;
	}

	private void UpdateHealthBar()
	{
		_slider.value = (float)_playerHealth.CurrentHealth / _playerHealth.MaxHealth;
	}
}
