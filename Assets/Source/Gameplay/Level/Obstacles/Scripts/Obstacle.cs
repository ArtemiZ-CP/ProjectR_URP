using UnityEngine;

[ExecuteAlways]
public class Obstacle : MonoBehaviour
{
	[SerializeField] private int _length = 1;

	private BaseObstacle _baseObstacle;
	private Chunk _currentChunk;

	public int Damage => _baseObstacle.Damage;
	public bool IsDestroyable => _baseObstacle.IsDestroyable;
	public bool CanDisplace => _baseObstacle.CanDisplace;
	public int Offset => _baseObstacle.Offset;
	public int RoadIndex => _baseObstacle.RoadIndex;
	public CellType CellType => _baseObstacle.CellType;
	public StatusEffect StatusEffect => _baseObstacle.StatusEffect;
	public float StatusEffectDuration => _baseObstacle.StatusEffectDuration;
	public int Length => _length;

	private void Awake()
	{
		_baseObstacle = transform.GetComponentInParent<BaseObstacle>();
		_currentChunk = GetChunkInParents();
	}

	public void Disactive()
	{
		_baseObstacle.gameObject.SetActive(false);
	}

	private Chunk GetChunkInParents()
	{
		Transform parent = transform.parent;

		while (parent != null)
		{
			Chunk chunk = parent.GetComponent<Chunk>();

			if (chunk != null)
			{
				return chunk;
			}

			parent = parent.parent;
		}

		return null;
	}

	private void OnDrawGizmos()
	{
		if (_currentChunk == null || _currentChunk.ShowGrid)
		{
			Gizmos.color = Color.red;
			float roadOffset = CurrentChunkSettings.Settings.RoadOffset;
			Vector3 center = transform.position + Vector3.forward * (Length - 1) * roadOffset / 2;
			Vector3 size = new(roadOffset, roadOffset, roadOffset * Length);
			center.y = roadOffset / 2;

			Gizmos.DrawWireCube(center, size);
			Gizmos.DrawWireCube(center + Vector3.up * roadOffset, size);
		}
	}
}
