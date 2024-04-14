using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	private readonly List<Chunk> _spawnedChunks = new();
	private readonly List<Enviroment> _spawnedEnviroment = new();

	[SerializeField] private PlayerMovement _playerMovement;
	[Header("Chunk")]
	[SerializeField] private Transform _chunksParent;
	[SerializeField] private Chunk _randomChunk;
	[SerializeField] private bool _random;
	[SerializeField] private List<Chunk> _startChunks = new();
	[SerializeField] private List<Chunk> _generalChunks = new();
	[Header("Enviroment")]
	[SerializeField] private List<Enviroment> _enviromentObjects;
	[SerializeField] private Transform _enviromentParent;
	[Header("Settings")]
	[SerializeField] private float _startSpawnPositionZ;
	[SerializeField, Min(0)] private float _distanceToSpawnNewChunk;

	private float _chunkPositionZ;
	private float _enviromentPositionZ;
	private int _lastRoadIndex;

	private void Start()
	{
		_chunkPositionZ = _startSpawnPositionZ;
		_enviromentPositionZ = _startSpawnPositionZ;

		_lastRoadIndex = SpawnStartChunk();
	}

	private void Update()
	{
		while (CheckDistanceToSpawnNewChunk())
		{
			if (_random)
			{
				_lastRoadIndex = SpawnRandomChunk(_lastRoadIndex);
			}
			else
			{
				_lastRoadIndex = SpawnGeneralChunk(_lastRoadIndex);
			}
		}

		while (CheckDistanceToSpawnNewEnviroment())
		{
			if (SpawnRandomEnviroment() == false)
			{
				break;
			}
		}

		foreach (Chunk chunk in _spawnedChunks)
		{
			if (chunk.ChunkGenerator.ChunkMap != null)
			{
				chunk.ChunkGenerator.SpawnObstacles(_distanceToSpawnNewChunk + _playerMovement.transform.position.z);
			}
		}

		DestroyPastObjects();
	}

	public ObstacleInfo GetRandomCell(float minZ, float maxZ, CellType cellType)
	{
		List<ObstacleInfo> cells = new();

		foreach (Chunk chunk in _spawnedChunks)
		{
			List<ObstacleInfo> newCells = GetCells(chunk, minZ, maxZ, cellType);

			if (newCells != null)
			{
				cells.AddRange(newCells);
			}
		}

		if (cells.Count == 0)
		{
			return null;
		}

		ObstacleInfo cell = cells[Random.Range(0, cells.Count)];

		return cell;
	}

	#region Chunks

	private List<ObstacleInfo> GetCells(Chunk chunk, float minZ, float maxZ, CellType targetCellType)
	{
		if (chunk.transform.position.z - chunk.Length > maxZ || chunk.transform.position.z + chunk.Length < minZ)
		{
			return null;
		}

		ObstacleInfo[,] obstacleInfos = chunk.ChunkGenerator.ChunkMap;

		List<ObstacleInfo> cells = new();

		for (int x = 0; x < obstacleInfos.GetLength(0); x++)
		{
			for (int y = 0; y < obstacleInfos.GetLength(1); y++)
			{
				if (obstacleInfos[x, y].CellType == targetCellType)
				{
					Vector3 position = obstacleInfos[x, y].GetCellPosition();

					if (position.z >= minZ && position.z <= maxZ)
					{
						cells.Add(obstacleInfos[x, y]);
					}
				}
			}
		}

		return cells;
	}

	private int SpawnStartChunk()
	{
		ChunkGenerator chunkGenerator = SpawnNextChunk(GetRandomChunk(_startChunks)).ChunkGenerator;
		return chunkGenerator.EndRoadIndex;
	}

	private int SpawnGeneralChunk(int roadIndex)
	{
		int endRoadIndex;

		if (TryGetRandomChunk(_generalChunks, roadIndex, out Chunk chunk))
		{
			endRoadIndex = SpawnNextChunk(chunk).ChunkGenerator.EndRoadIndex;
		}
		else
		{
			endRoadIndex = SpawnRandomChunk(roadIndex);
		}

		return endRoadIndex;
	}

	private int SpawnRandomChunk(int roadIndex)
	{
		ChunkGenerator chunkGenerator = SpawnNextChunk(_randomChunk).ChunkGenerator;
		chunkGenerator.GenerateRandomSeed();
		chunkGenerator.FillMap(roadIndex);
		return chunkGenerator.EndRoadIndex;
	}

	private Chunk GetRandomChunk(List<Chunk> chunks)
	{
		return chunks[Random.Range(0, chunks.Count)];
	}

	private bool TryGetRandomChunk(List<Chunk> chunks, int roadIndex, out Chunk chunk)
	{
		chunks = chunks.FindAll(chunk => chunk.ChunkGenerator.StartRoadIndex == roadIndex);

		if (chunks.Count == 0)
		{
			chunk = _randomChunk;
			return false;
		}

		chunk = chunks[Random.Range(0, chunks.Count)];
		return true;
	}

	private bool CheckDistanceToSpawnNewChunk()
	{
		return _playerMovement.transform.position.z > _chunkPositionZ - _distanceToSpawnNewChunk;
	}

	private Chunk SpawnNextChunk(Chunk chunk)
	{
		chunk = Instantiate(chunk, _chunkPositionZ * Vector3.forward, Quaternion.identity, _chunksParent);
		chunk.Init();
		_spawnedChunks.Add(chunk);
		_chunkPositionZ += chunk.Length;
		return chunk;
	}

	#endregion

	#region Enviroment

	private bool SpawnRandomEnviroment()
	{
		if (TryGetRandomEnviroment(_enviromentObjects, out Enviroment enviromentObject))
		{
			SpawnNextEnviroment(enviromentObject);
			return true;
		}

		return false;
	}

	private bool TryGetRandomEnviroment(List<Enviroment> enviromentObjects, out Enviroment enviromentObject)
	{
		if (enviromentObjects.Count == 0)
		{
			enviromentObject = null;
			return false;
		}

		enviromentObject = enviromentObjects[Random.Range(0, enviromentObjects.Count)];
		return true;
	}

	private bool CheckDistanceToSpawnNewEnviroment()
	{
		return _playerMovement.transform.position.z > _enviromentPositionZ - _distanceToSpawnNewChunk;
	}

	private void SpawnNextEnviroment(Enviroment enviroment)
	{
		enviroment = Instantiate(enviroment, _enviromentPositionZ * Vector3.forward, Quaternion.identity, _enviromentParent);
		_spawnedEnviroment.Add(enviroment);
		_enviromentPositionZ += enviroment.Length;
	}

	#endregion

	private void DestroyPastObjects()
	{
		Chunk chunkToDestroy = null;

		foreach (Chunk chunk in _spawnedChunks)
		{
			if (chunk.transform.position.z + chunk.Length < _playerMovement.transform.position.z + _startSpawnPositionZ)
			{
				chunkToDestroy = chunk;
				break;
			}
		}

		if (chunkToDestroy != null)
		{
			_spawnedChunks.Remove(chunkToDestroy);
			Destroy(chunkToDestroy.gameObject);
		}

		Enviroment enviromentToDestroy = null;

		foreach (Enviroment enviroment in _spawnedEnviroment)
		{
			if (enviroment.transform.position.z + enviroment.Length < _playerMovement.transform.position.z + _startSpawnPositionZ)
			{
				enviromentToDestroy = enviroment;
				break;
			}
		}

		if (enviromentToDestroy != null)
		{
			_spawnedEnviroment.Remove(enviromentToDestroy);
			Destroy(enviromentToDestroy.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(Vector3.forward * _startSpawnPositionZ, 0.5f);
		Gizmos.DrawLine(_playerMovement.transform.position, _playerMovement.transform.position + Vector3.forward * _distanceToSpawnNewChunk);
	}
}
