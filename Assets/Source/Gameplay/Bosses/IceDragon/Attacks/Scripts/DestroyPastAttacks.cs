using System.Collections.Generic;
using UnityEngine;

public class DestroyPastAttacks : MonoBehaviour
{
	[SerializeField] private float _destroyOffset;
	[SerializeField] private PlayerMovement _playerMovement;

	private List<Transform> _childrenToDestroy = new();

	private void FixedUpdate()
	{
		_childrenToDestroy.Clear();

		foreach (Transform child in transform)
		{
			if (child.position.z < _destroyOffset + _playerMovement.transform.position.z)
			{
				_childrenToDestroy.Add(child);
			}
		}

		foreach (Transform child in _childrenToDestroy)
		{
			Destroy(child.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(new Vector3(-10, 0, _destroyOffset + _playerMovement.transform.position.z), new Vector3(10, 0, _destroyOffset + _playerMovement.transform.position.z));
	}
}
