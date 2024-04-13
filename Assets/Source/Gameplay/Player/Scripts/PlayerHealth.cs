using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
	[SerializeField] private Reload _reload;

	public event Action OnHealthChanged;

    private int _currentHealth;

	public int MaxHealth => _maxHealth;
	public int CurrentHealth => _currentHealth;

	public void TakeDamage(int damage)
	{
		_currentHealth -= damage;

		if (_currentHealth <= 0)
		{
			_currentHealth = 0;
			Die();
		}

		OnHealthChanged?.Invoke();
	}

	private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Die()
    {
		_reload.RaloadLevel();
    }
}
