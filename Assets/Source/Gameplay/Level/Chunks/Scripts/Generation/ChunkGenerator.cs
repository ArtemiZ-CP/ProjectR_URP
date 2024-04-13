using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteAlways, RequireComponent(typeof(Chunk))]
public class ChunkGenerator : MonoBehaviour
{
	[SerializeField] private Transform _obstacleParent;

	#region Settings

	[SerializeField, Min(1)] private int _minLengthOfRoad = 0;
	[SerializeField, Min(1)] private int _minStraightLengthOfRoadAfterChangeType = 0;
	[SerializeField, Min(1)] private int _minLengthOfSecondFloor = 0;
	[SerializeField, Min(1)] private int _minStraightLengthOfSecondFloorAfterChangeType = 0;
	[SerializeField, Min(1)] private int _minLengthAfterChangeLine = 0;
	[SerializeField, Min(1)] private int _minRoadLengthNearObstacleOnRoad = 0;
	[SerializeField] private bool _cancelRoadAfterChangeLine;
	[SerializeField, Min(2)] private int _minSecondFloorLengthToSpawnStairs = 3;

	#endregion

	#region Randon Generation Chance

	[SerializeField] private bool _fixChanceToChangeRoad = false;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoad;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadMin;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadMax;

	[SerializeField] private bool _fixChanceToChangeRoadType = false;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadType;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadTypeMin;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadTypeMax;

	[SerializeField] private bool _fixChanceToChangeRoadTypeToSecondFloor = false;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadTypeToSecondFloor;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadTypeToSecondFloorMin;
	[SerializeField, Range(0, 1f)] private float _chanceToChangeRoadTypeToSecondFloorMax;

	[SerializeField] private bool _fixChanceToSpawnObstacleOnRoad = false;
	[SerializeField, Range(0, 1f)] private float _chanceToSpawnObstacleOnRoad;
	[SerializeField, Range(0, 1f)] private float _chanceToSpawnObstacleOnRoadMin;
	[SerializeField, Range(0, 1f)] private float _chanceToSpawnObstacleOnRoadMax;

	[SerializeField] private bool _fixChanceToSpawnFullObstacle = false;
	[SerializeField, Range(0, 1f)] private float _chanceToSpawnFullObstacle;
	[SerializeField, Range(0, 1f)] private float _chanceToSpawnFullObstacleMin;
	[SerializeField, Range(0, 1f)] private float _chanceToSpawnFullObstacleMax;

	#endregion

	private BaseObstacle _baseObstaclePrefab;
	private Chunk _currentChunk;
	private ObstacleInfo[,] _chunkMap;
	private int _currentLengthOfOneRoadType;
	private int _currentLengthAfterChangeLine;
	private int _currentStraightLengthAfterChangeType;

	private int _startRoadIndex;
	private int _endRoadIndex;

	public ObstacleInfo[,] ChunkMap => _chunkMap;
	public int ChunkLength => _currentChunk.RoadLengthInt;
	public int LinesCount => _currentChunk.Lines.Count;
	public int StartRoadIndex => _startRoadIndex;
	public int EndRoadIndex => _endRoadIndex;

	private void Awake()
	{
		_currentChunk = GetComponent<Chunk>();
		_baseObstaclePrefab = CurrentChunkSettings.Settings.BaseObstaclePrefab;
	}

	public Vector3 GetCellPosition(ObstacleInfo obstacleInfo)
	{
		for (int x = 0; x < _chunkMap.GetLength(0); x++)
		{
			for (int y = 0; y < _chunkMap.GetLength(1); y++)
			{
				if (_chunkMap[x, y] == obstacleInfo)
				{
					return GetCellPosition(x, y);
				}
			}
		}

		return Vector3.zero;
	}

	#region Generation

	public void GenerateObstaclesWithRandomSeed()
	{
		GenerateObstaclesWithRandomSeed(Random.Range(0, LinesCount));
	}

