using System;
using UnityEngine;

[Serializable]
public class LinePositions
{
	[SerializeField] private Transform _playerLineTransform;
	[SerializeField] private Transform _cameraTransform;

	public Transform PlayerLineTransform => _playerLineTransform;
	public Transform CameraTransform => _cameraTransform;
}
