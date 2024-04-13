using System;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
public class PlayerEnergy : MonoBehaviour
{
	[SerializeField] private int _maxEnergy;

	public event Action OnEnergyChanged;

	private PlayerAttack _playerAttack;
	private int _currentEnergy = 0;

	public int MaxEnergy => _maxEnergy;
	public int CurrentEnergy => _currentEnergy;

	public void IncreaseEnergy(int value)
	{
		_currentEnergy += value;

		if (_currentEnergy >= _maxEnergy)
		{
			_currentEnergy = _maxEnergy;

			SpendEnergy();
		}

		OnEnergyChanged?.Invoke();
	}

	private void Awake()
	{
		_playerAttack = GetComponent<PlayerAttack>();
	}

	private void Start()
	{
		_currentEnergy = 0;

		OnEnergyChanged?.Invoke();
	}

	private void SpendEnergy()
	{
		_currentEnergy = 0;

		_playerAttack.Attack();
		
		OnEnergyChanged?.Invoke();
	}
}
