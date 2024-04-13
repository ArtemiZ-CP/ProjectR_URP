using System;
using System.Collections;
using UnityEngine;

public class SwipeInput : MonoBehaviour
{
	[SerializeField] private float _minimumSwipeMagnitude = 10f;
	[SerializeField] private float _swipeDelay = 0.1f;

	public event Action<Vector2> OnSwipe;

	private PlayerControls _playerControls;
	private Vector2 _swipeDirection;
	private bool _isSwiped = false;

	private void Awake()
	{
		_playerControls = new PlayerControls();
	}

	private void OnEnable()
	{
		_playerControls.Enable();
		_playerControls.Player.Swipe.performed += context => _swipeDirection = context.ReadValue<Vector2>();
		_playerControls.Player.Touch.canceled += context => StartSwipeDelay();
		_playerControls.Player.KeyboardMovement.started += context => _swipeDirection = context.ReadValue<Vector2>() * _minimumSwipeMagnitude;
		_playerControls.Player.KeyboardMovement.canceled += context => StartSwipeDelay();
	}

	private void OnDisable()
	{
		_playerControls.Player.Swipe.performed -= context => _swipeDirection = context.ReadValue<Vector2>();
		_playerControls.Player.Touch.canceled -= context => StartSwipeDelay();
		_playerControls.Player.KeyboardMovement.started -= context => _swipeDirection = context.ReadValue<Vector2>() * _minimumSwipeMagnitude;
		_playerControls.Player.KeyboardMovement.canceled -= context => StartSwipeDelay();
		_playerControls.Disable();
		StopAllCoroutines();
	}

	private void Update()
	{
		if (_isSwiped == false)
		{
			Vector2 swipeDirection = ChechSwipe(_swipeDirection);

			if (swipeDirection != Vector2.zero)
			{
				_isSwiped = true;
				Swipe(swipeDirection);
				return;
			}
		}
		else
		{
			_swipeDirection = Vector2.zero;
		}
	}

	private Vector2 ChechSwipe(Vector2 swipeDirection)
	{
		if (swipeDirection.magnitude >= _minimumSwipeMagnitude)
		{
			if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
			{
				if (swipeDirection.x > 0)
				{
					return Vector2.right;
				}
				else
				{
					return Vector2.left;
				}
			}
			else
			{
				if (swipeDirection.y > 0)
				{
					return Vector2.up;
				}
				else
				{
					return Vector2.down;
				}
			}
		}

		return Vector2.zero;
	}

	private void Swipe(Vector2 direction)
	{
		OnSwipe?.Invoke(direction);
	}

	private void StartSwipeDelay()
	{
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(SwipeDelay());
		}
	}

	private IEnumerator SwipeDelay()
	{
		yield return new WaitForSeconds(_swipeDelay);
		_isSwiped = false;
	}
}
