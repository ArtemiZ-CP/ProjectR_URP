using UnityEngine;

public class BossObstacleCollider : MonoBehaviour
{
	[SerializeField] private BossProjectile _baseAttackPrefab;

	public BossProjectile BaseAttackPrefab => _baseAttackPrefab;
}
