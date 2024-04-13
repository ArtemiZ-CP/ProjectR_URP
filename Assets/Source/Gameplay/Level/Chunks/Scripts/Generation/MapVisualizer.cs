using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(ChunkGenerator))]
public class MapVisualizer : MonoBehaviour
{
	[SerializeField] private List<ChunkCellInfo> _chunkCellInfos;
	[SerializeField] private float _viewScale = 0.5f;

	private Texture2D _mapTexture;
	private ChunkGenerator _chunkGenerator;

	public Texture2D NoiseTexture => _mapTexture;
	public float ViewScale => _viewScale;

	private void Awake()
	{
		_chunkGenerator = GetComponent<ChunkGenerator>();
	}

	private void Update()
	{
		Generate(_chunkGenerator.LinesCount, _chunkGenerator.ChunkLength, _chunkGenerator.ChunkMap);
	}

	private void Generate(int width, int height, ObstacleInfo[,] map)
	{
		if (map == null || map.GetLength(0) != width || map.GetLength(1) != height)
		{
			return;
		}

		_mapTexture = new Texture2D(width, height);

		Color[] pixels = new Color[width * height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				pixels[y * width + x] = GetCellColor(map[x, y].CellType);
			}
		}

		_mapTexture.SetPixels(pixels);
		_mapTexture.Apply();

		_mapTexture.filterMode = FilterMode.Point;
		_mapTexture.wrapMode = TextureWrapMode.Clamp;
	}

	private Color GetCellColor(CellType cellType)
	{
		foreach (var chunkCellInfo in _chunkCellInfos)
		{
			if (chunkCellInfo.Type == cellType)
			{
				return chunkCellInfo.Color;
			}
		}

		return Color.white;
	}
}

[Serializable]
public struct ChunkCellInfo
{
	public CellType Type;
	public Color Color;
}