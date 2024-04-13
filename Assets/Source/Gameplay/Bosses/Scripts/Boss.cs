using UnityEngine;

public class Boss : MonoBehaviour
{
	[SerializeField] private LevelGenerator _levelGenerator;
	[SerializeField] private BossAnimation _bossAnimation;
	[SerializeField] private int _maxHealth = 10;
	[SerializeField] private int _damage = 1;
	[Header("BaseAttack")]
	[SerializeField] private Transform _attackTransform;
	[SerializeField] private Transform _attackProjectilesParent;
	[SerializeField] private float _baseAttackDuration;
	[SerializeField] private float _baseAttackTimeOffset;
	[SerializeField] private float _baseAttackCooldown;
	[SerializeField] private AnimationCurve _attackMotionCurve;
	[SerializeField] private float _gizmosOffset = 1;
	[Header("BigAttack")]
	[SerializeField] private int _cellsToFreezeCount;
	[SerializeField] private float _freezeDuration = 1;
	[SerializeField] private float _freezeAttackTimeOffset;
	[SerializeField] private float _freezeAttackCooldown = 1;
	[Header("Area")]
	[SerializeField] private Vector3 _attackAreaCenter;
	[SerializeField] private float _attackAreaLength;

	private int _currentHealth;
	private float _currentAttackCooldown;
	private ObstacleInfo _targetObstacleInfo; 

	private float MinZ => _attackAreaCenter.z + transform.position.z - _attackAreaLength / 2;
	private float MaxZ => _attackAreaCenter.z + transform.position.z + _attackAreaLength / 2;

	public static Vector3 GetObstaclePosition(AnimationCurve curve, Vector3 startPosition, Vector3 endPosition, float t)
	{
		float y = curve.Evaluate(t) * (startPosition.y - endPosition.y);
		Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);
		newPosition.y = y;
		return newPosition;
	}

	public void TakeDamage(int damage)
	{
		_currentHealth -= damage;

		if (_currentHealth <= 0)
		{
			_currentHealth = 0;
			Death();
		}
	}

	private void Awake()
	{
		_currentHealth = _maxHealth;
	}

	private void Update()
	{
		if (_currentAttackCooldown > 0)
		{
			_currentAttackCooldown -= Time.deltaTime;
		}
		else
		{
			switch (Random.Range(0, 5))
			{
				case 0:
					TryToFreezeAttack();
					_currentAttackCooldown = _freezeAttackCooldown;
					break;
				default:
					TryToBaseAttack();
					_currentAttackCooldown = _baseAttackCooldown;
					break;
			}
		}
	}

	private bool TryToBaseAttack()
	{
		_targetObstacleInfo = _levelGenerator.GetRandomCell(MinZ, MaxZ, CellType.Empty);

		if (_targetObstacleInfo == null)
		{
			return false;
		}

		_bossAnimation.BaseAttack();

		Invoke(nameof(SpawnBaseAttack), _baseAttackTimeOffset);

		return true;
	}

	private void SpawnBaseAttack()
	{
		_targetObstacleInfo.CellType = CellType.BossObstacle;
		BossProjectile baseAttackPrefab = Instantiate(CurrentChunkSettings.Settings.BossProjectilePrefab, _attackTransform.position, Quaternion.identity, _attackProjectilesParent);
		baseAttackPrefab.Init(_attackMotionCurve, _attackTransform.position, _targetObstacleInfo.GetCellPosition(), _baseAttackDuration, _damage);
	}

	private bool TryToFreezeAttack()
	{
		_bossAnimation.BigAttack();

		Invoke(nameof(SpawnFreezeAttack), _freezeAttackTimeOffset);

		return true;
	}

	private void SpawnFreezeAttack()
	{
		ObstacleInfo obstacleInfo;

		for (int i = 0; i < _cellsToFreezeCount; i++)
		{
			obstacleInfo = _levelGenerator.GetRandomCell(MinZ, MaxZ, CellType.Road);

			if (obstacleInfo == null)
			{
				break;
			}

			obstacleInfo.Damage = 0;
			obstacleInfo.StatusEffect = StatusEffect.Freeze;
			obstacleInfo.StatusEffectDuration = _freezeDuration;
			obstacleInfo.CellType = CellType.FreezedRoad;
			obstacleInfo.ReloadCell();
		}
	}

	private void Death()
	{
		Destroy(gameObject);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_attackAreaCenter + transform.position, new Vector3(CurrentChunkSettings.Settings.RoadOffset * 3, 1, _attackAreaLength));

		if (_gizmosOffset > 0)
		{
			Gizmos.color = Color.green;
			Vector3 startPosition = _attackTransform.position;
			Vector3 endPosition = _attackAreaCenter + transform.position;

			for (float i = 0; i < 1; i += _gizmosOffset)
			{
				Gizmos.DrawLine(GetObstaclePosition(_attackMotionCurve, startPosition, endPosition, i), GetObstaclePosition(_attackMotionCurve, startPosition, endPosition, i + _gizmosOffset));
			}
		}
	}
}
