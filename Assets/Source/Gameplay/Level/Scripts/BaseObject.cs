using UnityEngine;

[ExecuteAlways, SelectionBase, RequireComponent(typeof(SnapObjectToGrid))]
public abstract class BaseObject : MonoBehaviour
{
	private SnapObjectToGrid _snapObjectToGrid;

	public SnapObjectToGrid SnapObjectToGrid => _snapObjectToGrid;
	public int RoadIndex => _snapObjectToGrid.RoadIndex;

	public abstract int Init();

	public void Reload()
	{
		SpawnObstacle();
	}

	protected virtual void Awake()
	{
		_snapObjectToGrid = GetComponent<SnapObjectToGrid>();

		if (Application.isPlaying == false)
		{
			if (transform.parent != null && transform.parent.parent != null)
			{
				SpawnObstacle();
			}
		}
	}

	protected abstract void SpawnObstacle();

	protected T FindObjectOfTypeInParents<T>() where T : Component
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
}
