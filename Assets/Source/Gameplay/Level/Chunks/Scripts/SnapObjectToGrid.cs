using UnityEngine;

[ExecuteAlways]
public class SnapObjectToGrid : MonoBehaviour
{
	private Chunk _chunk;
	private int _roadIndex;

	public int RoadIndex => _roadIndex;

	private void Awake()
	{
		if (Application.isPlaying == false)
		{
			_chunk = FindObjectOfTypeInParents<Chunk>();
			SnapToGrid();
		}
	}

	private void Update()
	{
		if (Application.isPlaying == false)
		{
			if (_chunk != null)
			{
				SnapToGrid();
			}
		}
	}

	public void SetLine(int line)
	{
		_roadIndex = line;
	}

	private T FindObjectOfTypeInParents<T>() where T : Component
	{
		Transform parent = transform.parent;

		while (parent != null)
		{
			if (parent.TryGetComponent(out T component))
			{
				return component;
			}

			parent = parent.parent;
		}

		return null;
	}

	private void SnapToGrid()
	{
		if (_chunk != null)
		{
			float gridSize = Chunk.GridSize * CurrentChunkSettings.Settings.RoadOffset;

			Vector3 newPosition = transform.position;
			newPosition.z = Mathf.Round(transform.position.z / gridSize) * gridSize;
			newPosition.y = 0;

			transform.position = newPosition;

			SnapToLine();
		}
	}

	private void SnapToLine()
	{
		if (_chunk.Lines.Count > 0)
		{
			Line nearestLine = null;
			float nearestDistance = float.PositiveInfinity;

			foreach (Line line in _chunk.Lines)
			{
				float distance = Mathf.Abs(transform.position.x - line.transform.position.x);

				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestLine = line;
				}
			}

			if (nearestLine != null)
			{
				Vector3 newPosition = transform.position;
				newPosition.x = nearestLine.transform.position.x;
				transform.position = newPosition;
				_roadIndex = nearestLine.RoadIndex;
			}
		}
	}
}
