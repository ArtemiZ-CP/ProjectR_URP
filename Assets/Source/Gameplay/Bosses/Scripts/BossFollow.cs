using UnityEngine;

public class BossFollow : MonoBehaviour
{
	[SerializeField] private PlayerMovement _playerMovement;

	private Vector3 _startPosition;

	private void Start()
	{
		_startPosition = transform.position;
	}

	private void Update()
	{
		if (_playerMovement != null)
		{
			transform.position = _playerMovement.transform.position.z * Vector3.forward + _startPosition;
		}
	}
}
