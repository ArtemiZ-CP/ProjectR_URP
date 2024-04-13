using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SelectRandom : MonoBehaviour
{
	[SerializeField] private List<GameObject> _meshes;

	private void Start()
	{
		SelectRandomMesh();
	}

	private void SelectRandomMesh()
	{
		foreach (var mesh in _meshes)
		{
			mesh.SetActive(false);
		}

		int randomIndex = Random.Range(0, _meshes.Count);
		_meshes[randomIndex].SetActive(true);
	}
}
