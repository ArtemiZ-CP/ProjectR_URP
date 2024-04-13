using System;
using UnityEngine;

[Serializable]
public class CellInfo
{
	[SerializeField] private CellType _cellType;
	[SerializeField] private Obstacle _obstaclePrefab;

	public CellType Type => _cellType;
	public Obstacle Obstacle => _obstaclePrefab;
}
