using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SwipeInput), typeof(CameraFollow), typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private List<LinePositions> _roads = new();
	[SerializeField] private PlayerSettings _playerSettings;
	[SerializeField] private Acceleration _acceleration;
	[Header("Colliders")]
	[SerializeField] private CapsuleCollider _playerCollider;
	[SerializeField] private CapsuleCollider _baseCapsuleCollider;
	[SerializeField] private CapsuleCollider _lowCapsuleCollider;
	[SerializeField] private bool _showCollider;

	public event Action<float> OnTumble;
	public event Action<bool> OnGroundedChange;
	public event Action OnJump;

	private event Action OnGrounded;
	private event Action OnSwitchRoadEnd;

	private CharacterController _characterController;
	private CameraFollow _cameraFollow;
	private SwipeInput _swipeInput;

	private Vector3 _playerVelocity;
	private float _t = 0;
	private int _lastRoadIndex = 1;
	private int _targetRoadIndex = 1;
	private bool _isMovingHorizontal = false;
	private bool _isGrounded;
	private bool _isTumbling;

	private Coroutine _switchRoadCoroutine;
	private Coroutine _tumbleCoroutine;

	#region Settings

	private float _tumblingDuration;
	private float _gravity;
	private float _runSpeed;
	private float _jumpStartVelocity;
	private float _forceLandStartVelocity;
	private float _switchDuration;
	private float _switchDelay;
	private float _forceSwitchDuration;
	private AnimationCurve _moveCurve;
	private AnimationCurve _forceMoveCurve;
	private LayerMask _groundLayer;

	#endregion

	public float CurrentSpeedMultiplyer => _acceleration.Multiplyer;
	public bool IsGrounded => _isGrounded;

	private void Awake()
	{
		_characterController = GetComponent<CharacterController>();
		_cameraFollow = GetComponent<CameraFollow>();
		_swipeInput = GetComponent<SwipeInput>();

		Init(_playerSettings);

		ChangeCollider(isLow: false);
	}

	private void OnEnable()
	{
		_swipeInput.OnSwipe += OnSwipeHorizontal;
		_swipeInput.OnSwipe += OnSwipeVertical;
		OnSwitchRoadEnd += EndSwitch;
	}

	private void OnDisable()
	{
		_swipeInput.OnSwipe -= OnSwipeHorizontal;
		_swipeInput.OnSwipe -= OnSwipeVertical;
		OnSwitchRoadEnd -= EndSwitch;
		StopAllCoroutines();
	}

	private void Start()
	{
		Vector3 playerStartPosition = _roads[_targetRoadIndex].PlayerLineTransform.position;
		transform.position = new Vector3(playerStartPosition.x, transform.position.y, playerStartPosition.z);
		_cameraFollow.CameraPivotTransform.position = _roads[_targetRoadIndex].CameraTransform.position;
	}

	private void Update()
	{
		CheckGround();
		Move(_runSpeed);
	}

	public bool TryBacktrack(Obstacle obstacle)
	{
		if (_isMovingHorizontal == false)
		{
			return false;
		}

		if (obstacle.RoadIndex == _lastRoadIndex)
		{
			return false;
		}

		if (_switchRoadCoroutine != null)
		{
			StopCoroutine(_switchRoadCoroutine);
		}

		_t = 1 - _t;

		SwitchRoad(_lastRoadIndex - _targetRoadIndex, 0, _switchDuration, _moveCurve);

		return true;
	}

	public void ForceSwitchRoad(int direction)
	{
		if (_switchRoadCoroutine != null)
		{
			return;
		}

		if (_targetRoadIndex == 0)
		{
			direction = 1;
		}
		else if (_targetRoadIndex == _roads.Count - 1)
		{
			direction = -1;
		}
		else if (direction == 0)
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				direction = 1;
			}
			else
			{
				direction = -1;
			}
		}

		SwitchRoad(direction, 0, _forceSwitchDuration, _forceMoveCurve);
	}

	private void EndSwitch()
	{
		_lastRoadIndex = _targetRoadIndex;
		_switchRoadCoroutine = null;
	}

	private void Init(PlayerSettings playerSettings)
	{
		_gravity = playerSettings.Gravity;
		_runSpeed = playerSettings.RunSpeed;
		_jumpStartVelocity = playerSettings.JumpStartVelocity;
		_forceLandStartVelocity = playerSettings.ForceLandStartVelocity;
		_switchDuration = playerSettings.SwitchDuration;
		_switchDelay = playerSettings.SwitchDelay;
		_moveCurve = playerSettings.AnimationCurve;
		_forceSwitchDuration = playerSettings.ForceSwitchDuration;
		_forceMoveCurve = playerSettings.ForceAnimationCurve;
		_tumblingDuration = playerSettings.TumblingDuration;
		_groundLayer = playerSettings.GroundLayer;
	}

	#region OnSwipeActions

	private void OnSwipeHorizontal(Vector2 swipeDirection)
	{
		OnSwipeHorizontal(swipeDirection, _switchDuration, _moveCurve);
	}

	private void OnSwipeHorizontal(Vector2 swipeDirection, float duration, AnimationCurve moveCurve)
	{
		if (_isMovingHorizontal)
		{
			return;
		}

		if (swipeDirection == Vector2.right)
		{
			if (_targetRoadIndex < 2)
			{
				SwitchRoad(1, _switchDelay, duration, moveCurve);
			}
		}
		else if (swipeDirection == Vector2.left)
		{
			if (_targetRoadIndex > 0)
			{
				SwitchRoad(-1, _switchDelay, duration, moveCurve);
			}
		}
	}

	private void OnSwipeVertical(Vector2 swipeDirection)
	{
		if (swipeDirection == Vector2.up && _isGrounded)
		{
			Jump();
		}
		else if (swipeDirection == Vector2.down)
		{
			ForceLand();
		}
	}

	#endregion

	#region Movement

	private void Move(float speed)
	{
		_playerVelocity.z = speed * _acceleration.Multiplyer;

		if (_isGrounded == false)
		{
			_playerVelocity.y -= _gravity * Time.deltaTime;
		}

		_characterController.Move(_playerVelocity * Time.deltaTime);
	}

	private void CheckGround()
	{
		Vector3 origin = transform.position + _characterController.center;
		float radius = _characterController.radius;
		float maxDistance = _characterController.height / 2 - _characterController.radius + 0.1f;

		if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hitInfo, maxDistance, _groundLayer))
		{
			if (_isGrounded == false)
			{
				_isGrounded = true;
				_playerVelocity.y = 0;
				OnGroundedChange?.Invoke(_isGrounded);
			}

			OnGrounded?.Invoke();
		}
		else
		{
			if (_isGrounded)
			{
				_isGrounded = false;
				OnGroundedChange?.Invoke(_isGrounded);
			}
		}

		bool isGrounded = _isGrounded;

		if (isGrounded != _isGrounded && _playerVelocity.y < 0)
		{
			_playerVelocity.y = 0;
		}
	}

	private void Jump()
	{
		_playerVelocity.y = _jumpStartVelocity;
		OnJump?.Invoke();

		if (_tumbleCoroutine != null)
		{
			StopCoroutine(_tumbleCoroutine);
			ChangeCollider(isLow: false);
		}
	}

	private void ForceLand()
	{
		if (_isTumbling == false)
		{
			if (_isGrounded == false)
			{
				_playerVelocity.y = -_forceLandStartVelocity;
			}

			OnGrounded += Tumble;
		}
	}

	#region Tumbling

	public void Tumble()
	{
		_tumbleCoroutine = StartCoroutine(Tumble(_tumblingDuration));
		OnGrounded -= Tumble;
	}

	private IEnumerator Tumble(float duration)
	{
		ChangeCollider(isLow: true);
		OnTumble?.Invoke(duration);

		yield return new WaitForSeconds(duration);

		ChangeCollider(isLow: false);
	}

	private void ChangeCollider(bool isLow)
	{
		_isTumbling = isLow;

		if (isLow)
		{
			SetColliderSize(_characterController, _lowCapsuleCollider);
			SetColliderSize(_playerCollider, _lowCapsuleCollider);
		}
		else
		{
			SetColliderSize(_characterController, _baseCapsuleCollider);
			SetColliderSize(_playerCollider, _baseCapsuleCollider);
		}

		_lowCapsuleCollider.gameObject.SetActive(isLow);
		_baseCapsuleCollider.gameObject.SetActive(!isLow);
	}

	private void SetColliderSize(CapsuleCollider targetCollider, CapsuleCollider collider)
	{
		targetCollider.height = collider.height;
		targetCollider.center = collider.center;
		targetCollider.radius = collider.radius;
	}

	private void SetColliderSize(CharacterController targetCollider, CapsuleCollider collider)
	{
		targetCollider.height = collider.height;
		targetCollider.center = collider.center;
		targetCollider.radius = collider.radius;
	}

	#endregion

	#region Switch road

	private void SwitchRoad(int direction, float switchDelay, float duration, AnimationCurve moveCurve)
	{
		_lastRoadIndex = _targetRoadIndex;
		_targetRoadIndex = _lastRoadIndex + direction;
		_switchRoadCoroutine = StartCoroutine(SwitchRoad(_lastRoadIndex, _targetRoadIndex, switchDelay, duration / _acceleration.Multiplyer, moveCurve));
	}

	private IEnumerator SwitchRoad(int fromRoadIndex, int toRoadIndex, float switchDelay, float switchDuration, AnimationCurve moveCurve)
	{
		_isMovingHorizontal = true;
		float time = _t * switchDuration;

		yield return new WaitForSeconds(switchDelay);

		LinePositions fromRoad = _roads[fromRoadIndex];
		LinePositions toRoad = _roads[toRoadIndex];

		while (time < switchDuration)
		{
			_t = moveCurve.Evaluate(time / switchDuration);

			LerpX(transform, fromRoad.PlayerLineTransform, toRoad.PlayerLineTransform, _t);
			LerpX(_cameraFollow.CameraPivotTransform, fromRoad.CameraTransform, toRoad.CameraTransform, _t);

			time += Time.deltaTime;

			yield return null;
		}

		SetPositionX(transform, toRoad.PlayerLineTransform.position.x);
		SetPositionX(_cameraFollow.CameraPivotTransform, toRoad.CameraTransform.position.x);

		_t = 0;
		_isMovingHorizontal = false;
		OnSwitchRoadEnd?.Invoke();
	}

	private void LerpX(Transform transformToMove, Transform from, Transform to, float t)
	{
		float y = transformToMove.transform.position.y;
		float z = transformToMove.transform.position.z;

		transformToMove.transform.position = new Vector3(Mathf.Lerp(from.position.x, to.position.x, t), y, z);
	}

	private void SetPositionX(Transform transform, float x)
	{
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}

	#endregion

	#endregion

	private void OnDrawGizmos()
	{
		if (_characterController != null)
		{
			Vector3 origin = transform.position + _characterController.center;
			float radius = _characterController.radius;
			float maxDistance = _characterController.height / 2 - _characterController.radius + 0.1f;

			Gizmos.color = _isGrounded ? Color.green : Color.red;
			Gizmos.DrawWireSphere(origin + maxDistance * Vector3.down, radius);
		}
	}
}