	public void GenerateObstaclesWithRandomSeed(int roadIndex)
	{
		if (_fixChanceToChangeRoad == false)
		{
			_chanceToChangeRoad = Random.Range(_chanceToChangeRoadMin, _chanceToChangeRoadMax);
		}

		if (_fixChanceToChangeRoadType == false)
		{
			_chanceToChangeRoadType = Random.Range(_chanceToChangeRoadTypeMin, _chanceToChangeRoadTypeMax);
		}

		if (_fixChanceToChangeRoadTypeToSecondFloor == false)
		{
			_chanceToChangeRoadTypeToSecondFloor = Random.Range(_chanceToChangeRoadTypeToSecondFloorMin, _chanceToChangeRoadTypeToSecondFloorMax);
		}

		if (_fixChanceToSpawnObstacleOnRoad == false)
		{
			_chanceToSpawnObstacleOnRoad = Random.Range(_chanceToSpawnObstacleOnRoadMin, _chanceToSpawnObstacleOnRoadMax);
		}

		if (_fixChanceToSpawnFullObstacle == false)
		{
			_chanceToSpawnFullObstacle = Random.Range(_chanceToSpawnFullObstacleMin, _chanceToSpawnFullObstacleMax);
		}

		GenerateChunk(roadIndex);
	}

	public void GenerateObstacles()
	{
		GenerateChunk(Random.Range(0, LinesCount));
	}

	public void GenerateChunk(int roadIndex)
	{
		_startRoadIndex = roadIndex;

		DestroyAllObstacles();
		InitializeGrid();
		_endRoadIndex = FillGrid(roadIndex);
		SpawnObstacles();

		_currentChunk.Reload();
	}

	#endregion

	public void DestroyAllObstacles()
	{
		for (int i = _obstacleParent.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(_obstacleParent.GetChild(i).gameObject);
		}

		_currentChunk.SavePrefab();
	}

	private void InitializeGrid()
	{
		_chunkMap = new ObstacleInfo[LinesCount, ChunkLength];

		for (int i = 0; i < LinesCount; i++)
		{
			for (int j = 0; j < ChunkLength; j++)
			{
				_chunkMap[i, j] = new ObstacleInfo(_currentChunk);
			}
		}
	}

	private int FillGrid(int roadIndex)
	{
		int lastRoadIndex = SetPath(roadIndex);
		Unclutter();
		SetObstacles();
		SetObstaclesOnRoad();
		SetupObstacles();

		return lastRoadIndex;
	}

	#region Path generation

	private int SetPath(int roadLine = 0)
	{
		bool isSecondFloor = false;
		bool justChangeRoadType = false;

		for (int y = 0; y < ChunkLength; y++)
		{
			if (TryToChangeRoadType(isSecondFloor))
			{
				isSecondFloor = !isSecondFloor;
				justChangeRoadType = true;
			}

			SetRoad(roadLine, y, isSecondFloor);

			if (justChangeRoadType)
			{
				justChangeRoadType = false;
			}
			else
			{
				if (TryToChangeLine(isSecondFloor ? _minStraightLengthOfSecondFloorAfterChangeType : _minStraightLengthOfRoadAfterChangeType))
				{
					if (_cancelRoadAfterChangeLine)
					{
						isSecondFloor = false;
					}

					roadLine = ChangeRoad(roadLine);
					SetRoad(roadLine, y, isSecondFloor);

					if (y > 0)
					{
						SetRoad(roadLine, y - 1, isSecondFloor);
					}
					else
					{
						SetRoad(roadLine, y + 1, isSecondFloor);
					}
				}
			}

			_currentLengthOfOneRoadType++;
			_currentLengthAfterChangeLine++;
			_currentStraightLengthAfterChangeType++;
		}

		return roadLine;
	}

	private bool TryToChangeRoadType(bool isSecondFloor)
	{
		int minLength = isSecondFloor ? _minLengthOfSecondFloor : _minLengthOfRoad;

		if (Random.Range(0, 1f) < _chanceToChangeRoadType && _currentLengthOfOneRoadType > minLength - 1)
		{
			if (isSecondFloor && Random.Range(0, 1f) > _chanceToChangeRoadTypeToSecondFloor)
			{
				_currentLengthOfOneRoadType = 0;
				_currentStraightLengthAfterChangeType = 0;

				return true;
			}

			if (isSecondFloor == false && Random.Range(0, 1f) < _chanceToChangeRoadTypeToSecondFloor)
			{
				_currentLengthOfOneRoadType = 0;
				_currentStraightLengthAfterChangeType = 0;

				return true;
			}
		}

		return false;
	}

