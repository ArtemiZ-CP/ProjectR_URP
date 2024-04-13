using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomObstacles : MonoBehaviour
{
	[SerializeField] private List<BaseObstacle> _obstacles = new();
	[SerializeField] private int _numberOfSpawnObstacles;

	private void Start()
	{
		SpawnObstacles();
	}

	[ContextMenu("Spawn Obstacles")]
	private void SpawnObstacles()
	{
		List<BaseObstacle> obstacles = GetRandomObstacle(_numberOfSpawnObstacles);
		
		SetObstaclesActive(_obstacles, false);
		SetObstaclesActive(obstacles, true);
	}

	[ContextMenu("Reset Obstacles")]
	private void ResetObstacles()
	{
		SetObstaclesActive(_obstacles, true);
	}

	private void SetObstaclesActive(List<BaseObstacle> obstacles, bool isActive)
	{
		foreach (BaseObstacle obstacle in obstacles)
		{
			obstacle.gameObject.SetActive(isActive);
		}
	}

	private List<BaseObstacle> GetRandomObstacle(int count)
	{
		List<BaseObstacle> obstaclesToSpawn = new();
		List<BaseObstacle> obstacles = new(_obstacles);

		for (int i = 0; i < count; i++)
		{
			if (obstacles.Count == 0)
			{
				return obstaclesToSpawn;
			}

			BaseObstacle obstacle = obstacles[Random.Range(0, obstacles.Count)];
			obstaclesToSpawn.Add(obstacle);
			obstacles.Remove(obstacle);
		}

		return obstaclesToSpawn;
	}
}
