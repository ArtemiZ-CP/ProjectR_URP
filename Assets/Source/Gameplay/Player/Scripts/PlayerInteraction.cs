using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
	[SerializeField] private PlayerMovement _playerMovement;
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private PlayerEnergy _playerEnergy;
	[SerializeField] private PlayerStatusEffects _playerStatusEffects;

	private List<Obstacle> _collidedObstacles = new();

	private void OnTriggerEnter(Collider collider)
	{
		TryInteractWithObstacle(collider);
		TryInteractWithEnergyPower(collider);

		if (collider.TryGetComponent(out ObstacleCollider obstacleCollider))
		{
			_collidedObstacles.Add(obstacleCollider.Obstacle);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.TryGetComponent(out ObstacleCollider obstacleCollider))
		{
			if (_collidedObstacles.Contains(obstacleCollider.Obstacle))
			{
				_collidedObstacles.Remove(obstacleCollider.Obstacle);
			}
		}
	}

	private bool TryInteractWithEnergyPower(Collider collider)
	{
		if (collider.TryGetComponent(out EnergyPower energyPower))
		{
			_playerEnergy.IncreaseEnergy(energyPower.AdditionalEnergy);

			if (energyPower.IsDestroyable)
			{
				Destroy(energyPower.gameObject);
			}

			return true;
		}

		return false;
	}

	private bool TryInteractWithObstacle(Collider collider)
	{
		if (collider.TryGetComponent(out ObstacleCollider obstacleCollider))
		{
			Obstacle obstacle = obstacleCollider.Obstacle;

			if (_collidedObstacles.Contains(obstacle))
			{
				return false;
			}

			_playerHealth.TakeDamage(obstacle.Damage);

			if (obstacle.CanDisplace)
			{
				OffsetPlayer(obstacle.Offset, obstacle);
			}

			if (obstacle.IsDestroyable)
			{
				DestroyObstacle(obstacle);
			}

			if (obstacle.StatusEffect != StatusEffect.None)
			{
				_playerStatusEffects.AddNewStatusEffect(obstacle.StatusEffect, obstacle.StatusEffectDuration);
			}

			return true;
		}

		if (collider.TryGetComponent(out BossObstacleCollider bossObstacleCollider))
		{
			BossProjectile obstacle = bossObstacleCollider.BaseAttackPrefab;

			_playerHealth.TakeDamage(obstacle.Damage);

			Destroy(obstacle.gameObject);

			return true;
		}

		return false;
	}

	private void DestroyObstacle(Obstacle obstacle)
	{
		obstacle.Disactive();
	}

	private void OffsetPlayer(int offset, Obstacle obstacle)
	{
		if (_playerMovement.TryBacktrack(obstacle) == false)
		{
			_playerMovement.ForceSwitchRoad(offset);
		}
	}
}