	private bool TryToChangeLine(int minLength)
	{
		if (_currentStraightLengthAfterChangeType > minLength - 1)
		{
			if (Random.Range(0, 1f) < _chanceToChangeRoad && _currentLengthAfterChangeLine > _minLengthAfterChangeLine - 1)
			{
				_currentLengthAfterChangeLine = 0;

				return true;
			}
		}

		return false;
	}

	private void SetRoad(int x, int y, bool isSecondFloor)
	{
		if (isSecondFloor)
		{
			_chunkMap[x, y].CellType = CellType.SecondFloor;

			if (TryGetCellType(x - 1, y, out CellType cellType) && cellType == CellType.Empty)
			{
				_chunkMap[x - 1, y].CellType = CellType.Road;
			}

			if (TryGetCellType(x + 1, y, out cellType) && cellType == CellType.Empty)
			{
				_chunkMap[x + 1, y].CellType = CellType.Road;
			}

			if (TryGetCellType(x - 1, y - 1, out cellType) && cellType == CellType.Empty)
			{
				_chunkMap[x - 1, y - 1].CellType = CellType.Road;
			}

			if (TryGetCellType(x + 1, y - 1, out cellType) && cellType == CellType.Empty)
			{
				_chunkMap[x + 1, y - 1].CellType = CellType.Road;
			}

			if (TryGetCellType(x - 1, y + 1, out cellType) && cellType == CellType.Empty)
			{
				_chunkMap[x - 1, y + 1].CellType = CellType.Road;
			}

			if (TryGetCellType(x + 1, y + 1, out cellType) && cellType == CellType.Empty)
			{
				_chunkMap[x + 1, y + 1].CellType = CellType.Road;
			}
		}
		else
		{
			_chunkMap[x, y].CellType = CellType.Road;
		}
	}

	private int ChangeRoad(int roadLine)
	{
		if (roadLine == 0)
		{
			return roadLine + 1;
		}

		if (roadLine == LinesCount - 1)
		{
			return roadLine - 1;
		}

		return Random.Range(0, 2) == 0 ? roadLine - 1 : roadLine + 1;
	}

	#endregion

	#region Unclutter

	private void Unclutter()
	{
		for (int y = 0; y < ChunkLength; y++)
		{
			for (int x = 0; x < LinesCount; x++)
			{
				if (TryGetCellType(x, y, out CellType cellType))
				{
					if (cellType == CellType.SecondFloor || cellType == CellType.Empty)
					{
						CheckObstacleSpawnValid(x, y);
					}
				}
			}
		}
	}

	private void CheckObstacleSpawnValid(int x, int y)
	{
		if (IsCellUnpassable(x, y - 1))
		{
			return;
		}

		if (IsCellUnpassable(x - 1, y - 1))
		{
			if (IsCellUnpassable(x + 1, y) == false && IsCellUnpassable(x + 1, y - 1) == false)
			{
				_chunkMap[x + 1, y].CellType = CellType.Road;
				_chunkMap[x + 1, y - 1].CellType = CellType.Road;

				return;
			}

			_chunkMap[x, y].CellType = CellType.Road;
			return;
		}

		if (IsCellUnpassable(x + 1, y - 1))
		{
			if (IsCellUnpassable(x - 1, y) == false && IsCellUnpassable(x - 1, y - 1) == false)
			{
				_chunkMap[x - 1, y].CellType = CellType.Road;
				_chunkMap[x - 1, y - 1].CellType = CellType.Road;

				return;
			}

			_chunkMap[x, y].CellType = CellType.Road;
			return;
		}

		if (IsCellUnpassable(x - 1, y))
		{
			_chunkMap[x, y].CellType = CellType.Road;
			return;
		}

		if (IsCellUnpassable(x + 1, y))
		{
			_chunkMap[x, y].CellType = CellType.Road;
			return;
		}

		if (Random.Range(0, 2) == 0)
		{
			_chunkMap[x + 1, y].CellType = CellType.Road;
			_chunkMap[x + 1, y - 1].CellType = CellType.Road;
		}
		else
		{
			_chunkMap[x - 1, y].CellType = CellType.Road;
			_chunkMap[x - 1, y - 1].CellType = CellType.Road;
		}
	}

