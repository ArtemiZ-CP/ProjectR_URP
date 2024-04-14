using System.Collections.Generic;
using UnityEngine;

public class BaseObstacle : BaseObject
{
	[HideInInspector] public int OffsetAwake;
	[SerializeField] private List<CellInfo> _cellInfos = new();

	[SerializeField] private int _damage = 1;
	[SerializeField] private CellType _cellType;
	[SerializeField] private StatusEffect _statusEffect;
	[SerializeField] private float _statusEffectDuration;
	[Header("Options")]
	[SerializeField] private bool _isDestroyable = false;
	[SerializeField] private bool _canDisplace = true;

	private CellType _lastCellType;
	private int _offset;

	public int Damage => _damage;
	public bool IsDestroyable => _isDestroyable;
	public bool CanDisplace => _canDisplace;
	public int Offset => _offset;
	public CellType CellType => _cellType;
	public StatusEffect StatusEffect => _statusEffect;
	public float StatusEffectDuration => _statusEffectDuration;

	public override int Init()
	{
		return GetLength(_cellType);
	}

	public int Init(ObstacleInfo obstacleInfo)
	{
		SnapObjectToGrid.SetLine(obstacleInfo.Line);
		_damage = obstacleInfo.Damage;
		_isDestroyable = obstacleInfo.IsDestroyable;
		_canDisplace = obstacleInfo.CanDisplace;
		_offset = obstacleInfo.Offset;
		OffsetAwake = _offset;
		_cellType = obstacleInfo.CellType;
		_statusEffect = obstacleInfo.StatusEffect;
		_statusEffectDuration = obstacleInfo.StatusEffectDuration;
		obstacleInfo.IsSpawned = true;

		return Init();
	}

	public void ChangeType()
	{
		SpawnObstacle();
	}

	public int GetLength(CellType cellType)
	{
		foreach (var cellInfo in _cellInfos)
		{
			if (cellInfo.Type == cellType)
			{
				return cellInfo.Obstacle.Length;
			}
		}

		return 1;
	}

	protected override void Awake()
	{
		base.Awake();

		_offset = OffsetAwake;

		SpawnObstacle();
	}

	protected override void SpawnObstacle()
	{
		_lastCellType = _cellType;

		ActiveObstacle();
	}

	private void ActiveObstacle()
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(false);
		}

		CellInfo cellInfo = _cellInfos.Find(cellInfo => cellInfo.Type == _cellType);
		cellInfo?.Obstacle.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (Application.isPlaying == false)
		{
			if (_lastCellType != _cellType && FindObjectOfTypeInParents<Chunk>() != null)
			{
				SpawnObstacle();
			}
		}
	}
}
