using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/Player/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
	[SerializeField] private float _runSpeed;
	[Header("Jump and Land")]
	[SerializeField] private float _jumpStartVelocity;
	[SerializeField] private float _forceLandStartVelocity;
	[SerializeField] private float _tumblingDuration;
	[SerializeField] private float _gravity;
	[SerializeField] private LayerMask _groundLayer;
	[Header("Switch road")]
	[SerializeField] private float _switchDuration;
	[SerializeField] private float _switchDelay;
	[SerializeField] private AnimationCurve _animationCurve;
	[Header("Force switch road")]
	[SerializeField] private float _forceSwitchDuration;
	[SerializeField] private AnimationCurve _forceAnimationCurve;
	[Header("Camera")]
	[SerializeField] private float _cameraMoveSpeed;

	public float Gravity => _gravity;
	public float CameraMoveSpeed => _cameraMoveSpeed;
	public float RunSpeed => _runSpeed;
	public float JumpStartVelocity => _jumpStartVelocity;
	public float ForceLandStartVelocity => _forceLandStartVelocity;
	public float SwitchDuration => _switchDuration;
	public float SwitchDelay => _switchDelay;
	public AnimationCurve AnimationCurve => _animationCurve;
	public float ForceSwitchDuration => _forceSwitchDuration;
	public AnimationCurve ForceAnimationCurve => _forceAnimationCurve;
	public float TumblingDuration => _tumblingDuration;
	public LayerMask GroundLayer => _groundLayer;
}