	#endregion

	#region Set obstacle

	private void SetObstacles()
	{
		for (int x = 0; x < LinesCount; x++)
		{
			for (int y = 0; y < ChunkLength; y++)
			{
				if (Random.Range(0, 1f) < _chanceToSpawnFullObstacle && _chunkMap[x, y].CellType == CellType.Empty)
				{
					_chunkMap[x, y].CellType = CellType.FullObstacle;
				}
			}
		}
	}

	#endregion

	#region Obstacle on road generation

	private void SetObstaclesOnRoad()
	{
		for (int x = 0; x < LinesCount; x++)
		{
			for (int y = 0; y < ChunkLength; y++)
			{
				if (IsAbleToSpawnObstacleOnRoad(x, y) && Random.Range(0, 1f) < _chanceToSpawnObstacleOnRoad)
				{
					if (_chunkMap[x, y].CellType == CellType.Road || _chunkMap[x, y].CellType == CellType.Empty)
					{
						_chunkMap[x, y].CellType = GetRandomObstacle();

						_chunkMap[x, y - 1].CellType = CellType.Road;
						_chunkMap[x, y + 1].CellType = CellType.Road;
					}
				}
			}
		}
	}

	private bool IsAbleToSpawnObstacleOnRoad(int x, int y)
	{
		TryGetCellType(x, y, out CellType targetCellType);

		if (targetCellType == CellType.Empty)
		{
			targetCellType = CellType.Road;
		}

		if (targetCellType != CellType.Road && targetCellType != CellType.SecondFloor)
		{
			return false;
		}

		for (int i = -_minRoadLengthNearObstacleOnRoad; i <= _minRoadLengthNearObstacleOnRoad; i++)
		{
			if (TryGetCellType(x, y + i, out CellType cellType))
			{
				if (cellType == CellType.Empty)
				{
					cellType = CellType.Road;
				}

				if (cellType != targetCellType)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		return true;
	}

	private CellType GetRandomObstacle()
	{
		return Random.Range(0, 3) switch
		{
			0 => CellType.LowerObstacle,
			1 => CellType.UpperObstacle,
			_ => CellType.LowerUpperObstacle,
		};
	}

	#endregion

	#region Setup obstacles

	private void SetupObstacles()
	{
		for (int line = 0; line < LinesCount; line++)
		{
			for (int position = 0; position < ChunkLength;)
			{
				position += SetupObstacle(line, position);
			}
		}
	}

	private int SetupObstacle(int line, int position)
	{
		if (IsAbleToSpawnStairs(line, position))
		{
			return SpawnStairs(line, position);
		}

		bool isDestoyable;
		bool canDisplace;
		int offset = 0;

		if (_chunkMap[line, position].CellType == CellType.FullObstacle || _chunkMap[line, position].CellType == CellType.SecondFloor)
		{
			canDisplace = true;

			if (IsCellUnpassable(line - 1, position - 1) == false && IsCellUnpassable(line - 1, position) == false &&
				IsCellUnpassable(line + 1, position - 1) == false && IsCellUnpassable(line + 1, position) == false)
			{
				offset = 0;
			}
			else if (IsCellUnpassable(line - 1, position - 1) == false && IsCellUnpassable(line - 1, position) == false)
			{
				offset = -1;
			}
			else if (IsCellUnpassable(line + 1, position - 1) == false && IsCellUnpassable(line + 1, position) == false)
			{
				offset = 1;
			}
			else
			{
				if (_chunkMap[line, position].CellType == CellType.FullObstacle)
				{
					_chunkMap[line, position].CellType = CellType.FullDestroyableObstacle;
					canDisplace = false;
				}
			}
		}
		else
		{
			canDisplace = false;
		}

		if (_chunkMap[line, position].CellType == CellType.LowerObstacle ||
			_chunkMap[line, position].CellType == CellType.UpperObstacle ||
			_chunkMap[line, position].CellType == CellType.LowerUpperObstacle ||
			_chunkMap[line, position].CellType == CellType.FullDestroyableObstacle)
		{
			isDestoyable = true;
		}
		else
		{
			isDestoyable = false;
		}

		if (_chunkMap[line, position].CellType == CellType.Stairs)
		{
			canDisplace = true;
		}

		int length = _baseObstaclePrefab.GetLength(_chunkMap[line, position].CellType);

		if (length + position >= _chunkMap.GetLength(1))
		{
			_chunkMap[line, position].CellType = CellType.Road;
		}

		for (int i = 0; i < length; i++)
		{
			_chunkMap[line, position + i] = _chunkMap[line, position];
		}

		_chunkMap[line, position].Line = line;
		_chunkMap[line, position].Offset = offset;
		_chunkMap[line, position].IsDestroyable = isDestoyable;
		_chunkMap[line, position].CanDisplace = canDisplace;
		_chunkMap[line, position].Length = length;

		return length;
	}

	private bool IsAbleToSpawnStairs(int x, int y)
	{
		if (TryGetCellType(x, y - 1, out CellType cellType) && (cellType == CellType.SecondFloor || cellType == CellType.Stairs))
		{
			return false;
		}

		for (int i = 0; i < _minSecondFloorLengthToSpawnStairs; i++)
		{
			if (TryGetCellType(x, y + i, out cellType))
			{
				if (cellType != CellType.SecondFloor)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		return true;
	}

	private int SpawnStairs(int x, int y)
	{
		_chunkMap[x, y].CellType = CellType.Stairs;
		_chunkMap[x, y].IsDestroyable = false;
		_chunkMap[x, y].CanDisplace = true;
		_chunkMap[x, y].Line = x;
		_chunkMap[x, y].Length = _baseObstaclePrefab.GetLength(CellType.Stairs);

		return _chunkMap[x, y].Length;
	}

	#endregion

	#region Spawn obstacles

	private void SpawnObstacles()
	{
		for (int line = 0; line < LinesCount; line++)
		{
			for (int position = 0; position < ChunkLength;)
			{
				position += InstantiateObstacle(line, position);
			}
		}
	}

	private int InstantiateObstacle(int line, int position)
	{
		Vector3 obstaclePosition = GetCellPosition(line, position);

		BaseObstacle baseObstacle;

#if UNITY_EDITOR
		baseObstacle = (BaseObstacle)PrefabUtility.InstantiatePrefab(_baseObstaclePrefab, _obstacleParent);
		baseObstacle.transform.position = obstaclePosition;
#else
		baseObstacle = Instantiate(_baseObstaclePrefab, obstaclePosition, Quaternion.identity, _obstacleParent);
#endif

		_chunkMap[line, position].BaseObstacle = baseObstacle;
		baseObstacle.Init(_chunkMap[line, position]);

		return _chunkMap[line, position].Length;
	}

	#endregion

	private bool IsCellUnpassable(int x, int y)
	{
		if (TryGetCellType(x, y, out CellType cellType))
		{
			if (cellType == CellType.FullObstacle || cellType == CellType.SecondFloor || cellType == CellType.Empty)
			{
				return true;
			}
		}
		else
		{
			return true;
		}

		return false;
	}

	private bool TryGetCellType(int x, int y, out CellType cellType)
	{
		if (x < 0 || x >= LinesCount || y < 0 || y >= ChunkLength)
		{
			cellType = CellType.Empty;
			return false;
		}

		cellType = _chunkMap[x, y].CellType;
		return true;
	}

	private Vector3 GetCellPosition(int line, int position)
	{
		float xPos = _currentChunk.Lines[line].transform.position.x;
		float zPos = (position + 1) * CurrentChunkSettings.Settings.RoadOffset;
		return new Vector3(xPos, 0, zPos) + transform.position;
	}
}
