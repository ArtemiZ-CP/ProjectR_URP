using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] private int _damage = 1;
	[SerializeField] private float _damageDelay = 1;
	[SerializeField] private Projectile _projectile;

	public void Attack()
	{
		if (GameManager.Instance.CurrentBoss != null)
		{
			StartCoroutine(ThrowProjectile());
		}
	}

	private void DamageBoss(Boss boss)
	{
		if (boss != null)
		{
			boss.TakeDamage(_damage);
		}
	}

	private void OnProjectileHit(Projectile projectile, Collider collider)
	{
		if (collider.TryGetComponent(out Boss boss))
		{
			DamageBoss(boss);
			Destroy(projectile.gameObject);
		}
	}

	private IEnumerator ThrowProjectile()
	{
		Transform bossTransform = GameManager.Instance.CurrentBoss.transform;
		Projectile projectile = Instantiate(_projectile, transform.position, Quaternion.identity, bossTransform);
		Vector3 direction = bossTransform.position - transform.position;
		projectile.Init((direction.magnitude) / _damageDelay, direction);
		projectile.OnHit += OnProjectileHit;

		yield return new WaitForSeconds(_damageDelay);

		if (projectile != null)
		{
			DamageBoss(GameManager.Instance.CurrentBoss);
			Destroy(projectile.gameObject);
		}
	}
}
