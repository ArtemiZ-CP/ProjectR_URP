using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(ChunkGenerator))]
public class Chunk : MonoBehaviour
{
	public const float GridSize = 1;

	[SerializeField] private Transform _roadsParent;
	[SerializeField] private Transform _linesParent;
	[SerializeField] private int _chunksCount;
	[SerializeField] private bool _showGrid;

	[SerializeField] private Transform _parentObstacle;
	[SerializeField] private Transform _parentPowerup;

	private List<Line> _lines = new();
	private GameObject _roadPrefab;
	private BaseObstacle _baseObstaclePrefab;
	private BasePowerup _basePowerupPrefab;
	private ChunkGenerator _chunkGenerator;

	public ChunkGenerator ChunkGenerator => _chunkGenerator;
	public bool ShowGrid => _showGrid;
	public List<Line> Lines => _lines;
	public float Length => _chunksCount * ChunkSize;
	public int RoadLengthInt => _chunksCount * _lines.Count;
	public float ChunkSize => CurrentChunkSettings.Settings.RoadOffset * _lines.Count;

	private void Awake()
	{
		_chunkGenerator = GetComponent<ChunkGenerator>();

		if (Application.isPlaying == false)
		{
			Reload();
		}
		else
		{
			_showGrid = false;
			Init();
		}
	}

	public void SpawnObject()
	{
		BaseObstacle baseObstacle = Instantiate(_baseObstaclePrefab, _parentObstacle);
		baseObstacle.Init();

#if UNITY_EDITOR
		Selection.activeGameObject = baseObstacle.gameObject;
#endif

		SavePrefab();
	}

	public void SpawnPowerUp()
	{
		BasePowerup basePowerup = Instantiate(_basePowerupPrefab, _parentPowerup);
		basePowerup.Init();

#if UNITY_EDITOR
		Selection.activeGameObject = basePowerup.gameObject;
#endif

		SavePrefab();
	}

	public void Init()
	{
		_lines = new List<Line>(_linesParent.GetComponentsInChildren<Line>());
	}

	public void Reload()
	{
		Init();

		_roadPrefab = CurrentChunkSettings.Settings.RoadPrefab;
		_baseObstaclePrefab = CurrentChunkSettings.Settings.BaseObstaclePrefab;
		_basePowerupPrefab = CurrentChunkSettings.Settings.BasePowerupPrefab;

		if (_parentObstacle.childCount > 0)
		{
			foreach (Transform obstacle in _parentObstacle)
			{
				if (obstacle.TryGetComponent(out BaseObstacle baseObstacle))
				{
					baseObstacle.Reload();
				}
			}
		}

		if (_parentPowerup.childCount > 0)
		{
			foreach (Transform powerup in _parentPowerup)
			{
				if (powerup.TryGetComponent(out BasePowerup basePowerup))
				{
					basePowerup.Reload();
				}
			}
		}

		SpawnRoad();

		SavePrefab();
	}

	public void SavePrefab()
	{
#if UNITY_EDITOR
		EditorUtility.SetDirty(gameObject);
#endif
	}

	private void Update()
	{
		if (Application.isPlaying == false)
		{
			SetupLines();

			if (_roadsParent.childCount != _chunksCount)
			{
				SpawnRoad();
			}
		}
	}

	private void SetupLines()
	{
		if (_lines.Count != 0)
		{
			int halfLinesCount = _lines.Count / 2;
			int offset = _lines.Count % 2 == 0 ? halfLinesCount - 1 : halfLinesCount;

			for (int i = 0; i < _lines.Count; i++)
			{
				int lineIndex = i - offset;
				_lines[i].transform.position = new Vector3(lineIndex * GridSize * CurrentChunkSettings.Settings.RoadOffset, 0, 0);
			}
		}
	}

	private void SpawnRoad()
	{
		for (int i = _roadsParent.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(_roadsParent.GetChild(i).gameObject);
		}

		for (int i = 0; i < _chunksCount; i++)
		{
			Instantiate(_roadPrefab, new Vector3(0, 0, i * ChunkSize + ChunkSize / 2) + transform.position, _roadPrefab.transform.rotation, _roadsParent);
		}
	}

	private void OnDrawGizmos()
	{
		if (_showGrid)
		{
			if (_lines.Count != 0)
			{
				for (int i = 0; i < _lines.Count; i++)
				{
					Gizmos.DrawLine(_lines[i].transform.position, _lines[i].transform.position + Vector3.forward * Length);
				}

				for (float z = 0; z <= _chunksCount * _lines.Count; z += GridSize)
				{
					if (z == 0)
					{
						Gizmos.color = Color.green;
					}
					else if (z == _chunksCount * _lines.Count)
					{
						Gizmos.color = Color.red;
					}
					else
					{
						Gizmos.color = Color.white;
					}

					Vector3 start = new(_lines[0].transform.position.x - CurrentChunkSettings.Settings.RoadOffset / 2, 0, z * CurrentChunkSettings.Settings.RoadOffset);
					Vector3 end = new(_lines[^1].transform.position.x + CurrentChunkSettings.Settings.RoadOffset / 2, 0, z * CurrentChunkSettings.Settings.RoadOffset);

					Gizmos.DrawLine(start + transform.position, end + transform.position);
				}
			}
		}
	}
}
