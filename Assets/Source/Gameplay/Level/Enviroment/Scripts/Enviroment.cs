using UnityEngine;

public class Enviroment : MonoBehaviour
{
	[SerializeField, Min(0)] private float _length;

	public float Length => _length;

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * _length);
	}
}
