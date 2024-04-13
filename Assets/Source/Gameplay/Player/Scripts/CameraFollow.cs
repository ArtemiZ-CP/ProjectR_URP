using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] private Transform _playerPivotTransform;
	[SerializeField] private Transform _cameraPivotTransform;
	[SerializeField] private PlayerSettings _playerSettings;

	private LayerMask _groundLayer;
	private float _cameraHeight;
	private float _cameraMoveSpeed;

	public Transform CameraPivotTransform => _cameraPivotTransform;

	private void Awake()
	{
		_groundLayer = _playerSettings.GroundLayer;
		_cameraMoveSpeed = _playerSettings.CameraMoveSpeed;
	}

	private void OnEnable()
	{
		StartCoroutine(SmoothMoveCamera(_cameraPivotTransform));
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void FixedUpdate()
	{
		_cameraHeight = GetCameraHeight(GetCameraPivotHeight());
	}

	private float GetCameraHeight(float cameraPivotHeight)
	{
		if (cameraPivotHeight > _cameraPivotTransform.position.y)
		{
			return cameraPivotHeight;
		}
		else if (_playerPivotTransform.position.y < _cameraPivotTransform.position.y)
		{
			return _playerPivotTransform.position.y;
		}

		return _cameraPivotTransform.position.y;
	}

	private float GetCameraPivotHeight()
	{
		Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, float.PositiveInfinity, _groundLayer);

		if (hitInfo.collider != null)
		{
			return hitInfo.point.y;
		}

		Debug.LogWarning("No ground found");
		return _cameraHeight;
	}

	private IEnumerator SmoothMoveCamera(Transform transformToMove)
	{
		float height = transformToMove.position.y;

		while (true)
		{
			float x = transformToMove.position.x;
			float z = transform.position.z;
			height = Mathf.Lerp(height, _cameraHeight, _cameraMoveSpeed * Time.deltaTime);

			transformToMove.position = new Vector3(x, height, z);
			yield return null;
		}
	}
}
